#!/bin/bash
# Bash script to run PostgreSQL container for API debugging
# This script starts only PostgreSQL container for local development/debugging

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${CYAN}🐘 Starting PostgreSQL container for API debugging...${NC}"
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker is not running. Please start Docker Desktop.${NC}"
    exit 1
fi

# Run docker-compose with debug configuration
docker-compose -f docker-compose.debug.yml up -d

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ PostgreSQL container started successfully!${NC}"
    echo ""
    echo -e "${YELLOW}Connection String:${NC}"
    echo -e "${GRAY}  Host=localhost${NC}"
    echo -e "${GRAY}  Port=5432${NC}"
    echo -e "${GRAY}  Database=TaskForge${NC}"
    echo -e "${GRAY}  Username=postgres${NC}"
    echo -e "${GRAY}  Password=postgres${NC}"
    echo ""
    echo -e "${CYAN}📝 To stop the container, run:${NC}"
    echo -e "${GRAY}   docker-compose -f docker-compose.debug.yml down${NC}"
    echo ""
    echo -e "${CYAN}📝 To view logs, run:${NC}"
    echo -e "${GRAY}   docker-compose -f docker-compose.debug.yml logs -f postgres.debug${NC}"
else
    echo ""
    echo -e "${RED}❌ Failed to start PostgreSQL container${NC}"
    exit 1
fi

