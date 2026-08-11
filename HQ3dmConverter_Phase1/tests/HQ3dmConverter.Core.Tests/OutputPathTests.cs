using HQ3dmConverter.Core;

namespace HQ3dmConverter.Core.Tests;

public class OutputPathTests
{
    [Fact]
    public void RejectsSourceOverwrite()
    {
        var path = Path.GetFullPath("Model.3dm");
        Assert.Throws<InvalidOperationException>(() => OutputPath.EnsureDifferentFiles(path, path));
    }

    [Fact]
    public void RejectsSourceOverwriteCaseInsensitively()
    {
        var a = Path.GetFullPath("Model.3dm");
        var b = Path.Combine(Path.GetDirectoryName(a)!, "MODEL.3DM");
        Assert.Throws<InvalidOperationException>(() => OutputPath.EnsureDifferentFiles(a, b));
    }
}
