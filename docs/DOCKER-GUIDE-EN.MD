# Docker Guide and Troubleshooting for TaskForge

This guide provides comprehensive instructions for running TaskForge using Docker and Docker Compose, including troubleshooting common issues.

## Table of Contents

1. [Quick Start](#quick-start)
2. [Prerequisites](#prerequisites)
3. [Platform Detection](#platform-detection)
4. [Running Services](#running-services)
5. [Service Configuration](#service-configuration)
6. [Troubleshooting](#troubleshooting)
7. [Useful Commands](#useful-commands)

## Quick Start

### Mac/Linux
```bash
# Make executable (first time only)
chmod +x docker-compose-up.sh

# Run all services
./docker-compose-up.sh

# Run in background
./docker-compose-up.sh -d
```

### Windows PowerShell
```powershell
.\docker-compose-up.ps1

# Run in background
.\docker-compose-up.ps1 -d
```

### Windows Command Prompt
```cmd
docker-compose-up.bat

# Run in background
docker-compose-up.bat -d
```

### Manual Docker Compose
```bash
docker-compose up -d
```

## Prerequisites

- **Docker Desktop** (includes Docker Compose) installed and running
- For Linux: Docker Engine + Docker Compose installed separately
- Minimum 4GB RAM available for Docker
- Ports available: 3000, 5009, 5010, 5011, 5432, 5050, 5672, 15672, 16686

## Platform Detection

The project automatically detects your platform and selects the appropriate Docker images.

### Automatic Detection

The startup scripts (`docker-compose-up.sh`, `.ps1`, `.bat`) automatically:
1. Detect your platform (ARM64 or AMD64/x86_64)
2. Set `PLATFORM` environment variable
3. Select correct Docker images

### Manual Configuration

If automatic detection doesn't work, set manually:

**Linux/Mac (Bash)**
```bash
export PLATFORM=linux/amd64  # or linux/arm64
docker-compose up
```

**Windows PowerShell**
```powershell
$env:PLATFORM="linux/amd64"
docker-compose up
```

**Windows CMD**
```cmd
set PLATFORM=linux/amd64
docker-compose up
```

### Platform Details

| Platform | Architecture | Detection |
|----------|-------------|-----------|
| **Mac (Apple Silicon)** | ARM64 | `sysctl hw.optional.arm64` |
| **Mac (Intel)** | AMD64 | `uname -m` → `x86_64` |
| **Linux ARM** | ARM64/aarch64 | `uname -m` → `aarch64` |
| **Linux x86** | AMD64 | `uname -m` → `x86_64` |
| **Windows ARM** | ARM64 | `%PROCESSOR_ARCHITECTURE%` |
| **Windows x86** | AMD64 | `%PROCESSOR_ARCHITECTURE%` |

## Running Services

### Start All Services
```bash
docker-compose up -d
```

### Start Specific Service
```bash
docker-compose up -d taskforge.api
docker-compose up -d taskforge.client
```

### Stop All Services
```bash
docker-compose down
```

### Stop Specific Service
```bash
docker-compose stop taskforge.api
docker-compose rm -f taskforge.api
```

### Rebuild Services
```bash
# Rebuild all
docker-compose build --no-cache

# Rebuild specific service
docker-compose build --no-cache taskforge.api
```

## Service Configuration

### Available Services

| Service | Container Name | Port | Description |
|---------|---------------|------|-------------|
| **PostgreSQL** | `postgres.data` | 5432 | Database |
| **TaskForge API** | `taskforge.api` | 5009 | Main API service |
| **Event Processor** | `taskforge.eventprocessor` | 5010 | Synchronous event processing |
| **Message Consumer** | `taskforge.messageconsumer` | 5011 | RabbitMQ message consumer |
| **Client** | `taskforge.client` | 3000 | React frontend |
| **RabbitMQ** | `rabbitmq` | 5672, 15672 | Message broker |
| **Jaeger** | `jaeger` | 16686 | Distributed tracing UI |
| **OpenTelemetry Collector** | `otel-collector` | 4317, 4318 | Telemetry collection |
| **pgAdmin** | `pgadmin` | 5050 | Database administration |

### Environment Variables

Key environment variables (can be set in `.env` file or shell):

- `PLATFORM` - Target platform (`linux/amd64` or `linux/arm64`)
- `POSTGRES_IMAGE` - PostgreSQL image (default: `postgres:16-alpine`)
- `CLIENT_PORT` - Client port (default: `3000`)
- `REACT_APP_API_URL` - API URL for client (default: `/api`)
- `RABBITMQ_USER` - RabbitMQ username (default: `guest`)
- `RABBITMQ_PASSWORD` - RabbitMQ password (default: `guest`)
- `PGADMIN_EMAIL` - pgAdmin email (default: `admin@pgadmin.org`)
- `PGADMIN_PASSWORD` - pgAdmin password (default: `admin`)

### Docker Compose Files

- `docker-compose.yml` - Main configuration (base services)
- `docker-compose.override.yml` - Development overrides (ports, env vars, networks)
- Scripts auto-select appropriate platform configuration

## Troubleshooting

### Container Won't Start

**Problem:** Container exits immediately or shows "Exited" status

**Solutions:**
```bash
# Check container logs
docker logs <container-name>

# Check all containers status
docker-compose ps

# Restart specific container
docker-compose restart <service-name>

# Remove and recreate container
docker-compose up -d --force-recreate <service-name>
```

### Port Already in Use

**Problem:** `Bind for 0.0.0.0:3000 failed: port is already allocated`

**Solutions:**
```bash
# Find process using port
# Mac/Linux:
lsof -i :3000
# Windows:
netstat -ano | findstr :3000

# Stop the conflicting service or change port in docker-compose.override.yml
# Change CLIENT_PORT environment variable
export CLIENT_PORT=3001
docker-compose up -d taskforge.client
```

### Container Names Have "-1" Suffix

**Problem:** Containers named like `taskforge-taskforge.client-1` instead of `taskforge.client`

**Solution:**
```bash
# Stop and remove old containers
docker-compose down

# Remove old containers manually if needed
docker stop taskforge-taskforge.client-1
docker rm taskforge-taskforge.client-1

# Restart with correct names
docker-compose up -d
```

### Client Not Accessible on Port 3000

**Problem:** Client container runs but not accessible at `http://localhost:3000`

**Solutions:**
```bash
# Check if port is mapped correctly
docker ps | grep taskforge.client
# Should show: 0.0.0.0:3000->80/tcp

# Check container logs
docker logs taskforge.client

# Verify nginx is running inside container
docker exec taskforge.client nginx -t

# Restart client
docker-compose restart taskforge.client
```

### Database Connection Errors

**Problem:** Services can't connect to PostgreSQL

**Solutions:**
```bash
# Check PostgreSQL is running and healthy
docker-compose ps postgres.data

# Check PostgreSQL logs
docker logs postgres.data

# Verify connection string
docker exec taskforge.api printenv | grep ConnectionString

# Test connection from API container
docker exec taskforge.api wget -qO- http://localhost:80/health
```

### RabbitMQ Connection Errors

**Problem:** MessageConsumer fails to connect to RabbitMQ

**Solutions:**
```bash
# Check RabbitMQ is healthy
docker-compose ps rabbitmq

# Check RabbitMQ logs
docker logs rabbitmq

# Verify RabbitMQ is accessible
docker exec taskforge.messageconsumer printenv | grep RabbitMQ

# Check RabbitMQ management UI
# Open http://localhost:15672 (guest/guest)
```

### Platform Detection Issues

**Problem:** Wrong platform detected (ARM64 image on AMD64 or vice versa)

**Solutions:**
```bash
# Check detected architecture
# Mac/Linux:
uname -m
# Windows:
echo %PROCESSOR_ARCHITECTURE%

# Force platform manually
export PLATFORM=linux/amd64  # or linux/arm64
docker-compose up -d
```

### Build Errors

**Problem:** `docker-compose build` fails

**Solutions:**
```bash
# Clean build (no cache)
docker-compose build --no-cache

# Clean Docker system
docker system prune -a

# Check Dockerfile syntax
docker build -t test -f src/TaskForge.API/Dockerfile .
```

### Out of Memory Errors

**Problem:** Containers killed due to memory limits

**Solutions:**
- Increase Docker Desktop memory allocation (Settings → Resources → Memory)
- Minimum recommended: 4GB
- For all services: 6-8GB recommended

### Container Health Checks Fail

**Problem:** Health check endpoints return unhealthy

**Solutions:**
```bash
# Check health endpoint manually
curl http://localhost:5009/health/ready

# Check service logs
docker logs taskforge.api

# Increase health check timeout in docker-compose.override.yml
healthcheck:
  interval: 30s  # Increase from 10s
  timeout: 10s   # Increase from 5s
  retries: 5     # Increase retries
```

### Network Issues

**Problem:** Containers can't communicate with each other

**Solutions:**
```bash
# Check networks
docker network ls
docker network inspect taskforge_backend

# Verify containers are on same network
docker inspect taskforge.api | grep NetworkMode
docker inspect postgres.data | grep NetworkMode

# Recreate networks
docker-compose down
docker network prune
docker-compose up -d
```

## Useful Commands

### View Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker logs taskforge.api -f

# Last 100 lines
docker logs taskforge.api --tail 100
```

### Container Management
```bash
# List running containers
docker ps

# List all containers (including stopped)
docker ps -a

# Execute command in container
docker exec -it taskforge.api bash
docker exec taskforge.api dotnet --version

# View container resources
docker stats
```

### Cleanup
```bash
# Stop and remove all containers
docker-compose down

# Remove volumes (⚠️ deletes database data)
docker-compose down -v

# Remove all unused images
docker image prune -a

# Full cleanup (⚠️ removes everything)
docker system prune -a --volumes
```

### Health Checks
```bash
# Check API health
curl http://localhost:5009/health
curl http://localhost:5009/health/ready

# Check client health
curl http://localhost:3000/health

# Check EventProcessor health
curl http://localhost:5010/health
```

### Database Management
```bash
# Connect to PostgreSQL
docker exec -it postgres.data psql -U postgres -d TaskForge

# Backup database
docker exec postgres.data pg_dump -U postgres TaskForge > backup.sql

# View database logs
docker logs postgres.data -f
```

### RabbitMQ Management
```bash
# Access RabbitMQ Management UI
# Open browser: http://localhost:15672
# Login: guest / guest

# Check RabbitMQ status
docker exec rabbitmq rabbitmq-diagnostics ping
```

## Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres)
- [RabbitMQ Docker Hub](https://hub.docker.com/_/rabbitmq)

## Notes

- All services use health checks for dependency management
- Services wait for dependencies to be healthy before starting
- PostgreSQL migrations run automatically on API startup
- Client uses nginx to proxy API requests internally
- All containers are connected via Docker networks for secure communication

