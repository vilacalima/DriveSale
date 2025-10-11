param(
  [switch]$Recreate
)

Write-Host "Starting Docker Compose stack (Postgres + Adminer)..." -ForegroundColor Cyan

$cmd = "docker compose up -d"
if ($Recreate) { $cmd = "docker compose up -d --force-recreate --remove-orphans" }

Invoke-Expression $cmd

Write-Host "\nServices:" -ForegroundColor Green
Write-Host "- Postgres: localhost:5432 (db=fase2, user=postgres, pass=postgres)"
Write-Host "- Adminer:  http://localhost:8081 (System: PostgreSQL, Server: postgres, User: postgres, Password: postgres, Database: fase2)"

Write-Host "\nTip: set ConnectionStrings__Default to 'Host=localhost;Port=5432;Database=fase2;Username=postgres;Password=postgres'" -ForegroundColor Yellow

