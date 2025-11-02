@echo off
REM Auto-detect platform and set environment variables for docker-compose (Windows CMD)
REM For Windows Command Prompt

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
echo You can now run: docker-compose up

