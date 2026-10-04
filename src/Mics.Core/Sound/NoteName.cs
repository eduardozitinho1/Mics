namespace Mics;

/// <summary>
/// A named musical note such as "C4", "A#3" or "Bb5". Internally stores the MIDI note number.
/// </summary>
/// <remarks>
/// The MIDI standard defines note numbers from 0 (C-1) to 127 (G9). Every constructor and
/// parser in this type enforces that range. Enharmonic spellings normalize: "Bb3" equals
/// "A#3" equals MIDI 58.
/// </remarks>
public readonly record struct NoteName
{
    /// <summary>Lowest valid MIDI note number (C-1).</summary>
    public const int MinMidi = 0;

    /// <summary>Highest valid MIDI note number (G9).</summary>
    public const int MaxMidi = 127;

    /// <summary>MIDI note number in [0, 127].</summary>
    public int MidiNumber { get; }

    /// <summary>Creates a note from a MIDI number, validating the range.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="midi"/> is outside [0, 127].</exception>
    public NoteName(int midi)
    {
        if (midi < MinMidi || midi > MaxMidi)
            throw new ArgumentOutOfRangeException(nameof(midi), midi, $"MIDI note must be between {MinMidi} and {MaxMidi}.");

        MidiNumber = midi;
    }

    /// <summary>Frequency in hertz, using A4 = 440 Hz equal temperament.</summary>
    public double Frequency => 440.0 * Math.Pow(2, (MidiNumber - 69) / 12.0);

    /// <summary>Scientific pitch notation octave.</summary>
    public int Octave => (MidiNumber / 12) - 1;

    /// <summary>Creates a note from a MIDI number (0-127).</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="midi"/> is outside [0, 127].</exception>
    public static NoteName FromMidi(int midi) => new(midi);

    /// <summary>Attempts to create a note from a MIDI number without throwing.</summary>
    public static bool TryFromMidi(int midi, out NoteName name)
    {
        if (midi < MinMidi || midi > MaxMidi)
        {
            name = default;
            return false;
        }
        name = new NoteName(midi);
        return true;
    }

    /// <summary>
    /// Creates the closest note to a frequency in hertz.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="hz"/> is not a positive, finite number or produces a MIDI note outside [0, 127].</exception>
    public static NoteName FromFrequency(double hz)
    {
        if (!double.IsFinite(hz) || hz <= 0)
            throw new ArgumentOutOfRangeException(nameof(hz), hz, "Frequency must be a positive, finite number.");

        var midi = (int)Math.Round(69 + 12 * Math.Log2(hz / 440.0));
        if (midi < MinMidi || midi > MaxMidi)
            throw new ArgumentOutOfRangeException(nameof(hz), hz, $"Frequency maps to MIDI {midi}, outside [{MinMidi}, {MaxMidi}].");

        return new NoteName(midi);
    }

    /// <summary>Attempts to create a note from a frequency without throwing.</summary>
    public static bool TryFromFrequency(double hz, out NoteName name)
    {
        name = default;
        if (!double.IsFinite(hz) || hz <= 0)
            return false;

        var midi = (int)Math.Round(69 + 12 * Math.Log2(hz / 440.0));
        if (midi < MinMidi || midi > MaxMidi)
            return false;

        name = new NoteName(midi);
        return true;
    }

    /// <summary>Parses a note name like "C4", "A#3", "Bb5". Throws on invalid input.</summary>
    /// <exception cref="FormatException">Thrown when <paramref name="text"/> is not a valid note name.</exception>
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

        if (!int.TryParse(s.AsSpan(start, i - start), out var octave))
            return false;

        var midi = (octave + 1) * 12 + semitone;
        if (midi < MinMidi || midi > MaxMidi) return false;

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
