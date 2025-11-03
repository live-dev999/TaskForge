# Tests.TaskForge.Core

Проект содержит общий код для unit и integration тестов, который используется во всех тестовых проектах для устранения дублирования кода.

## Структура проекта

```
Tests.TaskForge.Core/
├── Helpers/           # Вспомогательные классы для создания тестовых объектов
│   ├── DatabaseTestHelper.cs           # Создание in-memory DbContext
│   ├── TestDataFactory.cs              # Фабрика тестовых данных (TaskItem и др.)
│   ├── LoggerTestHelper.cs             # Создание mock логгеров
│   ├── MapperTestHelper.cs             # Создание AutoMapper
│   ├── MediatorTestHelper.cs           # Создание mock IMediator
│   ├── HttpContextTestHelper.cs        # Создание HttpContext для тестов
│   ├── CancellationTokenTestHelper.cs  # Создание CancellationToken
│   └── EnvironmentTestHelper.cs       # Создание mock IHostEnvironment
└── Fixtures/         # Базовые классы для тестовых fixtures
    ├── BaseTestFixture.cs              # Базовая fixture для всех тестов
    ├── BaseHandlerTestFixture.cs       # Fixture для handler тестов
    └── BaseControllerTestFixture.cs   # Fixture для controller тестов
```

## Использование

### 1. В Handler тестах

**До рефакторинга:**
```csharp
public class CreateHandlerTests
{
    private DataContext CreateInMemoryContext() { /* ... */ }
    private TaskItem CreateValidTaskItem() { /* ... */ }
    private ILogger<Create.Handler> CreateLogger() { /* ... */ }
    // ... тесты
}
```

**После рефакторинга:**
```csharp
public class CreateHandlerTests : BaseHandlerTestFixture
{
    [Fact]
    public async Task Handle_WhenValidTaskItem_ReturnsSuccess()
    {
        // Arrange
        var logger = CreateLogger<Create.Handler>();
        var handler = new Create.Handler(Context, logger);
        var command = new Create.Command
        {
            TaskItem = CreateValidTaskItem()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
```

### 2. В Controller тестах

**До рефакторинга:**
```csharp
public class TaskItemsControllerTests
{
    private TaskItemsController CreateController(IMediator mediator) { /* ... */ }
    private TaskItem CreateValidTaskItem() { /* ... */ }
    // ... тесты
}
```

**После рефакторинга:**
```csharp
public class TaskItemsControllerTests : BaseControllerTestFixture
{
    [Fact]
    public async Task GetTaskItems_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        var mockMediator = MediatorTestHelper.CreateMediatorMock<List.Query, List<TaskItem>>(
            new List<TaskItem> { CreateValidTaskItem() });
        var controller = CreateController<TaskItemsController>(mockMediator.Object);

        // Act
        var actionResult = await controller.GetTaskItems(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
    }
}
```

### 3. Использование Helpers напрямую

```csharp
// Создание тестовых данных
var taskItem = TestDataFactory.CreateValidTaskItem();
var taskItemWithCustomId = TestDataFactory.CreateTaskItemWithId(Guid.NewGuid());
var multipleItems = TestDataFactory.CreateTaskItems(10, (item, index) => 
    item.Title = $"Task {index}");

// Создание in-memory контекста
using var context = DatabaseTestHelper.CreateInMemoryContext();

// Создание логгера
var logger = LoggerTestHelper.CreateLogger<MyHandler>();

// Создание mapper
var mapper = MapperTestHelper.CreateMapper();

// Создание cancelled token
var cancelledToken = CancellationTokenTestHelper.CreateCancelledToken();
```

## Добавление в проект

1. Добавьте ссылку на `Tests.TaskForge.Core` в ваш тестовый проект:

```xml
<ItemGroup>
  <ProjectReference Include="..\Tests.TaskForge.Core\Tests.TaskForge.Core.csproj" />
</ItemGroup>
```

2. Добавьте using директивы:

```csharp
using Tests.TaskForge.Core.Fixtures;
using Tests.TaskForge.Core.Helpers;
```

## Преимущества

✅ **Устранение дублирования кода** - общие helper методы вынесены в один проект  
✅ **Единообразие** - все тесты используют одинаковые подходы к созданию тестовых объектов  
✅ **Легкость поддержки** - изменения в helper методах применяются ко всем тестам  
✅ **Улучшенная читаемость** - тесты становятся более лаконичными  
✅ **Переиспользование** - легко добавлять новые helper методы для всех тестов  

