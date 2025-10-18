using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository für Messungen inkl. typischer Leseabfragen und Paging.
/// </summary>
public class MeasurementRepository(AppDbContext dbContext) : GenericRepository<Measurement>(dbContext), IMeasurementRepository
{

    /// <summary>
    /// Lädt alle Messungen read-only.
    /// </summary>
    public override async Task<IReadOnlyCollection<Measurement>> GetAllAsync(
        CancellationToken ct = default,
        Func<IQueryable<Measurement>, IOrderedQueryable<Measurement>>? orderBy = null,
        Expression<Func<Measurement, bool>>? filter = null)
    {
        // Timeout nur für diese Query anpassen
        _dbContext.Database.SetCommandTimeout(180); // 3 Minuten
        try
        {
            return await _dbContext.Measurements
                .Where(filter ?? (_ => true))
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync(ct);
        }
        finally
        {
            // Nach der Query wieder zurücksetzen → null = Standard (meist 30s oder globaler Wert)
            _dbContext.Database.SetCommandTimeout(null);
        }
    }

    /// <summary>
    /// Holt Messungen für einen Sensor anhand Location+Name, absteigend nach Zeit.
    /// </summary>
    public async Task<IReadOnlyCollection<Measurement>> GetBySensorAsync(string location, string name, CancellationToken ct = default)
        => await _dbContext.Measurements.AsNoTracking()
            .Include(m => m.Sensor)
            .Where(m => m.Sensor.Location == location && m.Sensor.Name == name)
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync(ct);

    /// <summary>
    /// Anzahl der Messungen eines Sensors.
    /// </summary>
    public async Task<int> CountBySensorIdAsync(int sensorId, CancellationToken ct = default)
        => await _dbContext.Measurements.AsNoTracking().Where(m => m.SensorId == sensorId).CountAsync(ct);

    /// <summary>
    /// Paginierte Messungen zu einem Sensor, absteigend nach Zeit.
    /// </summary>
    public async Task<IReadOnlyCollection<Measurement>> GetBySensorIdPagedAsync(int sensorId, int skip, int take, CancellationToken ct = default)
        => await _dbContext.Measurements.AsNoTracking()
            .Where(m => m.SensorId == sensorId)
            .OrderByDescending(m => m.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
}
