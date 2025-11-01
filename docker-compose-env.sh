#!/bin/bash
# Auto-detect platform and set environment variables for docker-compose

# Detect architecture
ARCH=$(uname -m)
OS=$(uname -s)

# Detect platform
if [[ "$ARCH" == "arm64" ]] || [[ "$ARCH" == "aarch64" ]]; then
    export PLATFORM="linux/arm64"
    export SQL_IMAGE="mcr.microsoft.com/azure-sql-edge:latest"
    echo "✅ Detected ARM64 platform (Apple Silicon M1/M2/M3 or ARM Linux)"
elif [[ "$ARCH" == "x86_64" ]] || [[ "$ARCH" == "amd64" ]]; then
    export PLATFORM="linux/amd64"
    export SQL_IMAGE="mcr.microsoft.com/mssql/server:2019-latest"
    echo "✅ Detected AMD64/x86_64 platform (Intel/AMD)"
else
    export PLATFORM="linux/amd64"
    export SQL_IMAGE="mcr.microsoft.com/mssql/server:2019-latest"
    echo "⚠️ Unknown architecture ($ARCH), defaulting to AMD64"
fi

echo "Platform: $PLATFORM"
echo "SQL Image: $SQL_IMAGE"
echo ""
echo "You can now run: docker-compose up"

