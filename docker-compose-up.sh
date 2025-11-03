#!/bin/bash
# Universal docker-compose startup script for Mac/Linux
# Automatically detects platform and runs docker-compose

# set -e  # Removed to prevent script from stopping on non-critical errors

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

# For macOS, check if running on Apple Silicon (even if in Rosetta mode)
if [[ "$OS" == "Darwin" ]]; then
    # Check if ARM64 is available (works even in Rosetta 2)
    if sysctl -n hw.optional.arm64 2>/dev/null | grep -q "1"; then
        ARCH="arm64"
    fi
fi

# Detect platform
if [[ "$ARCH" == "arm64" ]] || [[ "$ARCH" == "aarch64" ]]; then
    export PLATFORM="linux/arm64"
    export POSTGRES_IMAGE="postgres:16-alpine"
    OVERRIDE_FILE="docker-compose.override.arm.yml"
    echo -e "${GREEN}✅ Detected ARM64 platform (Apple Silicon M1/M2/M3 or ARM Linux)${NC}"
elif [[ "$ARCH" == "x86_64" ]] || [[ "$ARCH" == "amd64" ]] || [[ "$ARCH" == "i386" ]]; then
    export PLATFORM="linux/amd64"
    export POSTGRES_IMAGE="postgres:16-alpine"
    OVERRIDE_FILE="docker-compose.override.yml"
    echo -e "${GREEN}✅ Detected AMD64/x86_64 platform (Intel/AMD)${NC}"
else
    export PLATFORM="linux/amd64"
    export POSTGRES_IMAGE="postgres:16-alpine"
    OVERRIDE_FILE="docker-compose.override.yml"
    echo -e "${YELLOW}⚠️ Unknown architecture ($ARCH), defaulting to AMD64${NC}"
fi

echo -e "${BLUE}Platform:${NC} $PLATFORM"
echo -e "${BLUE}PostgreSQL Image:${NC} $POSTGRES_IMAGE"
echo -e "${BLUE}pgAdmin Email:${NC} ${PGADMIN_EMAIL:-admin@pgadmin.org} (default)"
echo -e "${BLUE}Override file:${NC} $OVERRIDE_FILE"
echo -e "${BLUE}OS:${NC} $OS"
echo ""

# Use docker compose (v2) if available, otherwise docker-compose (v1)
# Prefer docker compose (v2) if docker is available, otherwise use docker-compose (v1)
if command -v docker &> /dev/null; then
    # Assume docker compose is available if docker is installed (typical in Docker Desktop)
    COMPOSE_CMD_ARGS=("docker" "compose")
    echo -e "${GREEN}Using Docker Compose V2${NC}"
elif command -v docker-compose &> /dev/null; then
    COMPOSE_CMD_ARGS=("docker-compose")
    echo -e "${GREEN}Using Docker Compose V1${NC}"
else
    echo -e "${YELLOW}❌ Docker Compose not found. Please install Docker Desktop.${NC}"
    exit 1
fi

# Parse arguments for rebuild flag
BUILD_FLAG=""
NO_CACHE_FLAG=""
DOCKER_COMPOSE_ARGS=()

# Parse command line arguments
for arg in "$@"; do
    case $arg in
        --rebuild|-b)
            BUILD_FLAG="--build"
            echo -e "${YELLOW}🔄 Rebuild flag detected - images will be rebuilt${NC}"
            ;;
        --rebuild-no-cache|--no-cache|-B)
            BUILD_FLAG="--build"
            NO_CACHE_FLAG="--no-cache"
            echo -e "${YELLOW}🔄 Full rebuild flag detected (no cache) - images will be rebuilt from scratch${NC}"
            ;;
        *)
            DOCKER_COMPOSE_ARGS+=("$arg")
            ;;
    esac
done

echo ""

echo -e "${BLUE}Starting containers...${NC}"
echo ""

# Export variables for docker-compose (they will be picked up automatically)
export PLATFORM
export POSTGRES_IMAGE

# Build images if rebuild flag is set
# Note: Platform is controlled via PLATFORM environment variable and docker-compose.yml platform settings
if [[ -n "$BUILD_FLAG" ]]; then
    echo -e "${BLUE}Building images for platform: $PLATFORM${NC}"
    if [[ -n "$NO_CACHE_FLAG" ]]; then
        "${COMPOSE_CMD_ARGS[@]}" -f docker-compose.yml -f "$OVERRIDE_FILE" build --no-cache || {
            echo -e "${YELLOW}⚠️ Build failed, continuing anyway...${NC}"
        }
    else
        "${COMPOSE_CMD_ARGS[@]}" -f docker-compose.yml -f "$OVERRIDE_FILE" build || {
            echo -e "${YELLOW}⚠️ Build failed, continuing anyway...${NC}"
        }
    fi
    echo ""
fi

# Run docker-compose with all compose files
# Explicitly specify both compose files to ensure correct configuration
# Platform is already set via environment variable, so containers will use the correct platform
echo -e "${BLUE}Starting services with: docker-compose.yml and $OVERRIDE_FILE${NC}"
"${COMPOSE_CMD_ARGS[@]}" -f docker-compose.yml -f "$OVERRIDE_FILE" up "${DOCKER_COMPOSE_ARGS[@]}"

