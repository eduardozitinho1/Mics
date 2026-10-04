using Mics.Core.Audio;
using Mics.Core.Mapping;
using Mics.Core.Score;
using Xunit;

namespace Mics.Tests;

public class DeterminismTests
{
    [Fact]
    public void SonifySource_is_deterministic_across_voices()
    {
        var code = "public class Foo { public int Bar() { return 42; } }";
        var a = Sonifier.SonifySource(code);
        var b = Sonifier.SonifySource(code);

        Assert.Equal(a.Voices.Count, b.Voices.Count);
        for (int i = 0; i < a.Voices.Count; i++)
        {
            var va = a.Voices[i];
            var vb = b.Voices[i];

            Assert.Equal(va.Name, vb.Name);
            Assert.Equal(va.Instrument.Name, vb.Instrument.Name);
            Assert.Equal(va.Notes.Count, vb.Notes.Count);

            for (int j = 0; j < va.Notes.Count; j++)
            {
                Assert.Equal(va.Notes[j].Frequency, vb.Notes[j].Frequency);
                Assert.Equal(va.Notes[j].StartTime, vb.Notes[j].StartTime);
                Assert.Equal(va.Notes[j].DurationSeconds, vb.Notes[j].DurationSeconds);
                Assert.Equal(va.Notes[j].Velocity, vb.Notes[j].Velocity);
            }
        }
    }

    [Fact]
    public void SonifySource_is_deterministic_across_renders()
    {
        var code = "public class Foo { public int Bar(int x) { if (x > 0) return 1; return 0; } }";
        var comp = Sonifier.SonifySource(code);

        var a = Renderer.Render(comp, 8000);
        var b = Renderer.Render(comp, 8000);

        Assert.Equal(a.Length, b.Length);
        for (int i = 0; i < a.Length; i++)
            Assert.Equal(a[i], b[i]);
    }
}

public class WavStructuralTests
{
    [Fact]
    public void Wav_has_correct_header_fields()
    {
        var samples = new double[] { 0.0, 0.5, -0.5, 0.0 };
        var sampleRate = 8000;
        var bytes = BuildWav(samples, sampleRate);

        // "RIFF" + chunk size + "WAVE"
        Assert.Equal((byte)'R', bytes[0]);
        Assert.Equal((byte)'I', bytes[1]);
        Assert.Equal((byte)'F', bytes[2]);
        Assert.Equal((byte)'F', bytes[3]);

        var riffSize = BitConverter.ToInt32(bytes, 4);
        Assert.Equal(bytes.Length - 8, riffSize);

        Assert.Equal((byte)'W', bytes[8]);
        Assert.Equal((byte)'A', bytes[9]);
        Assert.Equal((byte)'V', bytes[10]);
        Assert.Equal((byte)'E', bytes[11]);

        // "fmt " + 16 + PCM(1) + channels(1)
        Assert.Equal((byte)'f', bytes[12]);
        Assert.Equal((byte)'m', bytes[13]);
        Assert.Equal((byte)'t', bytes[14]);
        Assert.Equal((byte)' ', bytes[15]);

        var fmtSize = BitConverter.ToInt32(bytes, 16);
        Assert.Equal(16, fmtSize);

        var audioFormat = BitConverter.ToInt16(bytes, 20);
        Assert.Equal(1, audioFormat);

        var channels = BitConverter.ToInt16(bytes, 22);
        Assert.Equal(1, channels);

        var sr = BitConverter.ToInt32(bytes, 24);
        Assert.Equal(sampleRate, sr);

        var byteRate = BitConverter.ToInt32(bytes, 28);
        Assert.Equal(sampleRate * 1 * 16 / 8, byteRate);

        var blockAlign = BitConverter.ToInt16(bytes, 32);
        Assert.Equal(2, blockAlign);

        var bitsPerSample = BitConverter.ToInt16(bytes, 34);
        Assert.Equal(16, bitsPerSample);

        // "data" + size
        Assert.Equal((byte)'d', bytes[36]);
        Assert.Equal((byte)'a', bytes[37]);
        Assert.Equal((byte)'t', bytes[38]);
        Assert.Equal((byte)'a', bytes[39]);

        var dataSize = BitConverter.ToInt32(bytes, 40);
        Assert.Equal(samples.Length * 2, dataSize);
    }

    [Fact]
    public void Wav_samples_are_little_endian_shorts()
    {
        var bytes = BuildWav(new double[] { 0.0, 0.5, -0.5 }, 8000);
        var pcmStart = 44;

        var s0 = BitConverter.ToInt16(bytes, pcmStart + 0);
        var s1 = BitConverter.ToInt16(bytes, pcmStart + 2);
        var s2 = BitConverter.ToInt16(bytes, pcmStart + 4);

        Assert.Equal(0, s0);
        Assert.Equal((short)(0.5 * short.MaxValue), s1);
        Assert.Equal((short)(-0.5 * short.MaxValue), s2);
    }

    [Fact]
    public void Wav_clamps_out_of_range_samples()
    {
        var bytes = BuildWav(new double[] { 100.0, -100.0 }, 8000);
        var pcmStart = 44;

        var s0 = BitConverter.ToInt16(bytes, pcmStart + 0);
        var s1 = BitConverter.ToInt16(bytes, pcmStart + 2);

        Assert.Equal(short.MaxValue, s0);
        Assert.Equal(unchecked((short)-short.MaxValue), s1);
    }

    private static byte[] BuildWav(double[] samples, int sampleRate)
    {
        using var ms = new MemoryStream();
        WavWriter.Write(ms, samples, sampleRate);
        return ms.ToArray();
    }
}
