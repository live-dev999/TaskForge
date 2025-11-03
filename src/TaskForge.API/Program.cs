/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.

 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

using Microsoft.EntityFrameworkCore;
using TaskForge.API.Extensions;
using TaskForge.API.Middleware;
using TaskForge.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

// Map Health Check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // Live endpoint - just checks if service is running
});

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    var context = services.GetRequiredService<DataContext>();
    
    // Check if we should drop and recreate the database on migration conflict
    var dropOnConflict = builder.Configuration.GetValue<bool>("Database:DropOnMigrationConflict", false);
    
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex) when (IsTableAlreadyExistsError(ex))
    {
        // Table already exists - this happens when old table exists but migration history is missing
        if (dropOnConflict || app.Environment.IsDevelopment())
        {
            logger.LogWarning("Table 'TaskItems' already exists. Dropping it to apply new migration...");
            await context.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS \"TaskItems\" CASCADE;");
            
            // Remove old migration history entries if they exist
            await context.Database.ExecuteSqlRawAsync("DELETE FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" != '20251103134328_InitialCreate';");
            
            // Retry migration
            await context.Database.MigrateAsync();
            logger.LogInformation("Database table recreated successfully.");
        }
        else
        {
            logger.LogError("Migration failed: Table 'TaskItems' already exists. " +
                          "Set 'Database:DropOnMigrationConflict' to 'true' in appsettings.json " +
                          "or manually drop the table using: DROP TABLE IF EXISTS \"TaskItems\" CASCADE;");
            throw;
        }
    }
    
    // Seed database
    try
    {
        await Seed.SeedData(context, logger);
    }
    catch (Exception seedEx)
    {
        logger.LogError(seedEx, "An error occurred during database seeding: {ErrorMessage}", seedEx.Message);
        // Don't throw - allow application to continue even if seed fails
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured during migration");
}
app.Run();

// Helper method to check if exception is a "table already exists" error
static bool IsTableAlreadyExistsError(Exception ex)
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

// Make Program class accessible for integration tests
public partial class Program { }
