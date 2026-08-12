# HQ 3DM Converter

Standalone Rhino 8/newer → Rhino 7 `.3dm` conversion tool using McNeel Rhino3dm/openNURBS.  
Does **not** require Rhino installation or license.

## Features

- **CLI** — `HQ3dmConverter.Cli.exe` for scripting/automation
- **WPF GUI** — `HQ3dmConverter.Wpf.exe` with modern drag-and-drop interface
- **Self-contained** — Runs on Windows 10/11 x64 without .NET installation
- **Verified conversion** — Reads → writes temp → re-reads → verifies v70 → atomic move
- **Safe** — Never overwrites source; auto-generates unique `_R7.3dm` names

## Download

| Platform | Artifact |
|----------|----------|
| Windows x64 (CLI) | [HQ3dmConverter-CLI-v1.0.0.zip](https://github.com/HQ-Architects/HQ3dmConverter/releases/download/v1.0.0/HQ3dmConverter-CLI-v1.0.0.zip) |
| Windows x64 (WPF) | [HQ3dmConverter-WPF-v1.0.0.zip](https://github.com/HQ-Architects/HQ3dmConverter/releases/download/v1.0.0/HQ3dmConverter-WPF-v1.0.0.zip) |

*Also available from [dor-HQ/HQ3dmConverter](https://github.com/dor-HQ/HQ3dmConverter/releases/tag/v1.0.0)*

## Usage

### CLI

```powershell
# Inspect a 3DM file
HQ3dmConverter.Cli.exe inspect "path\to\model.3dm"

# Convert Rhino 8+ → Rhino 7 (auto-names output as model_R7.3dm)
HQ3dmConverter.Cli.exe convert "path\to\model.3dm"

# Convert with explicit output path
HQ3dmConverter.Cli.exe convert "path\to\model.3dm" --output "path\to\model_R7.3dm"
```

### WPF GUI

1. Double-click `HQ3dmConverter.Wpf.exe`
2. Click **Browse...** to select a Rhino 8+ `.3dm` file
3. (Optional) Choose custom output location
4. Click **Convert** — results show version, object counts, layers before/after

## Build from Source

```powershell
# Prerequisites: .NET 7 SDK
dotnet restore HQ3dmConverter.sln
dotnet build HQ3dmConverter.sln -c Release
dotnet test HQ3dmConverter.sln -c Release

# Publish self-contained (CLI + WPF)
./build.ps1
# Output: dist/HQ3dmConverter/cli/  and  dist/HQ3dmConverter/wpf/
```

## Dependencies

- .NET 7 (target framework)
- `Rhino3dm` NuGet 8.32.0 (OpenNURBS-based, no Rhino required)

## Limitations

Saving a newer 3DM as Rhino 7 **can discard data** that the Rhino 7 file format cannot represent. Third-party plug-in userdata cannot be guaranteed. The tool reports conversion success with warnings rather than claiming model identity.

## Architecture

```
src/
├── HQ3dmConverter.Core/      # Core library (net7.0)
│   ├── ThreeDmConverter.cs   # Read → summarize → write v70 → verify → move
│   ├── ThreeDmInspector.cs   # Read-only model summary
│   ├── ModelSummary.cs       # Immutable record
│   ├── ConversionResult.cs   # Status, warnings, errors, logs
│   └── OutputPath.cs         # Unique naming + overwrite protection
├── HQ3dmConverter.Cli/       # CLI app (net7.0)
└── HQ3dmConverter.Wpf/       # WPF GUI (net7.0-windows)

tests/
└── HQ3dmConverter.Core.Tests/ # Unit tests (xUnit)
```

## CI/CD

- **GitHub Actions**: Build + test on every push/PR (`ci.yml`)
- **Release**: Tag `v*` triggers publish + ZIP artifacts (`release.yml`)

## License

MIT