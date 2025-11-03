# Объяснение использования `platform` vs `platforms` в Docker Compose

## PostgreSQL и ARM64 поддержка

✅ **PostgreSQL официально поддерживает ARM64**, включая Apple Silicon (M1/M2/M3):
- Образ `postgres:16-alpine` доступен для обеих платформ (`linux/amd64` и `linux/arm64`)
- Docker Hub автоматически предоставляет правильный вариант образа
- При указании `platform: linux/arm64` Docker скачает ARM64 версию образа
- PostgreSQL отлично работает на ARM64, производительность не уступает x86_64

## Разница между `platform` и `platforms`

### `platforms` (множественное число)
Используется в секции `build` для **сборки** образов под несколько платформ одновременно:

```yaml
taskforge.api:
  build:
    context: .
    dockerfile: src/TaskForge.API/Dockerfile
    platforms:           # ← Для сборки под несколько платформ
      - linux/amd64
      - linux/arm64
```

**Когда используется:**
- Когда у нас есть Dockerfile и мы собираем образы
- Для создания multi-arch образов (один образ для всех платформ)
- Требует Docker Buildx

### `platform` (единственное число)
Используется для **выбора** конкретной платформы при запуске готового образа:

```yaml
postgres.data:
  image: postgres:16-alpine  # Готовый образ из Docker Hub
  platform: ${PLATFORM:-linux/amd64}  # ← Выбор платформы при запуске
```

**Когда используется:**
- Для готовых образов из Docker Hub (не собираем сами)
- Выбираем платформу в зависимости от архитектуры хоста
- Docker автоматически выберет правильный вариант образа

## Почему PostgreSQL и pgAdmin используют `platform`, а не `platforms`?

### Причина 1: Готовые образы
PostgreSQL и pgAdmin — это **готовые образы** из Docker Hub:
- `postgres:16-alpine` — уже собран для всех платформ
- `dpage/pgadmin4:latest` — уже собран для всех платформ

У них нет секции `build`, поэтому нельзя указать `platforms`.

### Причина 2: Docker автоматически выбирает платформу
Docker Hub хранит образы для разных платформ:
- Когда вы указываете `platform: linux/arm64`, Docker автоматически скачает ARM64 версию
- Когда вы указываете `platform: linux/amd64`, Docker автоматически скачает AMD64 версию

### Причина 3: Переменная `${PLATFORM}` выбирается скриптом
Скрипт `docker-compose-up.sh` автоматически определяет платформу:
```bash
if [[ "$ARCH" == "arm64" ]]; then
    export PLATFORM="linux/arm64"  # PostgreSQL/pgAdmin запустятся на ARM64
else
    export PLATFORM="linux/amd64"  # PostgreSQL/pgAdmin запустятся на AMD64
fi
```

## Можно ли использовать `platforms` для готовых образов?

❌ **Нет, нельзя**. Docker Compose не поддерживает `platforms` для готовых образов без секции `build`:

```yaml
# ❌ Это вызовет ошибку:
postgres.data:
  image: postgres:16-alpine
  platforms:  # ERROR: "additional properties 'platforms' not allowed"
    - linux/amd64
    - linux/arm64
```

**Ошибка:** `services.postgres.data additional properties 'platforms' not allowed`

**Причина:**
- `platforms` работает ТОЛЬКО внутри секции `build`
- Для готовых образов нет секции `build`
- Docker Compose V2 не поддерживает `platforms` вне `build` (даже в experimental режиме)

**Решение:**
✅ Используйте `platform` с переменной `${PLATFORM}`, которая автоматически выбирается скриптом `docker-compose-up.sh`

## Текущее решение — почему оно правильное

### Для собираемых образов (TaskForge.API, TaskForge.EventProcessor):
```yaml
taskforge.api:
  platform: ${PLATFORM:-linux/amd64}  # Выбор платформы для запуска
  build:
    platforms:                        # Сборка под обе платформы
      - linux/amd64
      - linux/arm64
```

**Почему оба?**
- `platform` — для локального запуска (выбирается автоматически)
- `platforms` — для CI/CD, где нужно собрать образы под все платформы

### Для готовых образов (PostgreSQL, pgAdmin):
```yaml
postgres.data:
  image: postgres:16-alpine
  platform: ${PLATFORM:-linux/amd64}  # Только выбор платформы
```

**Почему только `platform`?**
- Нет секции `build` — образ уже готов
- Docker автоматически выберет правильный вариант из Docker Hub
- Скрипт `docker-compose-up.sh` устанавливает `${PLATFORM}` автоматически

## Итог

✅ **Текущее решение правильное:**
- PostgreSQL и pgAdmin используют `platform` потому что это готовые образы
- TaskForge сервисы используют и `platform` и `platforms` для сборки под все платформы
- Скрипт `docker-compose-up.sh` автоматически определяет платформу и устанавливает `${PLATFORM}`

❌ **Не нужно менять на `platforms`** для PostgreSQL/pgAdmin:
- Это готовые образы без секции `build`
- `platform` работает отлично и более надежно
- Docker автоматически выбирает правильный вариант образа

