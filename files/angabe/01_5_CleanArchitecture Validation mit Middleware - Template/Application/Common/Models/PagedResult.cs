namespace Application.Common.Models;


/// <summary>
/// Technisches Strukturmuster.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="Items"></param>
/// <param name="TotalCount"></param>
/// <param name="Page"></param>
/// <param name="PageSize"></param>
public record PagedResult<T>(IReadOnlyCollection<T> Items, int TotalCount, int Page, int PageSize);

