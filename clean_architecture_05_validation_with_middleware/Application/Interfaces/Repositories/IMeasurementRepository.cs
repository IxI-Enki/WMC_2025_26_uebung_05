using Domain.Entities;

namespace Application.Interfaces.Repositories;

/// <summary>
/// Abfragen f�r Messungen inkl. Paging und Z�hlen.
/// </summary>
public interface IMeasurementRepository : IGenericRepository<Measurement>
{
      Task<IReadOnlyCollection<Measurement>> GetBySensorAsync( string location , string name , CancellationToken ct = default );
      Task<int> CountBySensorIdAsync( int sensorId , CancellationToken ct = default );
      Task<IReadOnlyCollection<Measurement>> GetBySensorIdPagedAsync( int sensorId , int skip , int take , CancellationToken ct = default );
      Task<IReadOnlyCollection<Measurement>> GetLast100MeasurementsAsync( CancellationToken ct = default );
}
