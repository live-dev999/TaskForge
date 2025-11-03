#!/bin/bash
#
# Script to run all SQL scripts for Task 2
#
# Copyright (c) 2025 Dzianis Prokharchyk
# All rights reserved.

set -e

DB_NAME="${POSTGRES_DB:-taskforgedb}"
DB_USER="${POSTGRES_USER:-postgres}"
DB_PASSWORD="${POSTGRES_PASSWORD:-postgres}"
DB_HOST="${POSTGRES_HOST:-localhost}"
DB_PORT="${POSTGRES_PORT:-5432}"

export PGPASSWORD="$DB_PASSWORD"

echo "=========================================="
echo "Task 2: Running SQL scripts"
echo "=========================================="
echo "Database: $DB_NAME"
echo "Host: $DB_HOST:$DB_PORT"
echo "User: $DB_USER"
echo ""

# Wait for PostgreSQL to be ready
echo "Waiting for PostgreSQL to be ready..."
for i in {1..30}; do
    if psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d postgres -c '\q' 2>/dev/null; then
        echo "PostgreSQL is ready!"
        break
    fi
    if [ $i -eq 30 ]; then
        echo "ERROR: PostgreSQL is not available after 30 attempts"
        exit 1
    fi
    echo "PostgreSQL is unavailable - sleeping (attempt $i/30)"
    sleep 2
done
echo ""

# Run scripts in order
echo "Step 1: Creating database..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d postgres -f 01-create-database.sql

echo ""
echo "Step 2: Creating table..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f 02-create-table.sql

echo ""
echo "Step 3: Creating function..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f 03-create-function.sql

echo ""
echo "Step 4: Inserting seed data..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f 04-insert-seed-data.sql

echo ""
echo "Step 5: Running tests..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f 05-run-tests.sql

echo ""
echo "Step 6: Verifying results..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f 06-verify-results.sql

echo ""
echo "=========================================="
echo "All scripts completed successfully!"
echo "=========================================="

