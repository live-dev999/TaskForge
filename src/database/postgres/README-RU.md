# Задание 2: SQL Функция для анализа платежей

**Language / Язык:** [English](README.md) | [Русский](README-RU.md)

Эта директория содержит PostgreSQL скрипты и Docker настройку для Задания 2 - SQL Функция для анализа платежей.

## Обзор

Данная настройка автоматически:
1. Создает базу данных PostgreSQL
2. Создает таблицу `ClientPayments`
3. Создает функцию `GetDailyPayments`
4. Вставляет тестовые данные
5. Запускает тесты
6. Проверяет результаты

## Быстрый старт

### Вариант 1: Использование Docker Compose (Рекомендуется)

```bash
cd src/database/postgres
docker-compose up --build
```

Это выполнит:
- Запустит сервер PostgreSQL
- Дождется его готовности
- Автоматически выполнит все SQL скрипты
- Покажет результаты тестов
- Завершит работу после выполнения

### Вариант 2: Использование Docker напрямую

Сначала запустите PostgreSQL:
```bash
docker run -d \
  --name postgres-task2 \
  -e POSTGRES_DB=taskforgedb \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5433:5432 \
  postgres:16-alpine
```

Дождитесь готовности PostgreSQL, затем выполните скрипты:
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

### Вариант 3: Ручное выполнение

Если у вас установлен PostgreSQL локально:

```bash
cd src/database/postgres
export PGPASSWORD=postgres
./run-all.sh
```

## Порядок выполнения скриптов

1. **01-create-database.sql** - Создание базы данных
2. **02-create-table.sql** - Создание таблицы ClientPayments с индексами
3. **03-create-function.sql** - Создание функции GetDailyPayments
4. **04-insert-seed-data.sql** - Вставка тестовых данных
5. **05-run-tests.sql** - Запуск тестовых запросов
6. **06-verify-results.sql** - Сравнение фактических и ожидаемых результатов

## Сигнатура функции

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

## Ожидаемые результаты

### Пример 1: ClientId=1, 2022-01-02 до 2022-01-07
| Dt         | Amount |
|------------|--------|
| 2022-01-02 | 0      |
| 2022-01-03 | 100    |
| 2022-01-04 | 0      |
| 2022-01-05 | 450    |
| 2022-01-06 | 0      |
| 2022-01-07 | 50     |

### Пример 2: ClientId=2, 2022-01-04 до 2022-01-11
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

## Очистка

Для удаления контейнеров и томов:
```bash
docker-compose down -v
```

## Примечания

- Функция использует `generate_series` PostgreSQL для генерации дат
- Все дни в диапазоне включены (начальная и конечная даты включительно)
- Дни без платежей возвращают 0, а не NULL
- Функция обрабатывает диапазоны дат, охватывающие несколько лет

