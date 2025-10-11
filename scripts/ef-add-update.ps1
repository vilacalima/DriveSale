param(
  [Parameter(Mandatory=$false)][string]$Name = "InitialCreate",
  [switch]$InstallTool
)

Write-Host "EF Core: add migration '$Name' and update database" -ForegroundColor Cyan

if ($InstallTool) {
  Write-Host "Installing dotnet-ef tool (global) if needed..." -ForegroundColor Yellow
  try {
    dotnet tool install --global dotnet-ef | Out-Null
  } catch {
    Write-Host "dotnet-ef may already be installed. Continuing..." -ForegroundColor DarkYellow
  }
}

$infra = Join-Path $PSScriptRoot "..\src\Infrastructure"
$webapi = Join-Path $PSScriptRoot "..\src\WebApi"

if (!(Test-Path $infra) -or !(Test-Path $webapi)) {
  Write-Error "Could not locate project paths. Expected src/Infrastructure and src/WebApi relative to scripts/."
  exit 1
}

Write-Host "Adding migration..." -ForegroundColor Green
dotnet ef migrations add $Name -p "$infra" -s "$webapi"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Updating database..." -ForegroundColor Green
dotnet ef database update -p "$infra" -s "$webapi"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Done." -ForegroundColor Green

