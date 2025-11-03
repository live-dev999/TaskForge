# Руководство по Docker и устранению неполадок для TaskForge

Данное руководство предоставляет подробные инструкции по запуску TaskForge с использованием Docker и Docker Compose, включая решение распространенных проблем.

## Содержание

1. [Быстрый старт](#быстрый-старт)
2. [Требования](#требования)
3. [Определение платформы](#определение-платформы)
4. [Запуск сервисов](#запуск-сервисов)
5. [Конфигурация сервисов](#конфигурация-сервисов)
6. [Устранение неполадок](#устранение-неполадок)
7. [Полезные команды](#полезные-команды)

## Быстрый старт

### Mac/Linux
```bash
# Сделать исполняемым (только первый раз)
chmod +x docker-compose-up.sh

# Запустить все сервисы
./docker-compose-up.sh

# Запустить в фоновом режиме
./docker-compose-up.sh -d
```

### Windows PowerShell
```powershell
.\docker-compose-up.ps1

# Запустить в фоновом режиме
.\docker-compose-up.ps1 -d
```

### Windows Command Prompt
```cmd
docker-compose-up.bat

# Запустить в фоновом режиме
docker-compose-up.bat -d
```

### Ручной запуск Docker Compose
```bash
docker-compose up -d
```

## Требования

- **Docker Desktop** (включает Docker Compose) установлен и запущен
- Для Linux: Docker Engine + Docker Compose установлены отдельно
- Минимум 4GB RAM доступно для Docker
- Доступные порты: 3000, 5009, 5010, 5011, 5432, 5050, 5672, 15672, 16686

## Определение платформы

Проект автоматически определяет вашу платформу и выбирает подходящие Docker-образы.

### Автоматическое определение

Скрипты запуска (`docker-compose-up.sh`, `.ps1`, `.bat`) автоматически:
1. Определяют вашу платформу (ARM64 или AMD64/x86_64)
2. Устанавливают переменную окружения `PLATFORM`
3. Выбирают правильные Docker-образы

### Ручная конфигурация

Если автоматическое определение не работает, установите вручную:

**Linux/Mac (Bash)**
```bash
export PLATFORM=linux/amd64  # или linux/arm64
docker-compose up
```

**Windows PowerShell**
```powershell
$env:PLATFORM="linux/amd64"
docker-compose up
```

**Windows CMD**
```cmd
set PLATFORM=linux/amd64
docker-compose up
```

### Детали платформ

| Платформа | Архитектура | Определение |
|-----------|-------------|-------------|
| **Mac (Apple Silicon)** | ARM64 | `sysctl hw.optional.arm64` |
| **Mac (Intel)** | AMD64 | `uname -m` → `x86_64` |
| **Linux ARM** | ARM64/aarch64 | `uname -m` → `aarch64` |
| **Linux x86** | AMD64 | `uname -m` → `x86_64` |
| **Windows ARM** | ARM64 | `%PROCESSOR_ARCHITECTURE%` |
| **Windows x86** | AMD64 | `%PROCESSOR_ARCHITECTURE%` |

## Запуск сервисов

### Запустить все сервисы
```bash
docker-compose up -d
```

### Запустить конкретный сервис
```bash
docker-compose up -d taskforge.api
docker-compose up -d taskforge.client
```

### Остановить все сервисы
```bash
docker-compose down
```

### Остановить конкретный сервис
```bash
docker-compose stop taskforge.api
docker-compose rm -f taskforge.api
```

### Пересобрать сервисы
```bash
# Пересобрать все
docker-compose build --no-cache

# Пересобрать конкретный сервис
docker-compose build --no-cache taskforge.api
```

## Конфигурация сервисов

### Доступные сервисы

| Сервис | Имя контейнера | Порт | Описание |
|--------|----------------|------|----------|
| **PostgreSQL** | `postgres.data` | 5432 | База данных |
| **TaskForge API** | `taskforge.api` | 5009 | Основной API сервис |
| **Event Processor** | `taskforge.eventprocessor` | 5010 | Синхронная обработка событий |
| **Message Consumer** | `taskforge.messageconsumer` | 5011 | Потребитель сообщений RabbitMQ |
| **Client** | `taskforge.client` | 3000 | React фронтенд |
| **RabbitMQ** | `rabbitmq` | 5672, 15672 | Брокер сообщений |
| **Jaeger** | `jaeger` | 16686 | UI для распределенной трассировки |
| **OpenTelemetry Collector** | `otel-collector` | 4317, 4318 | Сбор телеметрии |
| **pgAdmin** | `pgadmin` | 5050 | Администрирование БД |

### Переменные окружения

Ключевые переменные окружения (можно установить в файле `.env` или в shell):

- `PLATFORM` - Целевая платформа (`linux/amd64` или `linux/arm64`)
- `POSTGRES_IMAGE` - Образ PostgreSQL (по умолчанию: `postgres:16-alpine`)
- `CLIENT_PORT` - Порт клиента (по умолчанию: `3000`)
- `REACT_APP_API_URL` - URL API для клиента (по умолчанию: `/api`)
- `RABBITMQ_USER` - Имя пользователя RabbitMQ (по умолчанию: `guest`)
- `RABBITMQ_PASSWORD` - Пароль RabbitMQ (по умолчанию: `guest`)
- `PGADMIN_EMAIL` - Email pgAdmin (по умолчанию: `admin@pgadmin.org`)
- `PGADMIN_PASSWORD` - Пароль pgAdmin (по умолчанию: `admin`)

### Файлы Docker Compose

- `docker-compose.yml` - Основная конфигурация (базовые сервисы)
- `docker-compose.override.yml` - Переопределения для разработки (порты, переменные окружения, сети)
- Скрипты автоматически выбирают подходящую конфигурацию платформы

## Устранение неполадок

### Контейнер не запускается

**Проблема:** Контейнер сразу завершается или показывает статус "Exited"

**Решения:**
```bash
# Проверить логи контейнера
docker logs <container-name>

# Проверить статус всех контейнеров
docker-compose ps

# Перезапустить конкретный контейнер
docker-compose restart <service-name>

# Удалить и пересоздать контейнер
docker-compose up -d --force-recreate <service-name>
```

### Порт уже занят

**Проблема:** `Bind for 0.0.0.0:3000 failed: port is already allocated`

**Решения:**
```bash
# Найти процесс, использующий порт
# Mac/Linux:
lsof -i :3000
# Windows:
netstat -ano | findstr :3000

# Остановить конфликтующий сервис или изменить порт в docker-compose.override.yml
# Изменить переменную окружения CLIENT_PORT
export CLIENT_PORT=3001
docker-compose up -d taskforge.client
```

### Имена контейнеров с суффиксом "-1"

**Проблема:** Контейнеры названы как `taskforge-taskforge.client-1` вместо `taskforge.client`

**Решение:**
```bash
# Остановить и удалить старые контейнеры
docker-compose down

# Удалить старые контейнеры вручную, если нужно
docker stop taskforge-taskforge.client-1
docker rm taskforge-taskforge.client-1

# Перезапустить с правильными именами
docker-compose up -d
```

### Клиент недоступен на порту 3000

**Проблема:** Контейнер клиента запущен, но недоступен по адресу `http://localhost:3000`

**Решения:**
```bash
# Проверить, правильно ли проброшен порт
docker ps | grep taskforge.client
# Должно показывать: 0.0.0.0:3000->80/tcp

# Проверить логи контейнера
docker logs taskforge.client

# Проверить, запущен ли nginx внутри контейнера
docker exec taskforge.client nginx -t

# Перезапустить клиент
docker-compose restart taskforge.client
```

### Ошибки подключения к базе данных

**Проблема:** Сервисы не могут подключиться к PostgreSQL

**Решения:**
```bash
# Проверить, запущен ли PostgreSQL и здоров ли он
docker-compose ps postgres.data

# Проверить логи PostgreSQL
docker logs postgres.data

# Проверить строку подключения
docker exec taskforge.api printenv | grep ConnectionString

# Протестировать подключение из контейнера API
docker exec taskforge.api wget -qO- http://localhost:80/health
```

### Ошибки подключения к RabbitMQ

**Проблема:** MessageConsumer не может подключиться к RabbitMQ

**Решения:**
```bash
# Проверить, здоров ли RabbitMQ
docker-compose ps rabbitmq

# Проверить логи RabbitMQ
docker logs rabbitmq

# Проверить доступность RabbitMQ
docker exec taskforge.messageconsumer printenv | grep RabbitMQ

# Проверить UI управления RabbitMQ
# Открыть http://localhost:15672 (guest/guest)
```

### Проблемы с определением платформы

**Проблема:** Неправильная платформа определена (образ ARM64 на AMD64 или наоборот)

**Решения:**
```bash
# Проверить определенную архитектуру
# Mac/Linux:
uname -m
# Windows:
echo %PROCESSOR_ARCHITECTURE%

# Принудительно установить платформу вручную
export PLATFORM=linux/amd64  # или linux/arm64
docker-compose up -d
```

### Ошибки сборки

**Проблема:** `docker-compose build` завершается с ошибкой

**Решения:**
```bash
# Очистка сборки (без кэша)
docker-compose build --no-cache

# Очистка системы Docker
docker system prune -a

# Проверить синтаксис Dockerfile
docker build -t test -f src/TaskForge.API/Dockerfile .
```

### Ошибки нехватки памяти

**Проблема:** Контейнеры убиваются из-за ограничений памяти

**Решения:**
- Увеличить выделение памяти в Docker Desktop (Настройки → Ресурсы → Память)
- Минимум рекомендуется: 4GB
- Для всех сервисов: рекомендуется 6-8GB

### Не проходят проверки здоровья

**Проблема:** Endpoints проверки здоровья возвращают unhealthy

**Решения:**
```bash
# Проверить endpoint здоровья вручную
curl http://localhost:5009/health/ready

# Проверить логи сервиса
docker logs taskforge.api

# Увеличить timeout проверки здоровья в docker-compose.override.yml
healthcheck:
  interval: 30s  # Увеличить с 10s
  timeout: 10s   # Увеличить с 5s
  retries: 5     # Увеличить количество попыток
```

### Проблемы с сетью

**Проблема:** Контейнеры не могут общаться друг с другом

**Решения:**
```bash
# Проверить сети
docker network ls
docker network inspect taskforge_backend

# Проверить, что контейнеры в одной сети
docker inspect taskforge.api | grep NetworkMode
docker inspect postgres.data | grep NetworkMode

# Пересоздать сети
docker-compose down
docker network prune
docker-compose up -d
```

## Полезные команды

### Просмотр логов
```bash
# Все сервисы
docker-compose logs -f

# Конкретный сервис
docker logs taskforge.api -f

# Последние 100 строк
docker logs taskforge.api --tail 100
```

### Управление контейнерами
```bash
# Список запущенных контейнеров
docker ps

# Список всех контейнеров (включая остановленные)
docker ps -a

# Выполнить команду в контейнере
docker exec -it taskforge.api bash
docker exec taskforge.api dotnet --version

# Просмотр ресурсов контейнеров
docker stats
```

### Очистка
```bash
# Остановить и удалить все контейнеры
docker-compose down

# Удалить volumes (⚠️ удаляет данные базы данных)
docker-compose down -v

# Удалить все неиспользуемые образы
docker image prune -a

# Полная очистка (⚠️ удаляет всё)
docker system prune -a --volumes
```

### Проверка здоровья
```bash
# Проверить здоровье API
curl http://localhost:5009/health
curl http://localhost:5009/health/ready

# Проверить здоровье клиента
curl http://localhost:3000/health

# Проверить здоровье EventProcessor
curl http://localhost:5010/health
```

### Управление базой данных
```bash
# Подключиться к PostgreSQL
docker exec -it postgres.data psql -U postgres -d TaskForge

# Резервная копия базы данных
docker exec postgres.data pg_dump -U postgres TaskForge > backup.sql

# Просмотр логов базы данных
docker logs postgres.data -f
```

### Управление RabbitMQ
```bash
# Доступ к UI управления RabbitMQ
# Открыть браузер: http://localhost:15672
# Логин: guest / guest

# Проверить статус RabbitMQ
docker exec rabbitmq rabbitmq-diagnostics ping
```

## Дополнительные ресурсы

- [Документация Docker](https://docs.docker.com/)
- [Документация Docker Compose](https://docs.docker.com/compose/)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres)
- [RabbitMQ Docker Hub](https://hub.docker.com/_/rabbitmq)

## Примечания

- Все сервисы используют проверки здоровья для управления зависимостями
- Сервисы ждут, пока зависимости станут здоровыми, перед запуском
- Миграции PostgreSQL запускаются автоматически при старте API
- Клиент использует nginx для проксирования запросов к API внутри сети
- Все контейнеры подключены через Docker-сети для безопасного общения

