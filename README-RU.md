# TaskForge 
Task Management Service with Events
<!-- ![logo](/design/logo.jpg?raw=true) { height=200px } -->
<img width="250" src="design/logo.jpeg?raw=true">

**Language / Язык:** [English](README.md) | [Русский](README-RU.md)

# О проекте
Проект является тестовым заданием. Полное описание задания можно прочитать здесь [EN](/docs/TASK-EN.MD) или [RU](/docs/TASK-RU.MD)

📄 **[Сопроводительное письмо (Cover Letter)](/docs/COVER_LETTER.md)** - подробное описание реализованных улучшений и технических решений (Обязательно к прочтению)


## Информация о сборке
Статус сборки Azure (dev)/(master)

| Ветка | Статус |
|--------|:------:|
| dev    | [![Build Status](https://dev.azure.com/live-dev/TaskForge/_apis/build/status/TaskForge?branchName=dev)](https://dev.azure.com/live-dev/TaskForge/_build/latest?definitionId=2&branchName=dev) |
| master | [![Build Status](https://dev.azure.com/live-dev/TaskForge/_apis/build/status/TaskForge?branchName=master)](https://dev.azure.com/live-dev/TaskForge/_build/latest?definitionId=2&branchName=master) |



Статус сборки GitHub (dev)/(master)

| Ветка | Статус |
|--------|:------:|
| dev    | [![Build Status](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml/badge.svg?branch=dev)](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml?query=branch%3Adev) |
| master | [![Build Status](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml/badge.svg?branch=master)](https://github.com/live-dev999/TaskForge/actions/workflows/github-ci.yml?query=branch%3Amaster) |

[Посмотреть последнюю сборку](https://github.com/live-dev999/TaskForge/actions)


Покрытие тестами (Coveralls) (dev)/(master)

| Ветка | Покрытие |
|--------|:-------:|
| dev    | [![Coverage Status](https://coveralls.io/repos/github/live-dev999/TaskForge/badge.svg?branch=dev)](https://coveralls.io/github/live-dev999/TaskForge?branch=dev) |
| master | [![Coverage Status](https://coveralls.io/repos/github/live-dev999/TaskForge/badge.svg?branch=master)](https://coveralls.io/github/live-dev999/TaskForge?branch=master) |


## **Предустановленное программное обеспечение**
### Windows:
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) или [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- База данных:
  - [PostgreSQL 15+](https://www.postgresql.org/download/) или Docker образ

### Mac
- [Visual Studio 2022 for Mac](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio-mac/?sku=communitymac&rel=17) или [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- База данных:
  - [PostgreSQL 15+](https://www.postgresql.org/download/) или Docker образ
  - PostgreSQL поддерживает процессоры Intel и Apple Silicon

### Linux
- [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- База данных:
  - [PostgreSQL 15+](https://www.postgresql.org/download/) или Docker образ 



 ## **Начало работы**
### Шаги: 
1. Установите базу данных [PostgreSQL 15+](https://www.postgresql.org/download/) или разверните базу данных с помощью docker
2. Установите переменные окружения в appSettings.json и appSettings.Development.json
3. Выполните миграции EF CORE или разверните резервную копию базы данных
4. Соберите и запустите проект (используйте команды dotnet или IDE ([Visual Studio 2022 / Visual Studio for Mac](https://visualstudio.microsoft.com/downloads/) или [Microsoft VS Code](https://visualstudio.microsoft.com/downloads/))


## **Развертывание баз данных**
Возможные сценарии развертывания базы данных:
+ использовать PostgreSQL в облаке (AWS RDS, Azure Database for PostgreSQL, Google Cloud SQL и т.д.)
+ использовать docker или docker-compose
+ развернуть локальную базу данных PostgreSQL

### Использование Docker или Docker compose (рекомендуемый метод)
Запуск базы данных с использованием docker:

```
sudo docker run -e "POSTGRES_DB=TaskForge" -e "POSTGRES_USER=postgres" -e "POSTGRES_PASSWORD=postgres" \
   -p 5432:5432 --name postgres-taskforge \
   -d \
   postgres:16-alpine
```


Запуск базы данных с использованием docker-compose:
В проекте уже включены файлы `docker-compose.yml` и `docker-compose.override.yml`, настроенные для PostgreSQL.

Для процессоров Intel / Amd (x86/x64):
```
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

Для процессоров Apple Silicon (M1/M2/M3) - ARM:
```
docker-compose -f docker-compose.yml -f docker-compose.override.arm.yml up -d
```

После запуска будут доступны следующие сервисы:
- **PostgreSQL**: `localhost:5432`
- **pgAdmin**: `http://localhost:5050` (учетные данные по умолчанию: `admin@pgadmin.org` / `admin`)
- **API**: `http://localhost:5009`
- **EventProcessor**: `http://localhost:5010`
- **Client**: `http://localhost:3000`

### **Клиентское приложение**

React клиентское приложение включено в настройку Docker Compose. После запуска всех сервисов:

- **Client**: Доступен по адресу `http://localhost:3000` (настраивается через переменную окружения `CLIENT_PORT`)
- **API**: Доступен по адресу `http://localhost:5009/api`
- **EventProcessor**: Доступен по адресу `http://localhost:5010`
- **База данных**: PostgreSQL на `localhost:5432`
- **pgAdmin**: Доступен по адресу `http://localhost:5050` (настраивается через переменную окружения `PGADMIN_PORT`)
  - Email по умолчанию: `admin@pgadmin.org` (стандартный pgAdmin по умолчанию)
  - Пароль по умолчанию: `admin` (настраивается через переменную окружения `PGADMIN_PASSWORD`)

#### **Подключение к PostgreSQL через pgAdmin**

pgAdmin предварительно настроен с подключением к базе данных TaskForge. Просто:

1. Откройте pgAdmin по адресу `http://localhost:5050`
2. Войдите с учетными данными:
   - Email: `admin@pgadmin.org` (или ваш пользовательский email через переменную окружения `PGADMIN_EMAIL`)
   - Пароль: `admin` (или ваш пользовательский пароль через переменную окружения `PGADMIN_PASSWORD`)
3. Вы должны увидеть сервер **"TaskForge DB"**, уже настроенный в разделе "Servers"
4. Нажмите на "TaskForge DB", чтобы развернуть и получить доступ к базе данных

Если сервер не виден, вы можете добавить его вручную:
1. Правой кнопкой мыши на "Servers" → "Create" → "Server"
2. На вкладке "General":
   - Имя: `TaskForge DB`
3. На вкладке "Connection":
   - Имя хоста/адрес: `postgres.data`
   - Порт: `5432`
   - База данных обслуживания: `TaskForge`
   - Имя пользователя: `postgres`
   - Пароль: `postgres`
   - Установите флажок "Save password"
4. Нажмите "Save"

Теперь вы можете просматривать базу данных, просматривать таблицы, выполнять запросы и отслеживать активность базы данных.

#### Конфигурация клиента

Клиент использует переменные окружения для конфигурации:

- `REACT_APP_API_URL` - базовый URL API (по умолчанию `/api` в Docker, использует nginx proxy)
- `CLIENT_PORT` - порт клиентского сервиса (по умолчанию `3000`)

В Docker клиент автоматически проксирует запросы API через nginx к серверному сервису.

Для получения дополнительной информации см. `src/client-app/README-DOCKER.md`.

### **Развертывание локальной базы данных PostgreSQL (альтернативный метод)**

Установите PostgreSQL локально на вашем компьютере:

**Windows:**
- Скачайте и установите с [PostgreSQL Downloads](https://www.postgresql.org/download/windows/)
- Или используйте менеджер пакетов: `choco install postgresql` (с Chocolatey)

**Mac:**
- Скачайте и установите с [PostgreSQL Downloads](https://www.postgresql.org/download/macosx/)
- Или используйте Homebrew: `brew install postgresql@16`

**Linux:**
- Ubuntu/Debian: `sudo apt-get install postgresql-16`
- CentOS/RHEL: `sudo yum install postgresql-server`
- Или скачайте с [PostgreSQL Downloads](https://www.postgresql.org/download/linux/)

После установки создайте базу данных:
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
- **[Docker Platforms Explanation](/docs/DOCKER_PLATFORMS_EXPLANATION.md)** - объяснение Docker платформ
- **[Задание 2: SQL Функция (EN)](/src/database/postgres/README.md)** - руководство по SQL функции для анализа платежей (английский)
- **[Задание 2: SQL Функция (RU)](/src/database/postgres/README-RU.md)** - руководство по SQL функции для анализа платежей (русский)

### Просмотр диаграмм Mermaid

В проекте используются диаграммы C4 Model в формате `.mermaid`, расположенные в папке [`docs/diagrams/`](/docs/diagrams/).

Для просмотра диаграмм `.mermaid` в Visual Studio Code рекомендуется использовать одно из следующих расширений:

- **[Markdown Preview Mermaid Support](https://marketplace.visualstudio.com/items?itemName=bierner.markdown-mermaid)** - позволяет просматривать диаграммы Mermaid прямо в предпросмотре Markdown файлов
- **[Mermaid Preview](https://marketplace.visualstudio.com/items?itemName=vstirbu.vscode-mermaid-preview)** - предварительный просмотр файлов `.mermaid` с поддержкой экспорта в SVG/PNG
- **[Mermaid Editor](https://marketplace.visualstudio.com/items?itemName=TomoyukiAota.vscode-mermaid-editor)** - редактор диаграмм Mermaid с поддержкой live preview

После установки расширения откройте файл `.mermaid` и используйте предпросмотр (`Ctrl+Shift+V` / `Cmd+Shift+V`) для просмотра диаграммы.

## Форматы коммитов
#### Типы
* Изменения, относящиеся к API
    * `feat` Коммиты, которые добавляют новую функцию
    * `fix` Коммиты, которые исправляют ошибку
* `refactor` Коммиты, которые переписывают/реструктурируют ваш код, но не меняют поведение
    * `perf` Коммиты - это специальные коммиты `refactor`, которые улучшают производительность
* `style` Коммиты, которые не влияют на значение (пробелы, форматирование, отсутствие точек с запятой и т.д.)
* `test` Коммиты, которые добавляют отсутствующие тесты или исправляют существующие тесты
* `docs` Коммиты, которые затрагивают только документацию
* `build` Коммиты, которые затрагивают компоненты сборки, такие как инструмент сборки, CI pipeline, зависимости, версия проекта, ...
* `devops` Коммиты, которые затрагивают операционные компоненты, такие как инфраструктура, развертывание, резервное копирование, восстановление, ...
* `chore` Разные коммиты, например, изменение `.gitignore`

#### Тема
* использовать императив, настоящее время (например: использовать "add" вместо "added" или "adds")
* не использовать точку (.) в конце
* не использовать заглавную букву в начале

### Примеры
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


## Сборка и запуск приложений
Перед запуском обязательно установите переменные в файлах конфигурации appsettings.json. Важно указать правильную строку подключения к базе данных

**Для PostgreSQL:**
```
 "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TaskForge;Username=postgres;Password=postgres"
  },
```

**Для окружения Docker** (строка подключения автоматически устанавливается через переменные окружения):
```
Host=postgres.data;Port=5432;Database=TaskForge;Username=postgres;Password=postgres
```
Можно использовать команды в терминале или использовать IDE (Microsoft Visual Studio 2022 или VS Code):
```
dotnet build [options]
dotnet run [options]
```


