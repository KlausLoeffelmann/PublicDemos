using System.Buffers.Binary;

namespace WinForms.Audio.Playback;

/// <summary>
///  A recorded sound, already converted to mono <see langword="float"/> at a target sample rate.
///  Conversion happens once on load, never on playback.
/// </summary>
public sealed class Sample
{
    private Sample(float[] data, int sampleRate)
    {
        Data = data;
        SampleRate = sampleRate;
    }

    /// <summary>
    ///  Mono samples in -1..1.
    /// </summary>
    public float[] Data { get; }

    /// <summary>
    ///  The rate <see cref="Data"/> is at; must match the engine.
    /// </summary>
    public int SampleRate { get; }

    /// <summary>
    ///  Playback length.
    /// </summary>
    public TimeSpan Duration
        => TimeSpan.FromSeconds(Data.Length / (double)SampleRate);

    /// <summary>
    ///  Wraps 16-bit PCM as a sample, resampling if the rates differ.
    /// </summary>
    public static Sample FromPcm(ReadOnlySpan<short> pcm, int sourceRate, int channels = 1, int targetRate = 48_000)
    {
        float[] mono = new float[pcm.Length / channels];

        for (int i = 0; i < mono.Length; i++)
        {
            float sum = 0;

            for (int c = 0; c < channels; c++)
            {
                sum += pcm[i * channels + c];
            }

            mono[i] = sum / channels / short.MaxValue;
        }

        return new Sample(Resample(mono, sourceRate, targetRate), targetRate);
    }

