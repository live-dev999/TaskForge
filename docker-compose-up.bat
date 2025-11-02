@echo off
REM Universal docker-compose startup script for Windows CMD
REM Automatically detects platform and runs docker-compose

echo 🐳 Docker Compose - Auto Platform Detection
echo.

REM Detect architecture
if "%PROCESSOR_ARCHITECTURE%"=="ARM64" (
    set PLATFORM=linux/arm64
    set SQL_IMAGE=mcr.microsoft.com/azure-sql-edge:latest
    echo ✅ Detected ARM64 platform (Windows on ARM)
) else if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    set PLATFORM=linux/amd64
    set SQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest
    echo ✅ Detected AMD64/x86_64 platform (Intel/AMD)
) else (
    set PLATFORM=linux/amd64
    set SQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest
    echo ⚠️ Unknown architecture (%PROCESSOR_ARCHITECTURE%), defaulting to AMD64
)

echo Platform: %PLATFORM%
echo SQL Image: %SQL_IMAGE%
echo.

REM Check if docker-compose is available
docker compose version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Using Docker Compose V2
    echo.
    echo Starting containers...
    echo.
    docker compose up %*
) else (
    docker-compose --version >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo Using Docker Compose V1
        echo.
        echo Starting containers...
        echo.
        docker-compose up %*
    ) else (
        echo ❌ Docker Compose not found. Please install Docker Desktop.
        exit /b 1
    )
)

