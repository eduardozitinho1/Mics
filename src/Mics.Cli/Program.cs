using Mics.Core.Audio;
using Mics.Core.Mapping;

if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
{
    PrintHelp();
    return 0;
}

if (args[0] != "compose")
{
    Console.Error.WriteLine($"Unknown command: {args[0]}");
    PrintHelp();
    return 1;
}

if (args.Length < 4 || args[2] != "-o")
{
    Console.Error.WriteLine("Usage: mics compose <input.cs> -o <output.wav>");
    return 1;
}

var input = args[1];
var output = args[3];

if (!File.Exists(input))
{
    Console.Error.WriteLine($"File not found: {input}");
    return 1;
}

Console.WriteLine($"mics v0.1.0");
Console.WriteLine($"Input:  {input}");

var notes = Sonifier.SonifyFile(input);
Console.WriteLine($"Notes:  {notes.Count}");

if (notes.Count == 0)
{
    Console.Error.WriteLine("No notes produced. The file may have no class or method declarations.");
    return 2;
}

var samples = Renderer.Render(notes);
var duration = (double)samples.Length / Renderer.DefaultSampleRate;
Console.WriteLine($"Length: {duration:F1}s");
Console.WriteLine($"Output: {output}");

WavWriter.Write(output, samples);
Console.WriteLine("Done.");
return 0;

static void PrintHelp()
{
    Console.WriteLine("mics - Music In C Sharp");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  mics compose <input.cs> -o <output.wav>");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -h, --help    Show this help");
}
