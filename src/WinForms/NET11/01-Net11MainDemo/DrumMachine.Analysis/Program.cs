using WinForms.Audio.Core;
using WinForms.Audio.Percussion;
using WinForms.Audio.Playback;

if (args.Length != 2 || args[0] is not ("--render-stock" or "--validate-wavs"))
{
    Console.Error.WriteLine("Usage: DrumMachine.Analysis --render-stock <output-folder> | --validate-wavs <input-folder>");
    return 2;
}

try
{
    const int rate = 48_000;
    if (args[0] == "--validate-wavs")
    {
        string[] paths = Directory.GetFiles(args[1], "*.wav");
        if (paths.Length == 0)
        {
            throw new InvalidOperationException("The input folder contains no WAV files.");
        }

        foreach (string path in paths)
        {
            Sample sample = await Sample.FromWaveFileAsync(path, rate);
            Console.WriteLine($"{Path.GetFileName(path)}: {sample.Duration.TotalSeconds:F3}s");
        }

        return 0;
    }

    Directory.CreateDirectory(args[1]);
    foreach (Cr78Instrument instrument in Cr78Kit.Instruments)
    {
        IVoice voice = Cr78Kit.CreateVoice(rate, instrument, 1, 0, noiseSeed: 42);
        List<short> pcm = [];
        while (!voice.IsFinished && pcm.Count < rate * 3)
        {
            pcm.Add((short)Math.Clamp((int)Math.Round(voice.Next() * 32767), short.MinValue, short.MaxValue));
        }

        if (!voice.IsFinished)
        {
            throw new InvalidOperationException($"{instrument} did not reach silence.");
        }

        string path = Path.Combine(args[1], instrument + ".wav");
        await File.WriteAllBytesAsync(path, WaveFile.ToWavBytes(pcm.ToArray(), rate));
        Console.WriteLine($"{instrument}: {pcm.Count / (double)rate:F3}s");
    }

    return 0;
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException or
    InvalidOperationException or FormatException or NotSupportedException)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}
