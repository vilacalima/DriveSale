Write-Host "Stopping and removing Docker Compose stack..." -ForegroundColor Cyan
docker compose down -v
Write-Host "Done." -ForegroundColor Green

