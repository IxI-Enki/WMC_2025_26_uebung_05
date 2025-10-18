using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Basisvertrag f�r alle Entit�ten in der Dom�nenschicht.
/// </summary>
public interface IBaseEntity
{
    /// <summary>
    /// Prim�rschl�ssel. Wird von EF Core gesetzt.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Concurrency-Token zur Absicherung gegen gleichzeitige Updates (SQL Server rowversion/timestamp).
    /// EF Core nutzt dieses Feld, um Optimistic Concurrency zu unterst�tzen.
    /// </summary>
    [Timestamp]
    byte[] RowVersion { get; set; }
}
