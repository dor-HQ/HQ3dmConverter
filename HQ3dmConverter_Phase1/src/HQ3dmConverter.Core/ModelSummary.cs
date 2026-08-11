namespace HQ3dmConverter.Core;

public sealed record ModelSummary(
    string FilePath,
    long FileSizeBytes,
    int ArchiveVersion,
    int ObjectCount,
    int LayerCount,
    int MaterialCount,
    int InstanceDefinitionCount,
    string UnitSystem);
