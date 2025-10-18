using Application.Dtos;
using Application.Interfaces;
using Mapster;
using MediatR;

namespace Application.Features.Measurements.Queries.GetLast100Measurements;

/// <summary>
/// Handler für GetLast100MeasurementsQuery.
/// Liefert die 100 neuesten Messwerte sortiert nach Timestamp absteigend.
/// </summary>
public sealed class GetLast100MeasurementsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetLast100MeasurementsQuery, IReadOnlyCollection<GetMeasurementDto>>
{
    public async Task<IReadOnlyCollection<GetMeasurementDto>> Handle(GetLast100MeasurementsQuery request,
        CancellationToken cancellationToken)
    {
        // GetAllAsync mit OrderBy: neueste zuerst (Timestamp DESC), dann Take(100)
        var entities = await uow.Measurements.GetAllAsync(
            cancellationToken,
            orderBy: query => query.OrderByDescending(m => m.Timestamp).Take(100));

        return entities.Adapt<IReadOnlyCollection<GetMeasurementDto>>();
    }
}
