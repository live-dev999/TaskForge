# Universal docker-compose startup script for Windows PowerShell
# Automatically detects platform and runs docker-compose

param(
    [Parameter(ValueFromRemainingArguments=$true)]
    [string[]]$DockerComposeArgs
)

Write-Host "🐳 Docker Compose - Auto Platform Detection" -ForegroundColor Cyan
Write-Host ""

# Detect architecture
$Arch = $env:PROCESSOR_ARCHITECTURE
$ProcArch = (Get-WmiObject Win32_Processor -ErrorAction SilentlyContinue).Architecture

# Check for ARM64
if ($Arch -eq "ARM64" -or $env:PROCESSOR_ARCHITEW6432 -eq "ARM64") {
    $env:PLATFORM = "linux/arm64"
    $env:SQL_IMAGE = "mcr.microsoft.com/azure-sql-edge:latest"
    Write-Host "✅ Detected ARM64 platform (Windows on ARM)" -ForegroundColor Green
}
# Check for x86_64/AMD64
elseif ($Arch -eq "AMD64" -or $Arch -eq "x86_64") {
    $env:PLATFORM = "linux/amd64"
    $env:SQL_IMAGE = "mcr.microsoft.com/mssql/server:2019-latest"
    Write-Host "✅ Detected AMD64/x86_64 platform (Intel/AMD)" -ForegroundColor Green
}
else {
    $env:PLATFORM = "linux/amd64"
    $env:SQL_IMAGE = "mcr.microsoft.com/mssql/server:2019-latest"
    Write-Host "⚠️ Unknown architecture ($Arch), defaulting to AMD64" -ForegroundColor Yellow
}

Write-Host "Platform: $env:PLATFORM" -ForegroundColor Cyan
Write-Host "SQL Image: $env:SQL_IMAGE" -ForegroundColor Cyan
Write-Host ""

# Check if docker-compose is available
$dockerComposeV2 = $false
if (Get-Command docker -ErrorAction SilentlyContinue) {
    try {
        docker compose version | Out-Null
        $dockerComposeV2 = $true
        Write-Host "Using Docker Compose V2" -ForegroundColor Green
    } catch {
        $dockerComposeV2 = $false
    }
}

if (-not $dockerComposeV2 -and -not (Get-Command docker-compose -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker Compose not found. Please install Docker Desktop." -ForegroundColor Red
    exit 1
}

if (-not $dockerComposeV2) {
    Write-Host "Using Docker Compose V1" -ForegroundColor Green
}

Write-Host ""
Write-Host "Starting containers..." -ForegroundColor Cyan
Write-Host ""

# Run docker-compose
if ($dockerComposeV2) {
    docker compose up @DockerComposeArgs
} else {
    docker-compose up @DockerComposeArgs
}

