using HQ3dmConverter.Core;

static int Usage()
{
    Console.WriteLine("HQ 3DM Converter - Phase 1 technical spike");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  HQ3dmConverter.Cli inspect <file.3dm>");
    Console.WriteLine("  HQ3dmConverter.Cli convert <file.3dm> [--output <file_R7.3dm>]");
    return 3;
}

static void PrintSummary(ModelSummary s)
{
    Console.WriteLine($"3DM version: {s.ArchiveVersion}");
    Console.WriteLine($"Objects: {s.ObjectCount:N0}");
    Console.WriteLine($"Layers: {s.LayerCount:N0}");
    Console.WriteLine($"Materials: {s.MaterialCount:N0}");
    Console.WriteLine($"Block definitions: {s.InstanceDefinitionCount:N0}");
    Console.WriteLine($"Units: {s.UnitSystem}");
    Console.WriteLine($"Size: {s.FileSizeBytes:N0} bytes");
}

if (args.Length < 2) return Usage();

var command = args[0].ToLowerInvariant();
var input = args[1];

if (command == "inspect")
{
    try
    {
        var summary = new ThreeDmInspector().Inspect(input);
        Console.WriteLine($"Input: {summary.FilePath}");
        PrintSummary(summary);
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"ERROR: {ex.Message}");
        return 2;
    }
}

if (command != "convert") return Usage();

string? output = null;
for (var i = 2; i < args.Length; i++)
{
    if (args[i] == "--output" && i + 1 < args.Length)
    {
        output = args[++i];
    }
    else
    {
        return Usage();
    }
}

var result = new ThreeDmConverter().ConvertToRhino7(input, output);
Console.WriteLine($"Status: {result.Status}");

if (result.SourceSummary is not null)
{
    Console.WriteLine();
    Console.WriteLine("Source:");
    PrintSummary(result.SourceSummary);
}
if (result.OutputSummary is not null)
{
    Console.WriteLine();
    Console.WriteLine("Output:");
    Console.WriteLine(result.OutputPath);
    PrintSummary(result.OutputSummary);
}
if (result.Warnings.Count > 0)
{
    Console.WriteLine();
    foreach (var warning in result.Warnings) Console.WriteLine($"WARNING: {warning}");
}
if (result.Errors.Count > 0)
{
    Console.WriteLine();
    foreach (var error in result.Errors) Console.Error.WriteLine($"ERROR: {error}");
}

return result.Status switch
{
    ConversionStatus.Success => 0,
    ConversionStatus.SuccessWithWarnings => 1,
    ConversionStatus.Skipped => 0,
    _ => 2
};
