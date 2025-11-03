# Task 2: SQL Function for Payment Analysis

**Language / Язык:** [English](README.md) | [Русский](README-RU.md)

This directory contains PostgreSQL scripts and Docker setup for Task 2 - SQL Function for Payment Analysis.

## Overview

This setup automatically:
1. Creates a PostgreSQL database
2. Creates the `ClientPayments` table
3. Creates the `GetDailyPayments` function
4. Inserts seed data
5. Runs tests
6. Verifies results

## Quick Start

### Option 1: Using Docker Compose (Recommended)

```bash
cd src/database/postgres
docker-compose up --build
```

This will:
- Start PostgreSQL server
- Wait for it to be ready
- Run all SQL scripts automatically
- Display test results
- Exit when complete

### Option 2: Using Docker directly

First, start PostgreSQL:
```bash
docker run -d \
  --name postgres-task2 \
  -e POSTGRES_DB=taskforgedb \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5433:5432 \
  postgres:16-alpine
```

Wait for PostgreSQL to be ready, then run the scripts:
```bash
docker build -t task2-runner -f Dockerfile.runner .
docker run --rm \
  --link postgres-task2:postgres \
  -e POSTGRES_HOST=postgres-task2 \
  -e POSTGRES_DB=taskforgedb \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  task2-runner
```

### Option 3: Manual execution

If you have PostgreSQL installed locally:

```bash
cd src/database/postgres
export PGPASSWORD=postgres
./run-all.sh
```

## Script Execution Order

1. **01-create-database.sql** - Creates the database
2. **02-create-table.sql** - Creates ClientPayments table with indexes
3. **03-create-function.sql** - Creates GetDailyPayments function
4. **04-insert-seed-data.sql** - Inserts test data
5. **05-run-tests.sql** - Runs test queries
6. **06-verify-results.sql** - Compares actual vs expected results

## Function Signature

```sql
CREATE FUNCTION public.getdailypayments(
    p_clientid bigint,
    p_startdate date,
    p_enddate date
)
RETURNS TABLE (
    dt date,
    amount numeric(18, 2)
)
```

## Expected Results

### Example 1: ClientId=1, 2022-01-02 to 2022-01-07
| Dt         | Amount |
|------------|--------|
| 2022-01-02 | 0      |
| 2022-01-03 | 100    |
| 2022-01-04 | 0      |
| 2022-01-05 | 450    |
| 2022-01-06 | 0      |
| 2022-01-07 | 50     |

### Example 2: ClientId=2, 2022-01-04 to 2022-01-11
| Dt         | Amount |
|------------|--------|
| 2022-01-04 | 0      |
| 2022-01-05 | 278    |
| 2022-01-06 | 0      |
| 2022-01-07 | 0      |
| 2022-01-08 | 0      |
| 2022-01-09 | 0      |
| 2022-01-10 | 300    |
| 2022-01-11 | 0      |

## Cleanup

To remove containers and volumes:
```bash
docker-compose down -v
```

## Notes

- The function uses PostgreSQL's `generate_series` for date generation
- All days in the range are included (inclusive start and end dates)
- Days without payments return 0, not NULL
- The function handles date ranges spanning multiple years

