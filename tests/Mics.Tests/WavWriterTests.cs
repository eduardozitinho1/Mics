using Mics.Core.Audio;
using Xunit;

namespace Mics.Tests;

public class WavWriterTests
{
    [Fact]
    public void Write_creates_file_with_riff_header()
    {
        var path = Path.GetTempFileName() + ".wav";
        try
        {
            WavWriter.Write(path, new double[] { 0.0, 0.5, -0.5, 0.0 });
            Assert.True(File.Exists(path));
            var bytes = File.ReadAllBytes(path);
            Assert.True(bytes.Length > 44);
            Assert.Equal((byte)'R', bytes[0]);
            Assert.Equal((byte)'I', bytes[1]);
            Assert.Equal((byte)'F', bytes[2]);
            Assert.Equal((byte)'F', bytes[3]);
            Assert.Equal((byte)'W', bytes[8]);
            Assert.Equal((byte)'A', bytes[9]);
            Assert.Equal((byte)'V', bytes[10]);
            Assert.Equal((byte)'E', bytes[11]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Write_clamps_out_of_range_samples()
    {
        var path = Path.GetTempFileName() + ".wav";
        try
        {
            WavWriter.Write(path, new double[] { 100.0, -100.0 });
            var bytes = File.ReadAllBytes(path);
            Assert.True(bytes.Length >= 48);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Render_produces_correct_sample_count()
    {
        var notes = new[]
        {
            new Mics.Core.Score.Note(440.0, 0.1, 0.7),
            new Mics.Core.Score.Note(523.25, 0.1, 0.7),
        };
        var samples = Renderer.Render(notes, sampleRate: 1000);
        Assert.Equal(200, samples.Length);
    }

    [Fact]
    public void Render_skips_invalid_notes()
    {
        var notes = new[]
        {
            new Mics.Core.Score.Note(0, 0.1, 0.7),
            new Mics.Core.Score.Note(440, -1, 0.7),
        };
        var samples = Renderer.Render(notes, sampleRate: 1000);
        Assert.Empty(samples);
    }
}
