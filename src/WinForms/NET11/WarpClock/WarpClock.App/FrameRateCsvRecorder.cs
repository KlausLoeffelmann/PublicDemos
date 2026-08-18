using System.Diagnostics;
using System.Globalization;

namespace WarpClock.App;

/// <summary>
///  Writes periodic frame-rate samples to a CSV file.
/// </summary>
internal sealed class FrameRateCsvRecorder : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public FrameRateCsvRecorder(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string? directory = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directory))
        {
            throw new InvalidOperationException($"Path '{filePath}' does not contain a directory.");
        }

        Directory.CreateDirectory(directory);

        FilePath = filePath;
        _writer = new StreamWriter(filePath, append: false);
        _writer.WriteLine("TimestampLocal,ElapsedMilliseconds,SampleIndex,WindowSeconds,AverageFramesPerSecond,ThemeKey,ThemeDisplayName,PresentationMode,VSyncEnabled,AlwaysOn,OledView");
        _writer.Flush();
    }

    public string FilePath { get; }

    public void WriteSample(
        DateTime timestampLocal,
        int sampleIndex,
        int windowSeconds,
        double framesPerSecond,
        string themeKey,
        string themeDisplayName,
        WindowPresentationMode presentationMode,
        bool vSyncEnabled,
        bool alwaysOn,
        bool oledView)
    {
        string[] fields =
        [
            timestampLocal.ToString("O", CultureInfo.InvariantCulture),
            _stopwatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture),
            sampleIndex.ToString(CultureInfo.InvariantCulture),
            windowSeconds.ToString(CultureInfo.InvariantCulture),
            framesPerSecond.ToString("0.###", CultureInfo.InvariantCulture),
            Escape(themeKey),
            Escape(themeDisplayName),
            presentationMode.ToString(),
            vSyncEnabled ? "true" : "false",
            alwaysOn ? "true" : "false",
            oledView ? "true" : "false",
        ];

        _writer.WriteLine(string.Join(",", fields));
        _writer.Flush();
    }

    public void Dispose() => _writer.Dispose();

    private static string Escape(string value)
    {
        value ??= string.Empty;

        return value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r')
            ? $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : value;
    }
}
