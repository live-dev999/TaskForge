// Вместо монолита - специализированные сервисы
services.AddScoped<ITaskService, TaskService>();
services.AddScoped<INotificationService, NotificationService>();
services.AddScoped<IAnalyticsService, AnalyticsService>();