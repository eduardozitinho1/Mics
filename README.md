# Mics

Music In C Sharp. Turn C# source code into music.

## Install

    dotnet tool install -g Mics.Cli

## Usage

    mics compose <input.cs> -o <output.wav> [options]
    mics scales
    mics instruments

## Options

- `-o, --output <file>` — output WAV file (required)
- `--scale <name>` — musical scale (default: major)
- `--tempo <bpm>` — tempo in BPM (default: 120)
- `--instrument <name>` — force a single instrument for all voices

## Scales

major, minor, dorian, mixolydian, pentatonic, blues

## Instruments

sine, triangle, square, sawtooth

## How it works

Mics parses C# with Roslyn, walks the syntax tree, and maps each
node to musical events. Each class becomes a voice with its own
timeline. Voices play in parallel, producing polyphony.

- `class` / `struct` / `record` → a new voice
- `method` → the opening phrase of a section
- `if` / `else` → tension and resolution
- `for` / `while` → a repeated motif
- `try` / `catch` → rise and fall
- `return` → tonic resolution
- `throw` → a sharp descending interval
- numeric literals → notes based on their value
- string literals → notes based on length
- nesting depth → octave

## License

MIT
