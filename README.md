# TaskForge 
Task Management Service with Events
<!-- ![logo](/design/logo.jpg?raw=true) { height=200px } -->
<img width="250" src="design/logo.jpeg?raw=true">

# About project
The project is a test task. The full description of the title can be read here [EN](/docs/TASK-EN.MD) or [RU](/docs/TASK-RU.MD)

📄 **[Сопроводительное письмо (Cover Letter)](/docs/COVER_LETTER.md)** - подробное описание реализованных улучшений и технических решений (Обязательно к прочтению)


## Build info
Azure Build Status (dev)/(master)

| Branch | Status |
|--------|:------:|
| dev    | [![Build Status](https://dev.azure.com/live-dev/TaskForge/_apis/build/status/TaskForge?branchName=dev)](https://dev.azure.com/live-dev/TaskForge/_build/latest?definitionId=2&branchName=dev) |
| master | [![Build Status](https://dev.azure.com/live-dev/TaskForge/_apis/build/status/TaskForge?branchName=master)](https://dev.azure.com/live-dev/TaskForge/_build/latest?definitionId=2&branchName=master) |



GitHub Build Status (dev)/(master)

| Branch | Status |
|--------|:------:|
| dev    | [![Build Status](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml/badge.svg?branch=dev)](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml?query=branch%3Adev) |
| master | [![Build Status](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml/badge.svg?branch=master)](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml?query=branch%3Amaster) |

[View Latest Build Run](https://github.com/live-dev999/TaskForge/actions)


Coverage (Coveralls) (dev)/(master)

| Branch | Coverage |
|--------|:-------:|
| dev    | [![Coverage Status](https://coveralls.io/repos/github/live-dev999/TaskForge/badge.svg?branch=dev)](https://coveralls.io/github/live-dev999/TaskForge?branch=dev) |
| master | [![Coverage Status](https://coveralls.io/repos/github/live-dev999/TaskForge/badge.svg?branch=master)](https://coveralls.io/github/live-dev999/TaskForge?branch=master) |


## **Preinstalled software**
### Windows:
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Database:
  - [PostgreSQL 15+](https://www.postgresql.org/download/) or Docker image

### Mac
- [Visual Studio 2022 for Mac](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio-mac/?sku=communitymac&rel=17) or [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Database:
  - [PostgreSQL 15+](https://www.postgresql.org/download/) or Docker image
  - PostgreSQL supports both Intel and Apple Silicon CPUs

### Linux
- [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Database:
  - [PostgreSQL 15+](https://www.postgresql.org/download/) or Docker image 



 ## **Getting Started**
### Steps: 
1. Install [PostgreSQL 15+](https://www.postgresql.org/download/) database or deploy database using docker
2. Set environment in appSettings.json and appSettings.Development.json
3. Migrate EF CORE or deploy a database backup
4. Build and run project (use dotnet commands or use IDEs([Visual Studio 2022 / Visual Studio for Mac](https://visualstudio.microsoft.com/downloads/) or [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/))


## **Deploy databases**
Possible Database deployment scenarios:
+ use PostgreSQL in cloud (AWS RDS, Azure Database for PostgreSQL, Google Cloud SQL, etc.)
+ use docker or docker-compose
+ deploy local PostgreSQL database

### Use Docker or Docker compose (recommended method)
Run database use docker:

```
sudo docker run -e "POSTGRES_DB=TaskForge" -e "POSTGRES_USER=postgres" -e "POSTGRES_PASSWORD=postgres" \
   -p 5432:5432 --name postgres-taskforge \
   -d \
   postgres:16-alpine
```


Run database use docker-compose:
The project already includes `docker-compose.yml` and `docker-compose.override.yml` files configured for PostgreSQL.

For Intel / Amd CPU (x86/x64):
```
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

For Apple Silicon CPU (M1/M2/M3) - ARM:
```
docker-compose -f docker-compose.yml -f docker-compose.override.arm.yml up -d
```

After starting, the following services will be available:
- **PostgreSQL**: `localhost:5432`
- **pgAdmin**: `http://localhost:5050` (default credentials: `admin@pgadmin.org` / `admin`)
- **API**: `http://localhost:5009`
- **EventProcessor**: `http://localhost:5010`
- **Client**: `http://localhost:3000`

### **Client Application**

The React client application is included in the Docker Compose setup. After starting all services:

- **Client**: Available at `http://localhost:3000` (configurable via `CLIENT_PORT` environment variable)
- **API**: Available at `http://localhost:5009/api`
- **EventProcessor**: Available at `http://localhost:5010`
- **Database**: PostgreSQL at `localhost:5432`
- **pgAdmin**: Available at `http://localhost:5050` (configurable via `PGADMIN_PORT` environment variable)
  - Default email: `admin@pgadmin.org` (standard pgAdmin default)
  - Default password: `admin` (configurable via `PGADMIN_PASSWORD` env var)

#### **Connecting to PostgreSQL via pgAdmin**

pgAdmin is pre-configured with the TaskForge database connection. Simply:

1. Open pgAdmin at `http://localhost:5050`
2. Login with credentials:
   - Email: `admin@pgadmin.org` (or your custom email via `PGADMIN_EMAIL` env var)
   - Password: `admin` (or your custom password via `PGADMIN_PASSWORD` env var)
3. You should see **"TaskForge DB"** server already configured under "Servers"
4. Click on "TaskForge DB" to expand and access the database

If the server is not visible, you can add it manually:
1. Right-click on "Servers" → "Create" → "Server"
2. In the "General" tab:
   - Name: `TaskForge DB`
3. In the "Connection" tab:
   - Host name/address: `postgres.data`
   - Port: `5432`
   - Maintenance database: `TaskForge`
   - Username: `postgres`
   - Password: `postgres`
   - Check "Save password"
4. Click "Save"

Now you can browse the database, view tables, execute queries, and monitor database activity.

#### Client Configuration

The client uses environment variables for configuration:

- `REACT_APP_API_URL` - API base URL (defaults to `/api` in Docker, uses nginx proxy)
- `CLIENT_PORT` - Client service port (defaults to `3000`)

In Docker, the client automatically proxies API requests through nginx to the backend service.

For more details, see `src/client-app/README-DOCKER.md`.

### **Deploy local PostgreSQL database (alternative method)**

Install PostgreSQL locally on your machine:

**Windows:**
- Download and install from [PostgreSQL Downloads](https://www.postgresql.org/download/windows/)
- Or use package manager: `choco install postgresql` (with Chocolatey)

**Mac:**
- Download and install from [PostgreSQL Downloads](https://www.postgresql.org/download/macosx/)
- Or use Homebrew: `brew install postgresql@16`

**Linux:**
- Ubuntu/Debian: `sudo apt-get install postgresql-16`
- CentOS/RHEL: `sudo yum install postgresql-server`
- Or download from [PostgreSQL Downloads](https://www.postgresql.org/download/linux/)

After installation, create the database:
```sql
CREATE DATABASE TaskForge;
CREATE USER postgres WITH PASSWORD 'postgres';
GRANT ALL PRIVILEGES ON DATABASE TaskForge TO postgres;
```

## 📚 Документация

Вся документация проекта находится в папке [`docs/`](/docs/):

- **[Сопроводительное письмо (Cover Letter)](/docs/COVER_LETTER.md)** - подробное описание реализованных улучшений и технических решений
- **[Описание задания (Task Description)](/docs/TASK-EN.MD)** - оригинальное задание на английском языке
- **[Описание задания (Task Description RU)](/docs/TASK-RU.MD)** - оригинальное задание на русском языке
- **[Docker Guide (EN)](/docs/DOCKER-GUIDE-EN.MD)** - полное руководство по Docker и устранению неполадок на английском
- **[Docker Guide (RU)](/docs/DOCKER-GUIDE-RU.MD)** - полное руководство по Docker и устранению неполадок на русском
- **[Docker Compose Guide](/docs/DOCKER-COMPOSE-GUIDE.md)** - руководство по использованию Docker Compose
- **[Development Sequence](/docs/DEVELOPMENT_SEQUENCE.md)** - последовательность разработки проекта
- **[Architecture Tests Summary](/docs/ARCHITECTURE_TESTS_SUMMARY.md)** - сводка архитектурных тестов
- **[Database Comparison](/docs/DATABASE_COMPARISON.md)** - сравнение баз данных
- **[Docker Platforms Explanation](/docs/DOCKER_PLATFORMS_EXPLANATION.md)** - объяснение Docker платформ
- **[Index Analysis](/docs/INDEX_ANALYSIS.md)** - анализ индексов базы данных
- **[Interview Answers](/docs/INTERVIEW_ANSWERS.md)** - ответы на вопросы интервью
- **[Test Coverage Summary](/docs/TEST_COVERAGE_SUMMARY.md)** - сводка покрытия тестами

### Просмотр диаграмм Mermaid

В проекте используются диаграммы C4 Model в формате `.mermaid`, расположенные в папке [`docs/diagrams/`](/docs/diagrams/).

Для просмотра диаграмм `.mermaid` в Visual Studio Code рекомендуется использовать одно из следующих расширений:

- **[Markdown Preview Mermaid Support](https://marketplace.visualstudio.com/items?itemName=bierner.markdown-mermaid)** - позволяет просматривать диаграммы Mermaid прямо в предпросмотре Markdown файлов
- **[Mermaid Preview](https://marketplace.visualstudio.com/items?itemName=vstirbu.vscode-mermaid-preview)** - предварительный просмотр файлов `.mermaid` с поддержкой экспорта в SVG/PNG
- **[Mermaid Editor](https://marketplace.visualstudio.com/items?itemName=TomoyukiAota.vscode-mermaid-editor)** - редактор диаграмм Mermaid с поддержкой live preview

После установки расширения откройте файл `.mermaid` и используйте предпросмотр (`Ctrl+Shift+V` / `Cmd+Shift+V`) для просмотра диаграммы.

## Commit Formats
#### Types
* API relevant changes
    * `feat` Commits, that adds a new feature
    * `fix` Commits, that fixes a bug
* `refactor` Commits, that rewrite/restructure your code, however does not change any behaviour
    * `perf` Commits are special `refactor` commits, that improves performance
* `style` Commits, that do not affect the meaning (white-space, formatting, missing semi-colons, etc)
* `test` Commits, that add missing tests or correcting existing tests
* `docs` Commits, that affect documentation only
* `build` Commits, that affect build components like build tool, ci pipeline, dependencies, project version, ...
* `devops` Commits, that affect operational components like infrastructure, deployment, backup, recovery, ...
* `chore` Miscellaneous commits e.g. modifying `.gitignore`

#### Subject
* use imperative, present tense (eg: use "add" instead of "added" or "adds")
* don't use dot(.) at end
* don't capitalize first letter

### Examples
* ```
  feat(service): add and setup swagger
  ```
* ```
  feat: remove ticket list endpoint
  
  refers to JIRA-999
  BREAKING CHANGES: ticket enpoints no longer supports list all entites.
  ```
* ```
  fix: add missing parameter to service call
  
  The error occurred because of <reasons>.
  ```
* ```
  build(release): bump version to 1.0.0
  ```
* ```
  build: update dependencies
  ```
* ```
  refactor: implement calculation method as recursion
  ```
* ```
  style: remove empty line


## Build and run applications
Before launching, be sure to set the variables in the appsettings.json configuration files. It is important to specify the correct database connection string

**For PostgreSQL:**
```
 "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TaskForge;Username=postgres;Password=postgres"
  },
```

**For Docker environment** (connection string is automatically set via environment variables):
```
Host=postgres.data;Port=5432;Database=TaskForge;Username=postgres;Password=postgres
```
Can use commands use terminal or use IDEs(Microsoft Visual Studio 2022 or VS Code):
```
dotnet build [options]
dotnet run [options]
```


