# Restore test projects with explicit NuGet source.
# Run from repository root: .\restore-tests.ps1

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$solution = Join-Path $root "src\FlaUIRecorder.sln"

Write-Host "Restoring solution packages from https://api.nuget.org/v3/index.json ..."
dotnet restore $solution --source https://api.nuget.org/v3/index.json

Write-Host "Building test projects ..."
dotnet build (Join-Path $root "src\Tests\FlaUIRecorder.CodeProvider.CSharp.Tests\FlaUIRecorder.CodeProvider.CSharp.Tests.csproj") --no-restore
dotnet build (Join-Path $root "src\Tests\FlaUIRecorder.CodeProvider.PowerShell.Tests\FlaUIRecorder.CodeProvider.PowerShell.Tests.csproj") --no-restore

Write-Host "Done."
