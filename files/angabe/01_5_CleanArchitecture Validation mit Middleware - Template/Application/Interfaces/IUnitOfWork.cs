using Application.Interfaces.Repositories;

namespace Application.Interfaces;

/// <summary>
/// Aggregiert Repositories und speichert �nderungen. Sicherer Umgang mit Transaktionen.
/// </summary>
public interface IUnitOfWork
{
    ISensorRepository Sensors { get; }
    IMeasurementRepository Measurements { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
