# HQ 3DM Converter — WPF GUI

Standalone Windows desktop application to convert Rhino 8+ `.3dm` files to Rhino 7 format.  
No Rhino installation required — uses McNeel's `Rhino3dm` (OpenNURBS) library.

## Usage

1. Double-click `HQ3dmConverter.Wpf.exe`
2. Click **Browse...** next to "Input File" and select a Rhino 8+ `.3dm` file
3. (Optional) Click **Browse...** next to "Output File" to choose a custom output location
   - If left empty, auto-generates `filename_R7.3dm` in the same folder
4. Click **Convert**
5. Results appear in the output panel — shows version, object counts, layers before/after

## Features

- Modern, clean UI with drag-and-drop ready file pickers
- Progress indicator during conversion
- Color-coded results:
  - 🟢 Green = Success
  - 🟡 Yellow = Success with warnings
  - 🔴 Red = Error
- Shows archive version (80→70), object count, layer count, units
- Preserves materials, blocks, layers where Rhino 7 supports them

## Warnings (Normal)

- `Some Rhino 8 features may not be representable in Rhino 7.`
- `Third-party plug-in data compatibility cannot be guaranteed.`
- Object/layer/block count changes reported if geometry is lost

## Requirements

- Windows 10/11 (x64)
- No .NET installation needed (self-contained, ~130MB folder)

## Source

https://github.com/dor-HQ/HQ3dmConverter