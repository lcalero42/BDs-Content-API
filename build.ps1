$config = Get-Content "config.json" | ConvertFrom-Json

$sourceFolder = $config.SourceFolder
$destinationZip = $config.DestinationZip
$buildDllPath = $config.BuildDllPath

dotnet build

# Check if source exists
if (!(Test-Path $sourceFolder)) { 
    Write-Error "Source folder not found."; exit 
}

# Temporary copies
$buildTempFileDest = Join-Path $sourceFolder (Split-Path $buildDllPath -Leaf)
Copy-Item $buildDllPath $buildTempFileDest -Force

# Create archive
Compress-Archive -Path "$sourceFolder\*" -DestinationPath $destinationZip -Force

# Clean up
Remove-Item $buildTempFileDest -Force