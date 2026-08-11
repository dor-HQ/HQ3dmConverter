$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$Dist = Join-Path $Root "dist\HQ3dmConverter"

Remove-Item -Recurse -Force $Dist -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $Dist | Out-Null

dotnet restore "$Root\HQ3dmConverter.sln"
dotnet build "$Root\HQ3dmConverter.sln" -c Release --no-restore
dotnet test "$Root\HQ3dmConverter.sln" -c Release --no-build

dotnet publish "$Root\src\HQ3dmConverter.Cli\HQ3dmConverter.Cli.csproj" `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=false `
  -o "$Dist\cli"

dotnet publish "$Root\src\HQ3dmConverter.Wpf\HQ3dmConverter.Wpf.csproj" `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=false `
  -o "$Dist\wpf"

Write-Host "Published to $Dist"
Write-Host "  CLI:  $Dist\cli\HQ3dmConverter.Cli.exe"
Write-Host "  WPF:  $Dist\wpf\HQ3dmConverter.Wpf.exe"
