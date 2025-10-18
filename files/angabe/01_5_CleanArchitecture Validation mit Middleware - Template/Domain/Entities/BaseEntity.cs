using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Abstrakte Basisklasse f�r Entit�ten mit Id und Concurrency-Token.
/// </summary>
public abstract class BaseEntity : IBaseEntity
{
    /// <summary>
    /// Prim�rschl�ssel (Identity). Protected set, damit nur EF/abgeleitete Klassen setzen k�nnen.
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Concurrency-Token (RowVersion). EF setzt diesen Wert bei jeder �nderung.
    /// Dient zur Erkennung konkurrierender Updates.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
