using Application.Dtos;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Spezifisches Repository f�r Sensoren mit zus�tzlichen Abfragen.
/// </summary>
public class SensorRepository(AppDbContext ctx) : GenericRepository<Sensor>(ctx), ISensorRepository
{
    public async Task<IReadOnlyCollection<GetSensorWithNumberOfMeasurementsDto>> GetAllWithNumberOfMeasurementsAsync(CancellationToken ct)
    {
        var result = await _dbContext.Sensors
            .AsNoTracking()
            .OrderBy(s => s.Location).ThenBy(s => s.Name)
            .Select(s => new GetSensorWithNumberOfMeasurementsDto(
                s.Id,
                s.Location,
                s.Name,
                s.Measurements.Count))
            .ToListAsync(ct);
        return result;
    }

    /// <summary>
    /// Findet einen Sensor anhand von Standort und Name.
    /// </summary>
    public async Task<Sensor?> GetByLocationAndNameAsync(string location, string name, 
            CancellationToken ct = default)
        => await _dbContext.Sensors.FirstOrDefaultAsync(s => s.Location == location && s.Name == name, ct);

    ///// <summary>
    ///// �berschreibt GetAllAsync, um die Sensoren konsistent sortiert zur�ckzugeben.
    ///// </summary>
    //public override async Task<IReadOnlyCollection<Sensor>> GetAllAsync(
    //    CancellationToken ct = default,
    //    Func<IQueryable<Sensor>, IOrderedQueryable<Sensor>>? orderBy = null,
    //    Expression<Func<Sensor, bool>>? filter = null)
    //    => await (orderBy is null
    //            ? _dbContext.Sensors.AsNoTracking().WhereIf(filter).OrderBy(s => s.Location).ThenBy(s => s.Name)
    //            : orderBy(_dbContext.Sensors.AsNoTracking().WhereIf(filter)))
    //        .ToListAsync(ct);
}

