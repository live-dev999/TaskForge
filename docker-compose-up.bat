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

REM Parse arguments for rebuild flag
set BUILD_FLAG=
set NO_CACHE_FLAG=
set REMAINING_ARGS=

:parse_args
if "%~1"=="" goto end_parse
if /i "%~1"=="--rebuild" (
    set BUILD_FLAG=1
    echo 🔄 Rebuild flag detected - images will be rebuilt
    shift
    goto parse_args
)
if /i "%~1"=="-b" (
    set BUILD_FLAG=1
    echo 🔄 Rebuild flag detected - images will be rebuilt
    shift
    goto parse_args
)
if /i "%~1"=="--rebuild-no-cache" (
    set BUILD_FLAG=1
    set NO_CACHE_FLAG=1
    echo 🔄 Full rebuild flag detected (no cache) - images will be rebuilt from scratch
    shift
    goto parse_args
)
if /i "%~1"=="--no-cache" (
    set BUILD_FLAG=1
    set NO_CACHE_FLAG=1
    echo 🔄 Full rebuild flag detected (no cache) - images will be rebuilt from scratch
    shift
    goto parse_args
)
if /i "%~1"=="-B" (
    set BUILD_FLAG=1
    set NO_CACHE_FLAG=1
    echo 🔄 Full rebuild flag detected (no cache) - images will be rebuilt from scratch
    shift
    goto parse_args
)
set REMAINING_ARGS=%REMAINING_ARGS% %1
shift
goto parse_args

:end_parse

REM Check if docker-compose is available
docker compose version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Using Docker Compose V2
    echo.
    
    REM Build images if rebuild flag is set
    if defined BUILD_FLAG (
        echo Building images...
        if defined NO_CACHE_FLAG (
            docker compose build --no-cache
        ) else (
            docker compose build
        )
        echo.
    )
    
    echo Starting containers...
    echo.
    docker compose up%REMAINING_ARGS%
) else (
    docker-compose --version >nul 2>&1
    if %ERRORLEVEL% EQU 0 (
        echo Using Docker Compose V1
        echo.
        
        REM Build images if rebuild flag is set
        if defined BUILD_FLAG (
            echo Building images...
            if defined NO_CACHE_FLAG (
                docker-compose build --no-cache
            ) else (
                docker-compose build
            )
            echo.
        )
        
        echo Starting containers...
        echo.
        docker-compose up%REMAINING_ARGS%
    ) else (
        echo ❌ Docker Compose not found. Please install Docker Desktop.
        exit /b 1
    )
)

