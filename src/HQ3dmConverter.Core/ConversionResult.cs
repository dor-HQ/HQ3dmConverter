namespace HQ3dmConverter.Core;

public enum ConversionStatus
{
    Success,
    SuccessWithWarnings,
    Failed,
    Skipped
}

public sealed record ConversionResult(
    ConversionStatus Status,
    string InputPath,
    string? OutputPath,
    ModelSummary? SourceSummary,
    ModelSummary? OutputSummary,
    IReadOnlyList<string> Warnings,
    IReadOnlyList<string> Errors,
    string? RhinoReadLog = null,
    string? RhinoWriteLog = null);
