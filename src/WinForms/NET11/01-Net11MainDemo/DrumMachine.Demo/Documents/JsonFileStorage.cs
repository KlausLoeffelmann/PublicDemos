using System.Text.Json;

namespace DrumMachine.Demo.Documents;

/// <summary>
///  Bounds JSON file reads and publishes completed writes through an exclusively owned same-directory file.
/// </summary>
internal static class JsonFileStorage
{
    /// <summary>
    ///  Reads an explicitly bounded file asynchronously without allowing a growing file to bypass the limit.
    /// </summary>
    internal static async Task<byte[]> ReadAsync(string path, int maximumBytes, CancellationToken cancellationToken)
    {
        await using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 65_536, FileOptions.Asynchronous | FileOptions.SequentialScan);
        byte[] contents = new byte[ValidateLength(stream.Length, maximumBytes)];
        await stream.ReadExactlyAsync(contents, cancellationToken).ConfigureAwait(false);
        if (await stream.ReadAsync(new byte[1], cancellationToken).ConfigureAwait(false) != 0)
        {
            throw new InvalidDataException("The JSON file changed size while it was being read.");
        }

        return contents;
    }

    /// <summary>
    ///  Reads the small startup preferences file with the same byte and growth checks as document loading.
    /// </summary>
    internal static byte[] Read(string path, int maximumBytes)
    {
        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        byte[] contents = new byte[ValidateLength(stream.Length, maximumBytes)];
        stream.ReadExactly(contents);
        if (stream.ReadByte() != -1)
        {
            throw new InvalidDataException("The JSON file changed size while it was being read.");
        }

        return contents;
    }

    /// <summary>
    ///  Serializes off the caller's thread, flushes to disk, and atomically publishes only a complete file.
    /// </summary>
    internal static Task WriteAsync(
        string path,
        Action<Utf8JsonWriter> writeJson,
        int maximumBytes,
        bool createDirectory,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(writeJson);
        return Task.Run(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            string destination = Path.GetFullPath(path);
            string directory = Path.GetDirectoryName(destination)
                ?? throw new ArgumentException("A destination directory is required.", nameof(path));
            if (createDirectory)
            {
                Directory.CreateDirectory(directory);
            }

            string stagingPath = Path.Combine(directory, $".drum-json-{Guid.NewGuid():N}.tmp");
            bool ownsStagingFile = false;
            try
            {
                await using (FileStream stream = new(stagingPath, FileMode.CreateNew, FileAccess.Write,
                    FileShare.None, bufferSize: 65_536, FileOptions.Asynchronous))
                {
                    ownsStagingFile = true;
                    await using (Utf8JsonWriter writer = new(stream, new JsonWriterOptions { Indented = true }))
                    {
                        writeJson(writer);
                        await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                    }

                    ValidateLength(stream.Length, maximumBytes);
                    await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                    stream.Flush(flushToDisk: true);
                }

                cancellationToken.ThrowIfCancellationRequested();
                if (File.Exists(destination))
                {
                    File.Replace(stagingPath, destination, destinationBackupFileName: null);
                }
                else
                {
                    // A competing first save must not overwrite a file that appeared after the check.
                    File.Move(stagingPath, destination);
                }

                ownsStagingFile = false;
            }
            finally
            {
                if (ownsStagingFile)
                {
                    try
                    {
                        File.Delete(stagingPath);
                    }
                    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                    {
                        AppLogger.Warning("Files", $"Could not remove owned staging file '{stagingPath}'.", ex);
                    }
                }
            }
        }, cancellationToken);
    }

    private static int ValidateLength(long length, int maximumBytes)
    {
        if (length <= 0 || length > maximumBytes)
        {
            throw new InvalidDataException($"JSON files must contain between 1 and {maximumBytes:N0} bytes.");
        }

        return checked((int)length);
    }
}
