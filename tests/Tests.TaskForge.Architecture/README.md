# Architecture Tests

## Описание

Этот проект содержит архитектурные тесты, которые проверяют:
- **Зависимости между слоями** (Clean Architecture)
- **Наследование** (правильное использование базовых классов и интерфейсов)
- **Конвенции именования** (контроллеры, handlers, validators, интерфейсы)
- **XML документацию** (наличие Summary для публичных API)
- **Ссылки между сборками** (правильная архитектура зависимостей)
- **Консистентность** (соответствие паттернам и конвенциям)

## Используемые библиотеки

- **NetArchTest.Rules** - для проверки архитектурных правил
- **FluentAssertions** - для улучшенных assertions
- **xUnit** - тестовый фреймворк

## Категории тестов

### 1. ArchitectureTests.cs
- Проверка зависимостей между слоями
- Проверка наследования
- Проверка именования классов
- Проверка структуры handlers и commands/queries

### 2. NamingConventionTests.cs
- Private fields должны начинаться с `_`
- Public properties/methods используют PascalCase
- Async методы заканчиваются на `Async`
- Интерфейсы начинаются с `I` + заглавная буква
- Handlers называются `Handler`
- Validators заканчиваются на `Validator`
- DTOs заканчиваются на `Dto`

### 3. ConsistencyTests.cs
- Handlers имеют соответствующие Command/Query
- Интерфейсы имеют реализации
- Event модели имеют согласованные свойства
- Result<T> является immutable
- Controllers используют HandleResult

### 4. XmlDocumentationTests.cs
- Public классы имеют XML Summary
- Public интерфейсы имеют XML Summary
- Public методы в controllers имеют XML Summary
- Public properties имеют XML Summary

## Запуск тестов

```bash
# Запустить все архитектурные тесты
dotnet test tests/Tests.TaskForge.Architecture

# Запустить конкретный файл тестов
dotnet test tests/Tests.TaskForge.Architecture --filter "FullyQualifiedName~ArchitectureTests"

# Запустить с подробным выводом
dotnet test tests/Tests.TaskForge.Architecture --verbosity normal
```

## Добавление в solution

```bash
dotnet sln add tests/Tests.TaskForge.Architecture/Tests.TaskForge.Architecture.csproj
```

## Что проверяется

### Зависимости слоев:
- ✅ Domain не зависит от других TaskForge сборок
- ✅ Application не зависит от API/Persistence напрямую
- ✅ API не зависит напрямую от Persistence
- ✅ Persistence зависит только от Domain

### Наследование:
- ✅ Controllers наследуются от BaseApiController или ControllerBase
- ✅ Handlers реализуют IRequestHandler<TRequest, TResponse>
- ✅ Validators наследуются от AbstractValidator<T>
- ✅ Workers наследуются от BackgroundService
- ✅ Consumers реализуют IConsumer<T>

### Именование:
- ✅ Private fields начинаются с `_`
- ✅ Public методы/свойства используют PascalCase
- ✅ Async методы заканчиваются на `Async`
- ✅ Интерфейсы начинаются с `I` + заглавная
- ✅ Controllers заканчиваются на `Controller`
- ✅ Handlers называются `Handler`

### Консистентность:
- ✅ Все Handlers возвращают Result<T>
- ✅ Все Commands имеют CommandValidator
- ✅ Интерфейсы имеют реализации
- ✅ Event модели согласованы между сервисами

## Примеры использования

Тесты автоматически запускаются в CI/CD pipeline и помогают поддерживать архитектурную целостность проекта.

