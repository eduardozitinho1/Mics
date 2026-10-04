# Mics

Music In C Sharp. Turn C# source code into music, or use the fluent API to compose sound from your own code.

## Install

CLI tool:

```bash
dotnet tool install -g Mics.Cli
```

Library:

```bash
dotnet add package Mics.Core
```

## CLI

```bash
mics compose <input.cs> -o <output.wav> [options]
mics scales
mics instruments
```

Options:

- `-o, --output <file>` — output WAV file (required)
- `--scale <name>` — major, minor, dorian, mixolydian, pentatonic, blues
- `--tempo <bpm>` — beats per minute (default: 120)
- `--instrument <name>` — sine, triangle, square, sawtooth
- `-v, --version` — show version and exit

Examples:

```bash
mics compose samples/Hello.cs -o hello.wav
mics compose samples/Hello.cs -o hello-minor.wav --scale minor --tempo 90
```

## Library

The `Sound` class is the entry point. Every method returns a `Track` you can chain.

```csharp
using Mics;

Sound.Note("C4").Save("note.wav");
Sound.Melody("C4", "E4", "G4").Save("melody.wav");
Sound.Chord("C4", "E4", "G4").Save("chord.wav");
```

Chaining with tempo and instrument:

```csharp
Sound.Note("C4", beats: 0.5)
    .Note("E4", beats: 0.5)
    .Rest(1)
    .Note("G4")
    .Bpm(100)
    .Instrument("triangle")
    .Save("phrase.wav");
```

Multi-voice song:

```csharp
Sound.Song()
    .Tempo(100)
    .Voice("lead", "triangle", v => v
        .Note("C4").Note("E4").Note("G4").Note("C5"))
    .Voice("bass", "sawtooth", v => v
        .Note("C2", beats: 4))
    .Save("song.wav");
```

In-memory output:

```csharp
byte[] wav = Sound.Note("A4").ToWavBytes();
double[] pcm = Sound.Note("A4").ToSamples();
```

## Note names

Scientific pitch notation. Enharmonic equivalents normalize.

- `C4`, `A#3`, `Bb5`, `Db2`
- Case-insensitive: `c4` equals `C4`
- `Bb3` equals `A#3`

MIDI numbers 0-127 are the canonical internal representation, so `NoteName.FromMidi(60)` returns middle C.

## Instruments

Four built-in waveforms, each with its own ADSR envelope:

- `sine` — pure tone, gentle attack
- `triangle` — softer harmonics, flute-like
- `square` — hollow, retro
- `sawtooth` — bright, buzzy

## Scales

- `major` — bright, resolved
- `minor` — melancholic
- `dorian` — jazzy
- `mixolydian` — bluesy rock
- `pentatonic` — safe, pleasant
- `blues` — gritty

## How it works

CLI mode parses C# with Roslyn, walks the syntax tree, and maps each node to a musical event. Each type declaration (class, struct, record, interface, enum) becomes a voice with its own timeline; voices play in parallel.

Library mode skips the parser entirely. You write the notes. The library handles timing, instruments, mixing, and WAV synthesis.

## License

MIT
