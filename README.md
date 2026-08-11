# HQ 3DM Converter — Phase 1

Standalone Rhino 8/newer -> Rhino 7 `.3dm` conversion proof of concept using McNeel Rhino3dm/openNURBS. It does **not** use or launch Rhino.

## Current scope

Phase 1 only: CLI + conversion core. No WPF GUI yet.

The converter:

1. reads the source with `Rhino.FileIO.File3dm.ReadWithLog`;
2. records a basic model summary;
3. writes a temporary file using `File3dmWriteOptions { Version = 7, SaveUserData = true }`;
4. reopens the temporary output;
5. verifies archive version 7 and basic counts;
6. moves the verified temporary file to a unique `_R7.3dm` name.

The original source is never intentionally overwritten.

## Dependency

- .NET 7
- `Rhino3dm` NuGet package `8.32.0`

Rhino3dm is designed to work independently of Rhino. No Rhino 8 installation or Rhino license is required by this project.

## Build

```powershell
dotnet restore HQ3dmConverter.sln
dotnet build HQ3dmConverter.sln -c Release
dotnet test HQ3dmConverter.sln -c Release
```

## Run

```powershell
dotnet run --project src/HQ3dmConverter.Cli -- inspect "C:\Models\Model.3dm"
```

```powershell
dotnet run --project src/HQ3dmConverter.Cli -- convert "C:\Models\Model_R8.3dm"
```

Explicit output:

```powershell
dotnet run --project src/HQ3dmConverter.Cli -- convert "C:\Models\Model_R8.3dm" --output "C:\Models\Model_R7.3dm"
```

## Publish Windows x64 self-contained

```powershell
./build.ps1
```

or:

```powershell
dotnet publish src/HQ3dmConverter.Cli/HQ3dmConverter.Cli.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o dist/HQ3dmConverter/cli
```

## Important limitation

Saving a newer 3DM as Rhino 7 can discard data that the Rhino 7 file format cannot represent. Third-party plug-in userdata cannot be guaranteed. This tool deliberately reports conversion success rather than claiming model identity.

## Verification status in this package

The source was prepared against McNeel's current Rhino3dm API documentation/source. The execution environment used to prepare this Phase-1 package does not contain a .NET SDK and cannot reach NuGet, so the package has **not yet been compiled here**. The first action on a Windows development machine should therefore be `dotnet restore`, `dotnet build`, and then a real Rhino 8 test-file conversion.
