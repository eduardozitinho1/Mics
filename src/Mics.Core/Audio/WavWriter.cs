namespace Mics.Core.Audio;

public static class WavWriter
{
    public static void Write(string path, double[] samples, int sampleRate = Renderer.DefaultSampleRate)
    {
        using var fs = File.Create(path);
        using var bw = new BinaryWriter(fs);

        const short channels = 1;
        const short bitsPerSample = 16;
        var byteRate = sampleRate * channels * bitsPerSample / 8;
        var blockAlign = (short)(channels * bitsPerSample / 8);
        var dataSize = samples.Length * blockAlign;

        WriteAscii(bw, "RIFF");
        bw.Write(36 + dataSize);
        WriteAscii(bw, "WAVE");

        WriteAscii(bw, "fmt ");
        bw.Write(16);
        bw.Write((short)1);
        bw.Write(channels);
        bw.Write(sampleRate);
        bw.Write(byteRate);
        bw.Write(blockAlign);
        bw.Write(bitsPerSample);

        WriteAscii(bw, "data");
        bw.Write(dataSize);

        foreach (var s in samples)
        {
            var clamped = Math.Clamp(s, -1.0, 1.0);
            bw.Write((short)(clamped * short.MaxValue));
        }
    }

    private static void WriteAscii(BinaryWriter bw, string text)
    {
        foreach (var c in text)
            bw.Write((byte)c);
    }
}
