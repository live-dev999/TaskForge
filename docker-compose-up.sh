#!/bin/bash
# Universal docker-compose startup script for Mac/Linux
# Automatically detects platform and runs docker-compose

set -e

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🐳 Docker Compose - Auto Platform Detection${NC}"
echo ""

# Detect architecture
ARCH=$(uname -m)
OS=$(uname -s)

# Detect platform
if [[ "$ARCH" == "arm64" ]] || [[ "$ARCH" == "aarch64" ]]; then
    export PLATFORM="linux/arm64"
    export SQL_IMAGE="mcr.microsoft.com/azure-sql-edge:latest"
    echo -e "${GREEN}✅ Detected ARM64 platform (Apple Silicon M1/M2/M3 or ARM Linux)${NC}"
elif [[ "$ARCH" == "x86_64" ]] || [[ "$ARCH" == "amd64" ]]; then
    export PLATFORM="linux/amd64"
    export SQL_IMAGE="mcr.microsoft.com/mssql/server:2019-latest"
    echo -e "${GREEN}✅ Detected AMD64/x86_64 platform (Intel/AMD)${NC}"
else
    export PLATFORM="linux/amd64"
    export SQL_IMAGE="mcr.microsoft.com/mssql/server:2019-latest"
    echo -e "${YELLOW}⚠️ Unknown architecture ($ARCH), defaulting to AMD64${NC}"
fi

echo -e "${BLUE}Platform:${NC} $PLATFORM"
echo -e "${BLUE}SQL Image:${NC} $SQL_IMAGE"
echo -e "${BLUE}OS:${NC} $OS"
echo ""

# Check if docker-compose is available
if ! command -v docker-compose &> /dev/null && ! command -v docker compose &> /dev/null; then
    echo -e "${YELLOW}❌ Docker Compose not found. Please install Docker Desktop.${NC}"
    exit 1
fi

# Use docker compose (v2) if available, otherwise docker-compose (v1)
if command -v docker &> /dev/null && docker compose version &> /dev/null; then
    COMPOSE_CMD="docker compose"
    echo -e "${GREEN}Using Docker Compose V2${NC}"
else
    COMPOSE_CMD="docker-compose"
    echo -e "${GREEN}Using Docker Compose V1${NC}"
fi

echo ""
echo -e "${BLUE}Starting containers...${NC}"
echo ""

# Export variables for docker-compose (they will be picked up automatically)
export PLATFORM
export SQL_IMAGE

# Run docker-compose with all compose files
# docker-compose automatically uses docker-compose.yml and docker-compose.override.yml
$COMPOSE_CMD up "$@"

