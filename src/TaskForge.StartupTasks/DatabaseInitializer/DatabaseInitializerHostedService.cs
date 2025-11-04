/*
 *   Copyright (c) 2024 Dzianis Prokharchyk
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TaskForge.StartupTasks.DatabaseInitializer;

public class DatabaseInitializerHostedService<TDbContext> : IHostedService
    where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DatabaseInitializerSettings _settings;
    private readonly ILogger<DatabaseInitializerHostedService<TDbContext>> _logger;
    private readonly IHostEnvironment _environment;

    public DatabaseInitializerHostedService(
        IServiceScopeFactory scopeFactory,
        DatabaseInitializerSettings settings,
        ILogger<DatabaseInitializerHostedService<TDbContext>> logger,
        IHostEnvironment environment
    )
    {
        _scopeFactory = scopeFactory;
        _settings = settings;
        _logger = logger;
        _environment = environment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_settings.Initialize)
        {
            _logger.LogDebug("Database initialization is disabled. Skipping migration and seed.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        try
        {
            // Run database migration
            await RunMigrationAsync(dbContext, cancellationToken).ConfigureAwait(false);

            // Seed database if enabled
            if (_settings.Seed)
            {
                await SeedDatabaseAsync(dbContext, scope.ServiceProvider, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database initialization");
            throw;
        }
    }

    private async Task RunMigrationAsync(TDbContext dbContext, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting database migration...");
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Database migration completed successfully.");
        }
        catch (Exception ex) when (IsTableAlreadyExistsError(ex))
        {
            // Table already exists - this happens when old table exists but migration history is missing
            if (_settings.DropOnMigrationConflict || _environment.IsDevelopment())
            {
                _logger.LogWarning("Table 'TaskItems' already exists. Dropping it to apply new migration...");
                
                await dbContext.Database.ExecuteSqlRawAsync(
                    "DROP TABLE IF EXISTS \"TaskItems\" CASCADE;", 
                    cancellationToken).ConfigureAwait(false);
                
                // Remove old migration history entries if they exist
                await dbContext.Database.ExecuteSqlRawAsync(
                    "DELETE FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" != '20251103134328_InitialCreate';",
                    cancellationToken).ConfigureAwait(false);
                
                // Retry migration
                await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Database table recreated successfully.");
            }
            else
            {
                _logger.LogError("Migration failed: Table 'TaskItems' already exists. " +
                              "Set 'DatabaseInitializer:DropOnMigrationConflict' to 'true' in appsettings.json " +
                              "or manually drop the table using: DROP TABLE IF EXISTS \"TaskItems\" CASCADE;");
                throw;
            }
        }
    }

    private async Task SeedDatabaseAsync(TDbContext dbContext, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting database seed...");
            
            // Try to find Seed.SeedData method via reflection if it exists
            // This allows for flexible seeding implementation
            var seedType = typeof(TDbContext).Assembly.GetType("TaskForge.Persistence.Seed");
            if (seedType != null)
            {
                var seedMethod = seedType.GetMethod("SeedData", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                if (seedMethod != null)
                {
                    var parameters = seedMethod.GetParameters();
                    if (parameters.Length >= 1 && parameters[0].ParameterType == typeof(TDbContext))
                    {
                        // Call Seed.SeedData(context, logger)
                        // Seed.SeedData accepts ILogger (base interface), so we can pass _logger
                        var seedTask = seedMethod.Invoke(null, new object[] { dbContext, _logger }) as Task;
                        if (seedTask != null)
                        {
                            await seedTask.ConfigureAwait(false);
                            _logger.LogInformation("Database seed completed successfully.");
                            return;
                        }
                    }
                }
            }
            
            _logger.LogWarning("Seed method not found. Skipping database seed.");
        }
        catch (Exception seedEx)
        {
            _logger.LogError(seedEx, "An error occurred during database seeding: {ErrorMessage}", seedEx.Message);
            // Don't throw - allow application to continue even if seed fails
        }
    }

    private static bool IsTableAlreadyExistsError(Exception ex)
    {
        // Check if it's a PostgresException with the specific error code
        if (ex is Npgsql.PostgresException pgEx && pgEx.SqlState == "42P07")
            return true;
        
        // Check inner exceptions recursively
        Exception current = ex.InnerException;
        while (current != null)
        {
            if (current is Npgsql.PostgresException innerPgEx && innerPgEx.SqlState == "42P07")
                return true;
            current = current.InnerException;
        }
        
        return false;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
