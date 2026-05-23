param(
    [switch]$Serve
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

Write-Host "Building DbsContentApi (Release)..."
dotnet build DbsContentApi.csproj -c Release

Write-Host "Extracting API metadata..."
dotnet docfx metadata docfx.json

Copy-Item -Force "apidoc\toc.yml" "api\toc.yml"

Write-Host "Building documentation site..."
dotnet docfx build docfx.json

Write-Host "Documentation generated in _site/"

if ($Serve) {
    Write-Host "Serving at http://localhost:8080 ..."
    dotnet docfx serve _site
}
