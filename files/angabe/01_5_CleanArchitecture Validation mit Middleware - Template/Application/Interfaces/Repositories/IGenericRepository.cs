using Domain.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Generisches Repository-Interface f�r einfache CRUD-Operationen.
/// </summary>
public interface IGenericRepository<T> where T : class, IBaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Liefert alle Entit�ten optional gefiltert und sortiert.
    /// </summary>
    /// <param name="ct">Abbruch-Token.</param>
    /// <param name="orderBy">Optionale Sortierung, z.B. q => q.OrderBy(x => x.Id).</param>
    /// <param name="filter">Optionales Filter-Kriterium, z.B. x => x.Id > 0.</param>
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken ct = default,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, bool>>? filter = null);

    Task AddAsync(T entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
