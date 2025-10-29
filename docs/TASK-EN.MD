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
- **Database:** PostgreSQL or MS SQL Server with EF Core
- **Message Broker:** Kafka or RabbitMQ (for asynchronous part)
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