# HQ 3DM Converter — CLI

Standalone command-line tool to convert Rhino 8+ `.3dm` files to Rhino 7 format.  
No Rhino installation required — uses McNeel's `Rhino3dm` (OpenNURBS) library.

## Usage

```powershell
# Inspect a 3DM file (shows version, object counts, units)
HQ3dmConverter.Cli.exe inspect "path\to\model.3dm"

# Convert Rhino 8+ → Rhino 7 (auto-names output as model_R7.3dm)
HQ3dmConverter.Cli.exe convert "path\to\model.3dm"

# Convert with explicit output path
HQ3dmConverter.Cli.exe convert "path\to\model.3dm" --output "path\to\model_R7.3dm"
```

## Exit Codes

| Code | Meaning |
|------|---------|
| 0    | Success (or skipped — already Rhino 7 compatible) |
| 1    | Success with warnings (data loss possible) |
| 2    | Error (file not found, invalid format, conversion failed) |
| 3    | Usage error (invalid arguments) |

## Warnings (Normal)

- `Some Rhino 8 features may not be representable in Rhino 7.`
- `Third-party plug-in data compatibility cannot be guaranteed.`
- `Object count changed: X → Y` (geometry not supported in Rhino 7)

## Requirements

- Windows 10/11 (x64)
- No .NET installation needed (self-contained)

## Source

https://github.com/dor-HQ/HQ3dmConverter