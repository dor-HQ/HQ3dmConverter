using Rhino.FileIO;

namespace HQ3dmConverter.Core;

public sealed class ThreeDmInspector
{
    public ModelSummary Inspect(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath)) throw new FileNotFoundException("3DM file not found.", fullPath);

        using var model = File3dm.Read(fullPath)
            ?? throw new InvalidDataException("Rhino3dm could not read the 3DM file.");

        return Summarize(model, fullPath);
    }

    internal static ModelSummary Summarize(File3dm model, string path)
    {
        return new ModelSummary(
            FilePath: path,
            FileSizeBytes: new FileInfo(path).Length,
            ArchiveVersion: model.ArchiveVersion,
            ObjectCount: model.Objects.Count,
            LayerCount: model.AllLayers.Count,
            MaterialCount: model.AllMaterials.Count,
            InstanceDefinitionCount: model.AllInstanceDefinitions.Count,
            UnitSystem: model.Settings.ModelUnitSystem.ToString());
    }
}