    /// <summary>
    ///  Loads a WAV file (PCM 8/16/24/32-bit or 32-bit float, any channel count) as mono at the target rate.
    /// </summary>
    public static async Task<Sample> FromWaveFileAsync(string path, int targetRate = 48_000, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        return await FromWaveStreamAsync(stream, targetRate, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///  Loads a WAV stream (PCM 8/16/24/32-bit or 32-bit float, any channel count) as mono at the target rate.
    /// </summary>
    public static async Task<Sample> FromWaveStreamAsync(Stream stream, int targetRate = 48_000, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using MemoryStream buffer = new();
        await stream.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);

        return Decode(buffer.GetBuffer().AsSpan(0, (int)buffer.Length), targetRate);
    }

    private static Sample Decode(ReadOnlySpan<byte> wav, int targetRate)
    {
        if (wav.Length < 12 || !wav[..4].SequenceEqual("RIFF"u8) || !wav[8..12].SequenceEqual("WAVE"u8))
        {
            throw new FormatException("Not a RIFF/WAVE file.");
        }

        int formatTag = 0, channels = 0, sampleRate = 0, bitsPerSample = 0;
        ReadOnlySpan<byte> data = default;
        int offset = 12;

        while (offset + 8 <= wav.Length)
        {
            ReadOnlySpan<byte> id = wav.Slice(offset, 4);
            int size = BinaryPrimitives.ReadInt32LittleEndian(wav.Slice(offset + 4, 4));
            int bodyStart = offset + 8;
            int bodyLength = Math.Min(size, wav.Length - bodyStart);
            ReadOnlySpan<byte> body = wav.Slice(bodyStart, bodyLength);

            if (id.SequenceEqual("fmt "u8))
            {
                formatTag = BinaryPrimitives.ReadUInt16LittleEndian(body);
                channels = BinaryPrimitives.ReadUInt16LittleEndian(body[2..]);
                sampleRate = BinaryPrimitives.ReadInt32LittleEndian(body[4..]);
                bitsPerSample = BinaryPrimitives.ReadUInt16LittleEndian(body[14..]);

                // WAVE_FORMAT_EXTENSIBLE carries the real tag in the sub-format GUID's first two bytes.
                if (formatTag == 0xFFFE && body.Length >= 26)
                {
                    formatTag = BinaryPrimitives.ReadUInt16LittleEndian(body[24..]);
                }
            }
            else if (id.SequenceEqual("data"u8))
            {
                data = body;
            }

            offset = bodyStart + size + (size & 1);
        }

        if (channels == 0 || data.IsEmpty)
        {
            throw new FormatException("WAV file has no fmt or data chunk.");
        }

        int bytesPerSample = bitsPerSample / 8;
        int frames = data.Length / (bytesPerSample * channels);
        float[] mono = new float[frames];

        for (int i = 0; i < frames; i++)
        {
            float sum = 0;

            for (int c = 0; c < channels; c++)
            {
                ReadOnlySpan<byte> s = data.Slice((i * channels + c) * bytesPerSample, bytesPerSample);

                sum += (formatTag, bitsPerSample) switch
                {
                    (3, 32) => BinaryPrimitives.ReadSingleLittleEndian(s),
                    (1, 8) => (s[0] - 128) / 128f,
                    (1, 16) => BinaryPrimitives.ReadInt16LittleEndian(s) / 32768f,
                    (1, 24) => ((s[0] | (s[1] << 8) | (s[2] << 16)) << 8 >> 8) / 8_388_608f,
                    (1, 32) => BinaryPrimitives.ReadInt32LittleEndian(s) / 2_147_483_648f,
                    _ => throw new NotSupportedException($"WAV format tag {formatTag} with {bitsPerSample} bits is not supported.")
                };
            }

            mono[i] = sum / channels;
        }

        return new Sample(Resample(mono, sampleRate, targetRate), targetRate);
    }

    private static float[] Resample(float[] source, int sourceRate, int targetRate)
    {
        if (sourceRate == targetRate || source.Length == 0)
        {
            return source;
        }

        double ratio = sourceRate / (double)targetRate;
        float[] result = new float[(int)(source.Length / ratio)];

        for (int i = 0; i < result.Length; i++)
        {
            double position = i * ratio;
            int index = (int)position;
            float fraction = (float)(position - index);
            float a = source[Math.Min(index, source.Length - 1)];
            float b = source[Math.Min(index + 1, source.Length - 1)];

            result[i] = a + (b - a) * fraction;
        }

        return result;
    }
}

/// <summary>
///  The 44-byte header trick, for the first slide: PCM in, WAV bytes out, hand it to SoundPlayer.
///  Also the reason SoundPlayer is a dead end: it plays a finished file, it can't mix or stream.
/// </summary>
public static class WaveFile
{
    /// <summary>
    ///  Wraps 16-bit PCM in a RIFF/WAVE container.
    /// </summary>
    public static byte[] ToWavBytes(ReadOnlySpan<short> pcm, int sampleRate, int channels = 1)
    {
        int dataBytes = pcm.Length * 2;
        byte[] wav = new byte[44 + dataBytes];
        Span<byte> w = wav;

        "RIFF"u8.CopyTo(w);
        BinaryPrimitives.WriteInt32LittleEndian(w[4..], 36 + dataBytes);
        "WAVE"u8.CopyTo(w[8..]);
        "fmt "u8.CopyTo(w[12..]);
        BinaryPrimitives.WriteInt32LittleEndian(w[16..], 16);
        BinaryPrimitives.WriteInt16LittleEndian(w[20..], 1);
        BinaryPrimitives.WriteInt16LittleEndian(w[22..], (short)channels);
        BinaryPrimitives.WriteInt32LittleEndian(w[24..], sampleRate);
        BinaryPrimitives.WriteInt32LittleEndian(w[28..], sampleRate * channels * 2);
        BinaryPrimitives.WriteInt16LittleEndian(w[32..], (short)(channels * 2));
        BinaryPrimitives.WriteInt16LittleEndian(w[34..], 16);
        "data"u8.CopyTo(w[36..]);
        BinaryPrimitives.WriteInt32LittleEndian(w[40..], dataBytes);

        for (int i = 0; i < pcm.Length; i++)
        {
            BinaryPrimitives.WriteInt16LittleEndian(w[(44 + i * 2)..], pcm[i]);
        }

        return wav;
    }

    /// <summary>
    ///  Generates one second of sine at a frequency. The anticlimax.
    /// </summary>
    public static short[] Sine(double frequency, int sampleRate = 48_000, double seconds = 1, double amplitude = 0.5)
    {
        short[] pcm = new short[(int)(sampleRate * seconds)];

        for (int i = 0; i < pcm.Length; i++)
        {
            double t = i / (double)sampleRate;
            pcm[i] = (short)(Math.Sin(Math.Tau * frequency * t) * short.MaxValue * amplitude);
        }

        return pcm;
    }
}
