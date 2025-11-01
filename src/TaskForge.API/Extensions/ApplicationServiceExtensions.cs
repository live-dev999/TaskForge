/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.

 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Core;
using TaskForge.Application.TaskItems;
using MediatR;
using FluentValidation.AspNetCore;
using FluentValidation;

namespace TaskForge.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<Persistence.DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors(opt =>
        {
            // Get allowed origins from configuration
            // Priority: 1) appsettings.json "Cors:AllowedOrigins" array
            //           2) Environment variable "CORS_ALLOWED_ORIGINS" (semicolon-separated, e.g., "http://localhost:3000;http://localhost:3001")
            //           3) Default: "http://localhost:3000"
            var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();
            
            // Fallback to environment variable if not in config (useful for Docker)
            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                var corsOriginsEnv = config["CORS_ALLOWED_ORIGINS"];
                if (!string.IsNullOrEmpty(corsOriginsEnv))
                {
                    // Split by semicolon to support multiple origins
                    allowedOrigins = corsOriginsEnv.Split(';', StringSplitOptions.RemoveEmptyEntries);
                }
            }
            
            // Default to localhost:3000 if nothing is configured
            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                allowedOrigins = new[] { "http://localhost:3000" };
            }
            
            opt.AddPolicy(
                "CorsPolicy",
                policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins(allowedOrigins);
                }
            );
        });

        services.AddMediatR(typeof(List.Handler));
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Create>();
        
        // Add Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<Persistence.DataContext>(
                name: "database",
                tags: new[] { "ready" });
        
        return services;
    }
}
