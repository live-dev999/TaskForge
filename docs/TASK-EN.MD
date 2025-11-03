# Test Task for .NET Developer: Task Management Service with Events

## Objective:
Develop an API service for user task management with support for both synchronous and asynchronous inter-service communication.

## Tasks:

### 1. **CRUD API Development for Task Management**
   - Implement create, read, update, and delete operations for tasks
   - Each task should contain: title, description, status (new/in progress/completed), creation and modification dates
   - Ensure data validation and proper handling of task status transitions

### 2. **Synchronous Inter-Service Communication**
   - Implement a separate HTTP endpoint for receiving task change events
   - Send events to the logger service on every task modification in the main service
   - Handle scenarios when the logger service is unavailable

### 3. **Asynchronous Communication via Message Broker** (optional, but preferred)
   - Configure event publishing to Kafka/RabbitMQ on task operations
   - Implement a background consumer service for processing events from the queue
   - Ensure reliable message delivery and duplicate message handling

### 4. **Logging and Observability**
   - Implement structured logging for all operations
   - Add operation tracing (OpenTelemetry) for workflow monitoring

### 5. **Reliability and Performance Assurance**
   - Implement handling of long-running operations and timeouts
   - Provide retry mechanisms for failures

## Technical Requirements:
- **Programming Language:** .NET 8 / ASP.NET Core
- **Database:** PostgreSQL with EF Core (original requirement allowed PostgreSQL or MS SQL Server)
- **Message Broker:** RabbitMQ (original requirement allowed Kafka or RabbitMQ)
- **API Documentation:** OpenAPI/Swagger format
- Code must be **covered with unit tests**
- **Architecture:** Clean architecture with SOLID principles compliance

## Additional Tasks (optional):
- **Containerization:** Dockerfile and docker-compose for the entire stack
- **Authentication:** JWT authentication for API access
- **Caching:** Redis for caching frequently requested tasks
- **Health Checks:** Endpoints for service health monitoring

## Deliverables:
- Application source code on **GitHub** with comprehensive README
- **Setup instructions** for running the project (locally and via Docker)
- **Architecture description** of the solution and design decisions
- Demonstration of **synchronous and asynchronous communication** workflows

## Evaluation Criteria:
- Architecture quality and SOLID principles compliance
- Reliability of error handling and edge cases management
- Code cleanliness and readability
- Completeness of implemented requirements
- Quality of tests and documentation


# Task 2: SQL Function for Payment Analysis

## Task Objective
It is necessary to write a SQL table-valued function that will return daily payment amounts for a given ClientId and date range (Date type). If there were no payments on a specified day, the function should return 0. Date ranges can span multiple years.

## Data Structure
Client payments table `ClientPayments`:

```sql
ClientPayments
(
    Id bigint,          -- table primary key
    ClientId bigint,    -- Client ID
    Dt datetime2(0),    -- payment date
    Amount money        -- payment amount
)
```

## Sample Input Data

| Id | ClientId | Dt                  | Amount |
|----|----------|---------------------|--------|
| 1  | 1        | 2022-01-03 17:24:00 | 100    |
| 2  | 1        | 2022-01-05 17:24:14 | 200    |
| 3  | 1        | 2022-01-05 18:23:34 | 250    |
| 4  | 1        | 2022-01-07 10:12:38 | 50     |
| 5  | 2        | 2022-01-05 17:24:14 | 278    |
| 6  | 2        | 2022-01-10 12:39:29 | 300    |

## Function Requirements
- The function should accept three parameters:
  - `@ClientId` - client identifier
  - `@StartDate` - period start date (Date type)
  - `@EndDate` - period end date (Date type)
- The function should return a table with columns:
  - `Dt` - date (Date type)
  - `Amount` - daily payment amount (money type)
- For days with no payments, the amount should be 0
- All days in the specified range should be included (including start and end dates)

## Function Usage Examples

### Example 1

**Input parameters:**
- `ClientId` = 1
- `StartDate` = 2022-01-02
- `EndDate` = 2022-01-07

**Function result:**

| Dt         | Amount |
|------------|--------|
| 2022-01-02 | 0      |
| 2022-01-03 | 100    |
| 2022-01-04 | 0      |
| 2022-01-05 | 450    |
| 2022-01-06 | 0      |
| 2022-01-07 | 50     |

### Example 2

**Input parameters:**
- `ClientId` = 2
- `StartDate` = 2022-01-04
- `EndDate` = 2022-01-11

**Function result:**

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

## Notes
- The function should work correctly for periods spanning multiple years
- Optimal performance should be ensured when working with large data volumes
- It is recommended to use recursive CTEs or number tables for date sequence generation

## Implementation Tips
1. Generate all dates in the specified range
2. Left join with payment data for the specific client
3. Group by date and calculate daily sums
4. Use COALESCE or ISNULL to replace NULL values with 0
5. Consider creating appropriate indexes on ClientId and Dt columns for better performance