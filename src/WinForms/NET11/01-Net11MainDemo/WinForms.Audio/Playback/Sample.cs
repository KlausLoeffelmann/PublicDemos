using System.Buffers.Binary;

namespace WinForms.Audio.Playback;

/// <summary>
///  A recorded sound, already converted to mono <see langword="float"/> at a target sample rate.
///  Conversion happens once on load, never on playback.
/// </summary>
public sealed class Sample
{
    /// <summary>
    ///  Bounds user-supplied WAV input before allocating or decoding its contents.
    /// </summary>
    public const int MaximumWaveFileBytes = 64 * 1024 * 1024;

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
        ValidateRates(sourceRate, targetRate);
        if (channels is < 1 or > 32 || pcm.Length % channels != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(channels), "PCM must contain complete frames with one through thirty-two channels.");
        }

        float[] mono = new float[pcm.Length / channels];

        for (int i = 0; i < mono.Length; i++)
        {
            float sum = 0;

            for (int c = 0; c < channels; c++)
            {
                sum += pcm[i * channels + c];
            }

            mono[i] = sum / channels / 32768f;
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
        ValidateRates(targetRate, targetRate);

        using MemoryStream buffer = new();
        byte[] chunk = new byte[65_536];
        int read;
        while ((read = await stream.ReadAsync(chunk, cancellationToken).ConfigureAwait(false)) != 0)
        {
            if (buffer.Length + read > MaximumWaveFileBytes)
            {
                throw new InvalidDataException("The WAV file exceeds the 64 MiB import limit.");
            }

            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
        }

        return Decode(buffer.GetBuffer().AsSpan(0, (int)buffer.Length), targetRate);
    }

    private static Sample Decode(ReadOnlySpan<byte> wav, int targetRate)
    {
        if (wav.Length < 12 || !wav[..4].SequenceEqual("RIFF"u8) || !wav[8..12].SequenceEqual("WAVE"u8))
        {
            throw new FormatException("Not a RIFF/WAVE file.");
        }

        long riffLength = BinaryPrimitives.ReadUInt32LittleEndian(wav[4..]) + 8L;
        if (riffLength != wav.Length)
        {
            throw new FormatException("The WAV RIFF length does not match its contents.");
        }

        int formatTag = 0, channels = 0, sampleRate = 0, bitsPerSample = 0, blockAlign = 0;
        bool foundFormat = false, foundData = false;
        ReadOnlySpan<byte> data = default;
        int offset = 12;

        while (offset + 8 <= wav.Length)
        {
            ReadOnlySpan<byte> id = wav.Slice(offset, 4);
            uint unsignedSize = BinaryPrimitives.ReadUInt32LittleEndian(wav.Slice(offset + 4, 4));
            int bodyStart = offset + 8;
            if (unsignedSize > wav.Length - bodyStart)
            {
                throw new FormatException("A WAV chunk is truncated or has an invalid length.");
            }

            int size = (int)unsignedSize;
            ReadOnlySpan<byte> body = wav.Slice(bodyStart, size);

            if (id.SequenceEqual("fmt "u8))
            {
                if (foundFormat || body.Length < 16)
                {
                    throw new FormatException("A WAV requires one complete format chunk.");
                }

                foundFormat = true;
                formatTag = BinaryPrimitives.ReadUInt16LittleEndian(body);
                channels = BinaryPrimitives.ReadUInt16LittleEndian(body[2..]);
                sampleRate = BinaryPrimitives.ReadInt32LittleEndian(body[4..]);
                bitsPerSample = BinaryPrimitives.ReadUInt16LittleEndian(body[14..]);
                blockAlign = BinaryPrimitives.ReadUInt16LittleEndian(body[12..]);

                // WAVE_FORMAT_EXTENSIBLE carries the real tag in the sub-format GUID's first two bytes.
                if (formatTag == 0xFFFE)
                {
                    if (body.Length < 40 || BinaryPrimitives.ReadUInt16LittleEndian(body[16..]) < 22 ||
                        !body[26..40].SequenceEqual(new byte[] { 0, 0, 0, 0, 0x10, 0, 0x80, 0, 0, 0xAA, 0, 0x38, 0x9B, 0x71 }))
                    {
                        throw new FormatException("The extensible WAV format is incomplete or unsupported.");
                    }

                    formatTag = BinaryPrimitives.ReadUInt16LittleEndian(body[24..]);
                }
            }
            else if (id.SequenceEqual("data"u8))
            {
                if (foundData)
                {
                    throw new FormatException("Multiple WAV data chunks are not supported.");
                }

                foundData = true;
                data = body;
            }

            offset = bodyStart + size + (size & 1);
        }

        if (!foundFormat || channels is < 1 or > 32 || data.IsEmpty)
        {
            throw new FormatException("WAV file has no fmt or data chunk.");
        }

        ValidateRates(sampleRate, targetRate);
        if (!((formatTag == 1 && bitsPerSample is 8 or 16 or 24 or 32) ||
            (formatTag == 3 && bitsPerSample == 32)))
        {
            throw new NotSupportedException($"WAV format tag {formatTag} with {bitsPerSample} bits is not supported.");
        }

        int bytesPerSample = bitsPerSample / 8;
        if (blockAlign != bytesPerSample * channels || data.Length % blockAlign != 0)
        {
            throw new FormatException("WAV data must contain complete, correctly aligned frames.");
        }

        int frames = data.Length / (bytesPerSample * channels);
        if ((long)frames * targetRate / sampleRate > MaximumWaveFileBytes / sizeof(float))
        {
            throw new InvalidDataException("The resampled WAV exceeds the supported decoded size.");
        }
        float[] mono = new float[frames];

        for (int i = 0; i < frames; i++)
        {
            float sum = 0;

            for (int c = 0; c < channels; c++)
            {
                ReadOnlySpan<byte> s = data.Slice((i * channels + c) * bytesPerSample, bytesPerSample);

                float value = (formatTag, bitsPerSample) switch
                {
                    (3, 32) => BinaryPrimitives.ReadSingleLittleEndian(s),
                    (1, 8) => (s[0] - 128) / 128f,
                    (1, 16) => BinaryPrimitives.ReadInt16LittleEndian(s) / 32768f,
                    (1, 24) => ((s[0] | (s[1] << 8) | (s[2] << 16)) << 8 >> 8) / 8_388_608f,
                    (1, 32) => BinaryPrimitives.ReadInt32LittleEndian(s) / 2_147_483_648f,
                    _ => throw new NotSupportedException($"WAV format tag {formatTag} with {bitsPerSample} bits is not supported.")
                };
                if (!float.IsFinite(value) || Math.Abs(value) > 1)
                {
                    throw new FormatException("WAV audio must contain finite values within -1..1.");
                }

                sum += value;
            }

            mono[i] = sum / channels;
        }

        return new Sample(Resample(mono, sampleRate, targetRate), targetRate);
    }

    private static void ValidateRates(int sourceRate, int targetRate)
    {
        if (sourceRate is < 8_000 or > 384_000 || targetRate is < 8_000 or > 384_000)
        {
            throw new ArgumentOutOfRangeException(nameof(sourceRate), "WAV source and target rates must be between 8 and 384 kHz.");
        }
    }

    private static float[] Resample(float[] source, int sourceRate, int targetRate)
    {
        if (sourceRate == targetRate || source.Length == 0)
        {
            return source;
        }

        double ratio = sourceRate / (double)targetRate;
        float[] result = new float[checked((int)((long)source.Length * targetRate / sourceRate))];

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
