param (
  [switch]$clean = $false
)

$ErrorActionPreference = "Stop"

Set-Location -Path $PSScriptRoot

Write-Host "Running build.ps1 for Schuub.Delphi11.Native"

if ($clean) {
  Write-Host "Removing bin folder..."
  Remove-Item "bin" -Recurse -ErrorAction Ignore
  exit 0
}

if (-not (Test-Path -Path "bin")) {
  Write-Host "Creating bin folder..."
  $unused = New-Item -ItemType Directory -Force -Path "bin"
}

if (-not (Test-Path -Path "tcc\tcc.exe")) {
  Write-Host "Extracting tcc..."
  Expand-Archive -LiteralPath "tcc-0.9.27-win32-bin.zip" -DestinationPath "." -Force
  if (-not (Test-Path -Path "tcc\tcc.exe")) {
    throw "tried to expand TCC files, but they didn't end up in the right spot"
  }
}

Write-Host "Compiling Schuub.Delphi11.Native.x86.dll..."
& tcc\tcc.exe -g -m32 -shared -o bin\Schuub.Delphi11.Native.x86.dll *.c
if (-not $?) { exit 1 }

Write-Host "Compiling Schuub.Delphi11.Native.x64.dll..."
& tcc\tcc.exe -g -m64 -shared -o bin\Schuub.Delphi11.Native.x64.dll *.c
if (-not $?) { exit 1 }

Write-Host "Done"
exit 0