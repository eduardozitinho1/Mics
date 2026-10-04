namespace Mics;

/// <summary>
/// A named musical note such as "C4", "A#3" or "Bb5". Internally stores the MIDI note number.
/// </summary>
public readonly record struct NoteName(int MidiNumber)
{
    /// <summary>Frequency in hertz, using A4 = 440 Hz equal temperament.</summary>
    public double Frequency => 440.0 * Math.Pow(2, (MidiNumber - 69) / 12.0);

    /// <summary>Scientific pitch notation octave.</summary>
    public int Octave => (MidiNumber / 12) - 1;

    /// <summary>Creates a note from a MIDI number (0-127).</summary>
    public static NoteName FromMidi(int midi) => new(midi);

    /// <summary>Creates the closest note to a frequency in hertz.</summary>
    public static NoteName FromFrequency(double hz)
        => new((int)Math.Round(69 + 12 * Math.Log2(hz / 440.0)));

    /// <summary>Parses a note name like "C4", "A#3", "Bb5". Throws on invalid input.</summary>
    public static NoteName Parse(string text)
    {
        if (!TryParse(text, out var n))
            throw new FormatException($"Invalid note name: '{text}'");
        return n;
    }

    /// <summary>Attempts to parse a note name without throwing.</summary>
    public static bool TryParse(string text, out NoteName name)
    {
        name = default;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var s = text.Trim();
        int i = 0;

        int semitone = char.ToUpperInvariant(s[i]) switch
        {
            'C' => 0,
            'D' => 2,
            'E' => 4,
            'F' => 5,
            'G' => 7,
            'A' => 9,
            'B' => 11,
            _ => int.MinValue,
        };

        if (semitone == int.MinValue) return false;
        i++;

        while (i < s.Length && (s[i] == '#' || s[i] == 'b'))
        {
            semitone += s[i] == '#' ? 1 : -1;
            i++;
        }

        int start = i;
        while (i < s.Length && char.IsDigit(s[i])) i++;
        if (i == start || i != s.Length) return false;

        int octave = int.Parse(s.AsSpan(start, i - start));
        int midi = (octave + 1) * 12 + semitone;
        if (midi < 0 || midi > 127) return false;

        name = new NoteName(midi);
        return true;
    }

    private static readonly string[] Names =
    {
        "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B",
    };

    public override string ToString()
        => $"{Names[((MidiNumber % 12) + 12) % 12]}{Octave}";
}
