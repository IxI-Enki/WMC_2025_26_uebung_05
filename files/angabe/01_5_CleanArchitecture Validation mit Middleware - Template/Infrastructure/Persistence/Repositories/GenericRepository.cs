using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Generische Repository-Implementierung f�r CRUD-Operationen.
/// Nutzt EF Core DbSet<T> f�r Datenzugriffe.
/// </summary>
public class GenericRepository<T>(AppDbContext dbContext) : IGenericRepository<T> where T : class, IBaseEntity
{
    protected readonly AppDbContext _dbContext = dbContext;
    protected readonly DbSet<T> _set = dbContext.Set<T>();

    /// <summary>
    /// Holt eine Entit�t per Prim�rschl�ssel.
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync([id], ct);

    /// <summary>
    /// Holt alle Entit�ten (optional gefiltert und sortiert). NoTracking f�r read-only.
    /// </summary>
    public virtual async Task<IReadOnlyCollection<T>> GetAllAsync(
        CancellationToken ct = default,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _set.AsNoTracking();
        if (filter is not null)
            query = query.Where(filter);
        if (orderBy is not null)
            query = orderBy(query);
        return await query.ToListAsync(ct);
    }

    /// <summary>
    /// F�gt eine neue Entit�t hinzu (noch nicht gespeichert).
    /// </summary>
    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    /// <summary>
    /// F�gt mehrere neue Entit�ten hinzu (noch nicht gespeichert).
    /// </summary>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
        => await _set.AddRangeAsync(entities, ct);

    /// <summary>
    /// Markiert eine Entit�t als ge�ndert.
    /// </summary>
    public virtual void Update(T entity) => _set.Update(entity);

    /// <summary>
    /// Entfernt eine Entit�t.
    /// </summary>
    public virtual void Remove(T entity) => _set.Remove(entity);

    /// <summary>
    /// Entfernt mehrere Entit�ten.
    /// </summary>
    public virtual void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
}
