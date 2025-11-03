/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 *
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
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskForge.Domain;
using TaskForge.Domain.Enum;

namespace TaskForge.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для сущности TaskItem
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        // Имя таблицы
        builder.ToTable("TaskItems");

        // Первичный ключ
        builder.HasKey(t => t.Id);

        // Конфигурация Id
        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .IsRequired();

        // Конфигурация Title
        builder.Property(t => t.Title)
            .HasColumnName("Title")
            .HasMaxLength(500) // Ограничение длины для оптимизации
            .IsRequired();

        // Конфигурация Description
        builder.Property(t => t.Description)
            .HasColumnName("Description")
            .HasMaxLength(2000) // Ограничение длины для оптимизации
            .IsRequired(false); // Может быть null

        // Конфигурация Status (enum)
        builder.Property(t => t.Status)
            .HasColumnName("Status")
            .IsRequired()
            .HasConversion<int>(); // Сохраняется как int в БД

        // Конфигурация CreatedAt
        builder.Property(t => t.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired()
            .HasColumnType("timestamp");

        // Конфигурация UpdatedAt
        builder.Property(t => t.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .IsRequired()
            .HasColumnType("timestamp");

        // Индексы для оптимизации запросов
        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_TaskItems_Status");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_TaskItems_CreatedAt");
    }
}

