using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence;

/// <summary>
/// Unit of Work aggregiert Repositories und speichert Änderungen transaktional.
/// </summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _dbContext;
    private bool _disposed;

    /// <summary>
    /// Zugriff auf Sensor-Repository.
    /// </summary>
    public ISensorRepository Sensors { get; }

    /// <summary>
    /// Zugriff auf Measurement-Repository.
    /// </summary>
    public IMeasurementRepository Measurements { get; }

    public UnitOfWork(AppDbContext dbContext, ISensorRepository sensors, IMeasurementRepository measurements)
    {
        _dbContext = dbContext;
        Sensors = sensors;
        Measurements = measurements;
    }

    /// <summary>
    /// Persistiert alle Änderungen in die DB. Gibt die Anzahl der betroffenen Zeilen zurück.
    /// </summary>
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _dbContext.SaveChangesAsync(ct);

    /// <summary>
    /// Gibt verwaltete Ressourcen frei. Der DbContext gehört zum Scope dieser UoW und wird hier entsorgt.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _dbContext.Dispose();
        }
        _disposed = true;
    }
}
