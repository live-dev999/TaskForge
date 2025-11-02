# Руководство по конфигурации архитектурных тестов

## 📋 Обзор

Все жестко закодированные строки (например, "TaskForge") были вынесены в конфигурацию `TestConfiguration` для обеспечения переиспользования тестов в других проектах.

## ⚙️ Конфигурация

### TestConfiguration

Класс `TestConfiguration` содержит все настраиваемые параметры:

```csharp
public class TestConfiguration
{
    // Основные настройки
    public string ProjectPrefix { get; set; } = "TaskForge";  // Префикс проекта
    public List<string> LayerNames { get; set; } = new() { ... };  // Имена слоев
    
    // Метод для построения полного имени сборки
    public string GetAssemblyName(string layerName)
    {
        return $"{ProjectPrefix}.{layerName}";
    }
}
```

## 🔧 Использование в тестах

### Базовый класс

Все тесты, наследующиеся от `ArchitectureTestBase`, получают доступ к `Configuration`:

```csharp
public class MyTests : ArchitectureTestBase
{
    protected override Dictionary<string, Assembly> GetAssemblies()
    {
        return new Dictionary<string, Assembly>
        {
            { "API", typeof(YourProject.API.Controllers.HomeController).Assembly },
            { "Application", typeof(YourProject.Application.Services.MyService).Assembly }
        };
    }

    [Fact]
    public void My_Test()
    {
        // Используйте Configuration.ProjectPrefix вместо "TaskForge"
        var projectPrefix = Configuration.ProjectPrefix;  // "YourProject"
        
        // Используйте GetAssemblyName() для построения полных имен
        var apiAssemblyName = GetAssemblyName("API");  // "YourProject.API"
        
        // Используйте Configuration.LayerNames для списка слоев
        var allLayers = Configuration.LayerNames;
    }
}
```

### Пример: Проверка зависимостей

**До (с жестко закодированными строками):**
```csharp
var result = Types
    .InAssembly(domainAssembly)
    .ShouldNot()
    .HaveDependencyOn("TaskForge.Application")
    .And()
    .ShouldNot()
    .HaveDependencyOn("TaskForge.API")
    // ...
```

**После (с использованием конфигурации):**
```csharp
var forbiddenDependencies = Configuration.LayerNames
    .Where(layer => layer != "Domain")
    .Select(layer => GetAssemblyName(layer))  // Использует Configuration.ProjectPrefix
    .ToArray();

var result = Types
    .InAssembly(domainAssembly)
    .ShouldNot()
    .HaveDependencyOnAny(forbiddenDependencies);
```

## 🎯 Настройка для другого проекта

### Шаг 1: Измените ProjectPrefix

В вашем тесте можно переопределить конфигурацию:

```csharp
public class YourProjectTests : ArchitectureTestBase
{
    protected override Dictionary<string, Assembly> GetAssemblies()
    {
        // Конфигурация будет использована автоматически
        return new Dictionary<string, Assembly> { /* ... */ };
    }

    protected override void Configure()
    {
        // Настройте для вашего проекта
        Configuration.ProjectPrefix = "YourProject";
        Configuration.LayerNames = new List<string>
        {
            "API",
            "Application", 
            "Domain",
            "Infrastructure"
        };
    }
}
```

Или инициализируйте в конструкторе:

```csharp
public class YourProjectTests : ArchitectureTestBase
{
    public YourProjectTests()
    {
        Configuration.ProjectPrefix = "YourProject";
        Configuration.LayerNames = new List<string> { /* ваши слои */ };
    }
}
```

### Шаг 2: Используйте GetAssemblyName()

Вместо жестко закодированных строк используйте метод:

```csharp
// ❌ Плохо
.HaveDependencyOn("TaskForge.Application")

// ✅ Хорошо
.HaveDependencyOn(GetAssemblyName("Application"))  // Автоматически использует ProjectPrefix
```

### Шаг 3: Используйте LayerNames

Для динамического построения списков:

```csharp
// Все слои, кроме текущего
var otherLayers = Configuration.LayerNames
    .Where(layer => layer != currentLayer)
    .Select(layer => GetAssemblyName(layer))
    .ToArray();
```

## 📝 Примеры использования

### Пример 1: Проверка зависимостей слоев

```csharp
[Fact]
public void Domain_Should_Not_Depend_On_Other_Layers()
{
    var domainAssembly = GetAssembly("Domain");
    
    // Используем конфигурацию для динамического построения списка
    var forbiddenDependencies = Configuration.LayerNames
        .Where(layer => layer != "Domain")
        .Select(layer => GetAssemblyName(layer))
        .ToArray();
    
    var result = Types
        .InAssembly(domainAssembly)
        .ShouldNot()
        .HaveDependencyOnAny(forbiddenDependencies)
        .GetResult();

    result.IsSuccessful.Should().BeTrue();
}
```

### Пример 2: Проверка всех слоев

```csharp
[Fact]
public void All_Layers_Should_Follow_Naming_Conventions()
{
    foreach (var layerName in Configuration.LayerNames)
    {
        var assembly = GetAssembly(layerName);
        var fullName = GetAssemblyName(layerName);
        
        // Используйте fullName для проверок
        // ...
    }
}
```

## ✅ Преимущества

1. **Переиспользуемость** - легко адаптировать для другого проекта
2. **Централизованная настройка** - все строки в одном месте
3. **Типобезопасность** - используйте методы вместо строковых литералов
4. **Гибкость** - легко добавлять новые слои через конфигурацию

## 🔄 Миграция существующих тестов

Если у вас есть старые тесты с жестко закодированными "TaskForge":

1. Найдите все использования `"TaskForge.` в коде
2. Замените на `GetAssemblyName("LayerName")`
3. Используйте `Configuration.LayerNames` для списков слоев
4. Обновите `ProjectPrefix` в конфигурации для нового проекта

## 📚 См. также

- [README_REFACTORING.md](README_REFACTORING.md) - Общая информация о рефакторинге
- [Examples/ExampleProjectTests.cs](Examples/ExampleProjectTests.cs) - Примеры использования

