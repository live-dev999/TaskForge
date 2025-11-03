# PowerShell script to run PostgreSQL container for API debugging
# This script starts only PostgreSQL container for local development/debugging

Write-Host "🐘 Starting PostgreSQL container for API debugging..." -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
try {
    docker info | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "❌ Docker is not running. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Run docker-compose with debug configuration
docker-compose -f docker-compose.debug.yml up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ PostgreSQL container started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Connection String:" -ForegroundColor Yellow
    Write-Host "  Host=localhost" -ForegroundColor Gray
    Write-Host "  Port=5432" -ForegroundColor Gray
    Write-Host "  Database=TaskForge" -ForegroundColor Gray
    Write-Host "  Username=postgres" -ForegroundColor Gray
    Write-Host "  Password=postgres" -ForegroundColor Gray
    Write-Host ""
    Write-Host "📝 To stop the container, run:" -ForegroundColor Cyan
    Write-Host "   docker-compose -f docker-compose.debug.yml down" -ForegroundColor Gray
    Write-Host ""
    Write-Host "📝 To view logs, run:" -ForegroundColor Cyan
    Write-Host "   docker-compose -f docker-compose.debug.yml logs -f postgres.debug" -ForegroundColor Gray
}
else {
    Write-Host ""
    Write-Host "❌ Failed to start PostgreSQL container" -ForegroundColor Red
    exit 1
}

