# Docker Compose - Cross-Platform Setup Guide

This guide explains how to use Docker Compose with automatic platform detection for Mac (Intel/Apple Silicon), Windows, and Linux.

## Quick Start

### Mac/Linux
```bash
# Make executable (first time only)
chmod +x docker-compose-up.sh

# Run
./docker-compose-up.sh
```

### Windows PowerShell
```powershell
.\docker-compose-up.ps1
```

### Windows Command Prompt
```cmd
docker-compose-up.bat
```

## How It Works

The setup automatically:
1. **Detects your platform** (ARM64 or AMD64/x86_64)
2. **Selects the correct SQL Server image**:
   - AMD64/x86_64: `mcr.microsoft.com/mssql/server:2019-latest`
   - ARM64: `mcr.microsoft.com/azure-sql-edge:latest`
3. **Sets platform flags** for all containers

## Manual Configuration (Optional)

If you prefer to set environment variables manually:

### Linux/Mac (Bash)
```bash
export PLATFORM=linux/amd64  # or linux/arm64
export SQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest  # or azure-sql-edge:latest
docker-compose up
```

### Windows PowerShell
```powershell
$env:PLATFORM="linux/amd64"
$env:SQL_IMAGE="mcr.microsoft.com/mssql/server:2019-latest"
docker-compose up
```

### Windows CMD
```cmd
set PLATFORM=linux/amd64
set SQL_IMAGE=mcr.microsoft.com/mssql/server:2019-latest
docker-compose up
```

## Platform Detection Details

### Mac
- **Apple Silicon (M1/M2/M3)**: Detected as `arm64` → uses `azure-sql-edge`
- **Intel**: Detected as `x86_64` → uses `mssql/server:2019-latest`

### Windows
- **ARM64**: Detected via `PROCESSOR_ARCHITECTURE` → uses `azure-sql-edge`
- **AMD64/x86_64**: Uses `mssql/server:2019-latest`

### Linux
- **ARM64/aarch64**: Uses `azure-sql-edge`
- **x86_64/amd64**: Uses `mssql/server:2019-latest`

## Docker Compose Files

- `docker-compose.yml` - Main configuration (uses environment variables)
- `docker-compose.override.yml` - Development overrides (ports, env vars, networks)
- `docker-compose.override.arm.yml` - Legacy ARM override (not needed with auto-detection)

## Troubleshooting

### SQL Server fails to start on ARM64
- Ensure you're using `azure-sql-edge:latest` for ARM64
- Check Docker logs: `docker logs sql.data`

### Platform detection incorrect
- Manually set `PLATFORM` and `SQL_IMAGE` environment variables
- Check your architecture: 
  - Mac/Linux: `uname -m`
  - Windows: `echo %PROCESSOR_ARCHITECTURE%`

### Docker Compose not found
- Install Docker Desktop (includes Docker Compose)
- For Linux, install separately: `sudo apt-get install docker-compose`

## Services

- **sql.data**: SQL Server database (port 5433)
- **taskforge.api**: API service (port 5009)
- **taskforge.eventprocessor**: Event processor service (port 5010)

## Useful Commands

```bash
# Start services in background
./docker-compose-up.sh -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Rebuild images
docker-compose build --no-cache
```

