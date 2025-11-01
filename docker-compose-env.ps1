# Auto-detect platform and set environment variables for docker-compose (PowerShell)
# For Windows PowerShell

# Detect architecture
$Arch = $env:PROCESSOR_ARCHITECTURE
$ProcArch = (Get-WmiObject Win32_Processor).Architecture

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

Write-Host "Platform: $env:PLATFORM"
Write-Host "SQL Image: $env:SQL_IMAGE"
Write-Host ""
Write-Host "You can now run: docker-compose up" -ForegroundColor Cyan

