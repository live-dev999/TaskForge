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
2. **Selects the correct PostgreSQL image**: `postgres:16-alpine` (supports both ARM64 and AMD64)
3. **Sets platform flags** for all containers

## Manual Configuration (Optional)

If you prefer to set environment variables manually:

### Linux/Mac (Bash)
```bash
export PLATFORM=linux/amd64  # or linux/arm64
export POSTGRES_IMAGE=postgres:16-alpine
docker-compose up
```

### Windows PowerShell
```powershell
$env:PLATFORM="linux/amd64"
$env:POSTGRES_IMAGE="postgres:16-alpine"
docker-compose up
```

### Windows CMD
```cmd
set PLATFORM=linux/amd64
set POSTGRES_IMAGE=postgres:16-alpine
docker-compose up
```

## Platform Detection Details

PostgreSQL supports both ARM64 and AMD64 platforms, so the same image (`postgres:16-alpine`) works on all architectures:

### Mac
- **Apple Silicon (M1/M2/M3)**: Detected as `arm64` → uses `postgres:16-alpine`
- **Intel**: Detected as `x86_64` → uses `postgres:16-alpine`

### Windows
- **ARM64**: Detected via `PROCESSOR_ARCHITECTURE` → uses `postgres:16-alpine`
- **AMD64/x86_64**: Uses `postgres:16-alpine`

### Linux
- **ARM64/aarch64**: Uses `postgres:16-alpine`
- **x86_64/amd64**: Uses `postgres:16-alpine`

## Docker Compose Files

- `docker-compose.yml` - Main configuration (uses environment variables)
- `docker-compose.override.yml` - Development overrides (ports, env vars, networks)
- `docker-compose.override.arm.yml` - Legacy ARM override (not needed with auto-detection)

## Troubleshooting

### PostgreSQL fails to start
- Check Docker logs: `docker logs postgres.data`
- Verify PostgreSQL is healthy: `docker-compose ps postgres.data`
- Ensure port 5432 is not already in use

### Platform detection incorrect
- Manually set `PLATFORM` and `POSTGRES_IMAGE` environment variables
- Check your architecture: 
  - Mac/Linux: `uname -m`
  - Windows: `echo %PROCESSOR_ARCHITECTURE%`

### Docker Compose not found
- Install Docker Desktop (includes Docker Compose)
- For Linux, install separately: `sudo apt-get install docker-compose`

## Services

- **postgres.data**: PostgreSQL database (port 5432)
- **pgadmin**: Database administration UI (port 5050)
- **taskforge.api**: API service (port 5009)
- **taskforge.eventprocessor**: Event processor service (port 5010)
- **taskforge.messageconsumer**: Message consumer service (port 5011)
- **taskforge.client**: React frontend (port 3000)
- **rabbitmq**: Message broker (ports 5672, 15672)
- **jaeger**: Distributed tracing UI (port 16686)
- **otel-collector**: OpenTelemetry collector (ports 4317, 4318)

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

