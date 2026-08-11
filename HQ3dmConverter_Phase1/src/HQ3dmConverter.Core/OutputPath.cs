namespace HQ3dmConverter.Core;

public static class OutputPath
{
    public static string GetUniqueRhino7Path(string inputPath)
    {
        var fullInput = Path.GetFullPath(inputPath);
        var directory = Path.GetDirectoryName(fullInput)
            ?? throw new ArgumentException("Input file has no parent directory.", nameof(inputPath));
        var stem = Path.GetFileNameWithoutExtension(fullInput);
        var candidate = Path.Combine(directory, $"{stem}_R7.3dm");
        if (!File.Exists(candidate)) return candidate;

        for (var i = 1; i <= 9999; i++)
        {
            candidate = Path.Combine(directory, $"{stem}_R7_{i:00}.3dm");
            if (!File.Exists(candidate)) return candidate;
        }

        throw new IOException("Could not create a unique Rhino 7 output filename.");
    }

    public static void EnsureDifferentFiles(string inputPath, string outputPath)
    {
        var a = Path.GetFullPath(inputPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var b = Path.GetFullPath(outputPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The output file cannot overwrite the source file.");
    }
}
