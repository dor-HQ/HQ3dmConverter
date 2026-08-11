# Phase 1 research notes

Checked against McNeel sources in August 2026.

Confirmed API surface in Rhino3dm 8.x source:

- `Rhino.FileIO.File3dm.Read(string path)`
- `Rhino.FileIO.File3dm.ReadWithLog(string path, out string errorLog)`
- `File3dm.ReadArchiveVersion(string path)`
- `File3dm.ArchiveVersion`
- `File3dm.Write(string path, int version)`
- `File3dm.Write(string path, File3dmWriteOptions options)`
- `File3dm.WriteWithLog(string path, File3dmWriteOptions options, out string errorLog)`
- write options pass `SaveUserData` to the native openNURBS writer
- `File3dm` implements `IDisposable`

Selected package: `Rhino3dm` 8.32.0 (stable NuGet version visible during research).

## Runtime premise

McNeel describes rhino3dm as an OpenNURBS-based library for .NET/Python/JavaScript applications independent of Rhino; OpenNURBS itself is intended to read/write 3DM without Rhino.

## Not yet proven in this environment

- NuGet restore
- C# compilation
- native Rhino3dm loading
- conversion of an actual Rhino 8 file
- Rhino 7 opening the output

Reason: the current execution environment has no .NET SDK and cannot download the SDK/NuGet packages from the shell.
