using System.Globalization;
using System.Reflection;
using Mics.Core.Audio;
using Mics.Core.Mapping;
using Mics.Core.Score;

var version = typeof(Program).Assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
    .InformationalVersion
    ?? typeof(Program).Assembly.GetName().Version?.ToString()
    ?? "unknown";

if (args.Length > 0 && args[0] is "--version" or "-v")
{
    Console.WriteLine(version);
    return 0;
}

if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
{
    PrintHelp(version);
    return 0;
}

var command = args[0];

if (command == "scales")
{
    Console.WriteLine("Available scales:");
    foreach (var s in Scales.All)
        Console.WriteLine($"  {s.Name}");
    return 0;
}

if (command == "instruments")
{
    Console.WriteLine("Available instruments:");
    foreach (var i in Instruments.All)
        Console.WriteLine($"  {i.Name}");
    return 0;
}

if (command != "compose")
{
    Console.Error.WriteLine($"Unknown command: {command}");
    PrintHelp(version);
    return 1;
}

var options = new SonifierOptions();
string? input = null;
string? output = null;

for (int i = 1; i < args.Length; i++)
{
    var arg = args[i];
    switch (arg)
    {
        case "-o":
        case "--output":
            if (i + 1 >= args.Length) { Console.Error.WriteLine($"{arg} requires a value"); return 1; }
            output = args[++i];
            break;
        case "--scale":
            if (i + 1 >= args.Length) { Console.Error.WriteLine("--scale requires a value"); return 1; }
            var scale = Scales.ByName(args[++i]);
            if (scale is null) { Console.Error.WriteLine("Unknown scale. Try 'mics scales'."); return 1; }
            options = options with { Scale = scale };
            break;
        case "--tempo":
            if (i + 1 >= args.Length) { Console.Error.WriteLine("--tempo requires a value"); return 1; }
            if (!double.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var tempo) || !double.IsFinite(tempo) || tempo <= 0)
            {
                Console.Error.WriteLine("Tempo must be a positive number (use '.' as decimal separator).");
                return 1;
            }
            options = options with { TempoBpm = tempo };
            i++;
            break;
        case "--instrument":
            if (i + 1 >= args.Length) { Console.Error.WriteLine("--instrument requires a value"); return 1; }
            var inst = Instruments.ByName(args[++i]);
            if (inst is null) { Console.Error.WriteLine("Unknown instrument. Try 'mics instruments'."); return 1; }
            options = options with { Instrument = inst };
            break;
        default:
            if (arg.StartsWith('-')) { Console.Error.WriteLine($"Unknown option: {arg}"); return 1; }
            if (input is null) input = arg;
            else { Console.Error.WriteLine($"Unexpected argument: {arg}"); return 1; }
            break;
    }
}

if (input is null) { Console.Error.WriteLine("Missing input file"); PrintHelp(version); return 1; }
if (output is null) { Console.Error.WriteLine("Missing -o <output>"); PrintHelp(version); return 1; }
if (!File.Exists(input)) { Console.Error.WriteLine($"File not found: {input}"); return 1; }

Console.WriteLine($"mics v{version}");
Console.WriteLine($"Input:       {input}");
Console.WriteLine($"Scale:       {options.Scale.Name}");
Console.WriteLine($"Tempo:       {options.TempoBpm} BPM");
Console.WriteLine($"Instrument:  {options.Instrument?.Name ?? "auto"}");

Composition composition;
try
{
    composition = Sonifier.SonifyFile(input, options);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to sonify: {ex.Message}");
    return 2;
}

var totalNotes = 0;
foreach (var v in composition.Voices) totalNotes += v.Notes.Count;

Console.WriteLine($"Voices:      {composition.Voices.Count}");
Console.WriteLine($"Notes:       {totalNotes}");

if (composition.Voices.Count == 0 || totalNotes == 0)
{
    Console.Error.WriteLine("No notes produced.");
    return 2;
}

var samples = Renderer.Render(composition);
var duration = (double)samples.Length / Renderer.DefaultSampleRate;
Console.WriteLine($"Length:      {duration:F1}s");
Console.WriteLine($"Output:      {output}");

WavWriter.Write(output, samples);
Console.WriteLine("Done.");
return 0;

static void PrintHelp(string version)
{
    Console.WriteLine($"mics - Music In C Sharp (v{version})");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  mics compose <input.cs> -o <output.wav> [options]");
    Console.WriteLine("  mics scales          List available scales");
    Console.WriteLine("  mics instruments     List available instruments");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -o, --output <file>     Output WAV file (required)");
    Console.WriteLine("  --scale <name>          Musical scale (default: major)");
    Console.WriteLine("  --tempo <bpm>           Tempo in BPM (default: 120)");
    Console.WriteLine("  --instrument <name>     Force a single instrument for all voices");
    Console.WriteLine("  -v, --version           Show version and exit");
    Console.WriteLine("  -h, --help              Show this help");
}
