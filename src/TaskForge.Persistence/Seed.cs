
using TaskForge.Domain;

namespace TaskForge.Persistence
{
    public class Seed
    {
        #region Methods

        public static async Task SeedData(DataContext context)
        {
            if (context.TaskItems.Any()) return;

            var taskItems = new List<TaskItem>
            {
                new TaskItem
                {
                    Title = "Завершить проект API",
                    Description = "Завершить разработку REST API для системы управления задачами",
                    Status = Domain.Enum.TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new TaskItem
                {
                    Title = "Написать unit-тесты",
                    Description = "Создать unit-тесты для сервисов и контроллеров",
                    Status = Domain.Enum.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new TaskItem
                {
                    Title = "Code review",
                    Description = "Провести ревью кода для pull request #42",
                    Status = Domain.Enum.TaskStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new TaskItem
                {
                    Title = "Обновить документацию",
                    Description = "Обновить README и добавить примеры использования API",
                    Status = Domain.Enum.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new TaskItem
                {
                    Title = "Исправить баг с авторизацией",
                    Description = "Исправить проблему с истечением токена авторизации",
                    Status = Domain.Enum.TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new TaskItem
                {
                    Title = "Оптимизация базы данных",
                    Description = "Добавить индексы для улучшения производительности запросов",
                    Status = Domain.Enum.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddHours(-12),
                    UpdatedAt = DateTime.UtcNow.AddHours(-12)
                },
                new TaskItem
                {
                    Title = "Интеграция с внешним API",
                    Description = "Интегрировать систему с сервисом отправки уведомлений",
                    Status = Domain.Enum.TaskStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new TaskItem
                {
                    Title = "Рефакторинг сервисов",
                    Description = "Провести рефакторинг сервисного слоя для улучшения читаемости кода",
                    Status = Domain.Enum.TaskStatus.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    UpdatedAt = DateTime.UtcNow.AddHours(-3)
                },
                new TaskItem
                {
                    Title = "Деплой на production",
                    Description = "Выполнить деплой последней версии приложения на production сервер",
                    Status = Domain.Enum.TaskStatus.Pending,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    UpdatedAt = DateTime.UtcNow.AddDays(-6)
                },
                new TaskItem
                {
                    Title = "Настройка мониторинга",
                    Description = "Настроить мониторинг производительности и логирования",
                    Status = Domain.Enum.TaskStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    UpdatedAt = DateTime.UtcNow.AddDays(-4)
                }
            };

            await context.TaskItems.AddRangeAsync(taskItems);
            await context.SaveChangesAsync();
        }
    }

    #endregion
}
