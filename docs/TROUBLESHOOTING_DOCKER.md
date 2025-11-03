# Устранение проблем с Docker контейнерами

## Проблема: Контейнеры taskforge.api, taskforge.eventprocessor, taskforge.client не запускаются

### Диагностика

#### 1. Проверка статуса контейнеров
```bash
docker-compose ps
```

#### 2. Проверка логов
```bash
# API
docker-compose logs taskforge.api

# EventProcessor
docker-compose logs taskforge.eventprocessor

# Client
docker-compose logs taskforge.client

# SQL Server
docker-compose logs sql.data
```

#### 3. Проверка зависимостей
```bash
# Проверить, что sql.data запущен
docker-compose ps sql.data

# Проверить, что rabbitmq запущен
docker-compose ps rabbitmq
```

### Основные причины и решения

#### ❌ Проблема 1: SQL Server не запускается / падает

**Симптомы:**
- `sql.data` в статусе `Exited` или постоянно перезапускается
- Логи содержат: `fatal error`, `core.sqlservr`, `Dump already generated`

**Причины:**
1. **Azure SQL Edge нестабилен** на некоторых системах
   - Может падать с фатальной ошибкой при запуске
   - Особенно на Apple Silicon с эмуляцией

2. **Несовместимость версии БД** (Azure SQL Edge vs SQL Server 2019)
   - Azure SQL Edge создает БД версии 904
   - SQL Server 2019 создает БД версии 921
   - При переключении между ними возникает конфликт

3. **Недостаточно памяти**
   - SQL Server требует минимум 2GB RAM

**Решение:**

### ✅ **Рекомендация: Используйте SQL Server 2019 даже на ARM Mac**

Docker Desktop на Mac с Apple Silicon поддерживает запуск x86 контейнеров через Rosetta 2. SQL Server 2019 работает стабильнее чем Azure SQL Edge.

```bash
# Используйте x86 конфигурацию даже на ARM
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

**Альтернатива (если нужно использовать Azure SQL Edge):**

```bash
# 1. Удалить старые volumes
docker-compose down -v

# 2. Убедиться, что healthcheck отключен в docker-compose.override.arm.yml

# 3. Пересоздать контейнеры с чистой БД
docker-compose -f docker-compose.yml -f docker-compose.override.arm.yml up -d sql.data

# 4. Подождать, пока SQL Server полностью запустится (60-90 секунд)

# 5. Проверить логи - если падает с фатальной ошибкой, используйте SQL Server 2019
docker logs sql.data
```

#### ❌ Проблема 2: API не может подключиться к БД

**Симптомы:**
- Логи API содержат: `network-related error`, `Could not open a connection`
- Health check возвращает 503

**Решение:**

1. Проверить connection string в docker-compose:
```yaml
ConnectionString=Server=sql.data,1433;...
ConnectionStrings__DefaultConnection=Server=sql.data,1433;...
TrustServerCertificate=True;  # Важно для локальной разработки!
```

2. Убедиться, что база данных `TaskForge` создана:
```bash
docker exec sql.data /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd123' -C -Q "SELECT name FROM sys.databases WHERE name = 'TaskForge'"
```

3. Если базы нет, создать вручную:
```bash
docker exec sql.data /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd123' -C -Q "CREATE DATABASE TaskForge;"
```

4. Применить миграции вручную (если нужно):
```bash
dotnet ef database update --project src/TaskForge.Persistence/TaskForge.Persistence.csproj --startup-project src/TaskForge.API/TaskForge.API.csproj
```

#### ❌ Проблема 3: Health check для Azure SQL Edge не работает

**Симптомы:**
- `sql.data` в статусе `health: starting` или не проходит healthcheck
- Зависимые сервисы не запускаются (ожидают `service_healthy`)

**Решение:**

Для Azure SQL Edge healthcheck отключен, так как `sqlcmd` недоступен в стандартном пути.

Зависимые сервисы используют `service_started` вместо `service_healthy`:
```yaml
depends_on:
  sql.data:
    condition: service_started  # Вместо service_healthy
```

#### ❌ Проблема 4: Client не может найти taskforge.api

**Симптомы:**
- Логи client: `host not found in upstream "taskforge.api"`

**Решение:**

1. Убедиться, что API и Client в одной сети:
```yaml
networks:
  - backend
  - frontend  # для client
```

2. Проверить, что API запущен:
```bash
docker-compose ps taskforge.api
```

3. Убедиться, что Client зависит от API:
```yaml
depends_on:
  taskforge.api:
    condition: service_started  # или service_healthy если API проходит healthcheck
```

### Быстрое решение для ARM (Apple Silicon)

**Рекомендуется использовать SQL Server 2019 через Rosetta 2:**

```bash
# Используйте x86 версию с эмуляцией (работает на ARM Mac)
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

В `docker-compose.override.yml` настроен SQL Server 2019 с правильным healthcheck через `sqlcmd`.

### Проверка успешного запуска

После исправления проблем проверьте:

```bash
# Все контейнеры должны быть Up
docker-compose ps

# API должен отвечать
curl http://localhost:5009/health/ready

# EventProcessor должен отвечать
curl http://localhost:5010/health

# Client должен быть доступен
curl http://localhost:3000
```

### Перезапуск всех сервисов

```bash
# Остановить всё
docker-compose down

# Удалить volumes (если нужна чистая БД)
docker-compose down -v

# Запустить заново
docker-compose up -d

# Проверить статус
docker-compose ps
```

### Известные проблемы

1. **Azure SQL Edge падает с фатальной ошибкой**
   - Решение: Используйте SQL Server 2019 (`docker-compose.override.yml`) даже на ARM Mac
   - SQL Server 2019 работает стабильнее через Rosetta 2

2. **Healthcheck для Azure SQL Edge не работает**
   - Решение: Healthcheck отключен, используется `service_started` в `depends_on`

3. **Миграции не применяются автоматически**
   - Решение: Применить вручную через `dotnet ef database update` или проверить connection string
