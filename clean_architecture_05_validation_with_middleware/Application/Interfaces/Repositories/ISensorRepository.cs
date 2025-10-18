using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Sensor-spezifische Abfragen zus�tzlich zu den generischen CRUDs.
/// </summary>
public interface ISensorRepository : IGenericRepository<Sensor>
{
      Task<IReadOnlyCollection<GetSensorWithNumberOfMeasurementsDto>> GetAllWithNumberOfMeasurementsAsync( CancellationToken ct );
      Task<Sensor?> GetByLocationAndNameAsync( string location , string name , CancellationToken ct = default );
}
