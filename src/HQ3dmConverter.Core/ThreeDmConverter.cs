using Rhino.FileIO;

namespace HQ3dmConverter.Core;

public sealed class ThreeDmConverter
{
    public const int TargetArchiveVersion = 7;

    public ConversionResult ConvertToRhino7(string inputPath, string? outputPath = null)
    {
        var warnings = new List<string>();
        var errors = new List<string>();
        string? readLog = null;
        string? writeLog = null;
        string? finalOutput = null;
        string? tempOutput = null;
        ModelSummary? sourceSummary = null;
        ModelSummary? outputSummary = null;

        try
        {
            var fullInput = Path.GetFullPath(inputPath);
            if (!File.Exists(fullInput))
                throw new FileNotFoundException("3DM file not found.", fullInput);
            if (!string.Equals(Path.GetExtension(fullInput), ".3dm", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Input must be a .3dm file.", nameof(inputPath));

            finalOutput = outputPath is null ? OutputPath.GetUniqueRhino7Path(fullInput) : Path.GetFullPath(outputPath);
            OutputPath.EnsureDifferentFiles(fullInput, finalOutput);
            if (File.Exists(finalOutput))
                throw new IOException($"Output file already exists: {finalOutput}");

            using var source = File3dm.ReadWithLog(fullInput, out readLog)
                ?? throw new InvalidDataException("Rhino3dm could not read the source file.");

            sourceSummary = ThreeDmInspector.Summarize(source, fullInput);
            if (sourceSummary.ArchiveVersion <= TargetArchiveVersion)
            {
                return new ConversionResult(
                    ConversionStatus.Skipped, fullInput, null, sourceSummary, null,
                    new[] { $"File is already Rhino {sourceSummary.ArchiveVersion} compatible." },
                    Array.Empty<string>(), readLog, null);
            }

            warnings.Add("Some Rhino 8 features may not be representable in Rhino 7.");
            warnings.Add("Third-party plug-in data compatibility cannot be guaranteed.");

            tempOutput = finalOutput + ".__converting";
            if (File.Exists(tempOutput)) File.Delete(tempOutput);

            var options = new File3dmWriteOptions
            {
                Version = TargetArchiveVersion,
                SaveUserData = true
            };

            if (!source.WriteWithLog(tempOutput, options, out writeLog))
                throw new IOException("Rhino3dm reported that writing the Rhino 7 archive failed.");

            if (!File.Exists(tempOutput) || new FileInfo(tempOutput).Length == 0)
                throw new IOException("Conversion produced an empty or missing temporary output file.");

            using (var reopened = File3dm.ReadWithLog(tempOutput, out var reopenLog))
            {
                if (reopened is null)
                    throw new InvalidDataException($"Converted file could not be reopened. {reopenLog}".Trim());

                outputSummary = ThreeDmInspector.Summarize(reopened, tempOutput);
                if (outputSummary.ArchiveVersion != TargetArchiveVersion)
                    throw new InvalidDataException($"Expected Rhino 7 archive, got version {outputSummary.ArchiveVersion}.");

                if (sourceSummary.ObjectCount != outputSummary.ObjectCount)
                    warnings.Add($"Object count changed: {sourceSummary.ObjectCount} -> {outputSummary.ObjectCount}.");
                if (sourceSummary.LayerCount != outputSummary.LayerCount)
                    warnings.Add($"Layer count changed: {sourceSummary.LayerCount} -> {outputSummary.LayerCount}.");
                if (sourceSummary.InstanceDefinitionCount != outputSummary.InstanceDefinitionCount)
                    warnings.Add($"Block definition count changed: {sourceSummary.InstanceDefinitionCount} -> {outputSummary.InstanceDefinitionCount}.");
            }

            File.Move(tempOutput, finalOutput);
            tempOutput = null;

            outputSummary = outputSummary! with
            {
                FilePath = finalOutput,
                FileSizeBytes = new FileInfo(finalOutput).Length
            };

            var status = warnings.Count > 2 ? ConversionStatus.SuccessWithWarnings : ConversionStatus.Success;
            return new ConversionResult(status, fullInput, finalOutput, sourceSummary, outputSummary, warnings, errors, readLog, writeLog);
        }
        catch (Exception ex)
        {
            errors.Add(ex.Message);
            return new ConversionResult(ConversionStatus.Failed, inputPath, finalOutput, sourceSummary, outputSummary, warnings, errors, readLog, writeLog);
        }
        finally
        {
            if (tempOutput is not null && File.Exists(tempOutput))
            {
                try { File.Delete(tempOutput); } catch { /* Phase 2: structured cleanup logging */ }
            }
        }
    }
}
