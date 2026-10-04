# Changelog

## [1.0.0] - 2026-10-04

### Added
- `Sound.ToSamples`, `Sound.ToWavBytes`, `Sound.Save` for rendering compositions built by the code analyzer.
- `Mics.CodeAnalysis` package with the Roslyn-based `Sonifier`.

### Changed
- `Mics.Core` no longer depends on Roslyn. It now contains only the audio engine,
  `Sound`, `Track`, and `SongBuilder`.
- `Sound.DefaultSampleRate` is the canonical sample rate constant.

### Removed
- Public access to implementation details: `Renderer`, `WavWriter`, `Oscillator`,
  `Envelope`, `CodeSonifier`. They are now `internal`.

## [0.5.0] - 2026-10-04

### Added
- `Mics.CodeAnalysis` package.

### Changed
- Split the sonifier out of `Mics.Core`.

## [0.4.0] - 2026-10-04

### Added
- Validation for BPM, beats, sample rate, NoteName MIDI range, frequency.
- Determinism and WAV structure tests.

### Fixed
- Voice context in `CodeSonifier` now uses a stack.
- Scale frequencies go through `NoteName`.

## [0.3.0] - 2026-10-04

### Added
- `Mics.Core` fluent sound library: `Sound`, `Track`, `SongBuilder`.

## [0.2.0] - 2026-10-04

### Added
- Polyphonic voices, instruments, scales, and CLI options.

## [0.1.0] - 2026-10-04

### Added
- Initial release.
