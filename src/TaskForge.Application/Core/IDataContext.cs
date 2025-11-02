using Microsoft.EntityFrameworkCore;
using TaskForge.Domain;

namespace TaskForge.Application.Core;

/// <summary>
/// Abstraction for database context to follow Clean Architecture principles.
/// Application layer should not depend on Persistence layer directly.
/// </summary>
public interface IDataContext
{
    DbSet<TaskItem> TaskItems { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    void Add<TEntity>(TEntity entity) where TEntity : class;
    
    void Update<TEntity>(TEntity entity) where TEntity : class;
    
    void Remove<TEntity>(TEntity entity) where TEntity : class;
    
    Task<TEntity?> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken = default) where TEntity : class;
}

