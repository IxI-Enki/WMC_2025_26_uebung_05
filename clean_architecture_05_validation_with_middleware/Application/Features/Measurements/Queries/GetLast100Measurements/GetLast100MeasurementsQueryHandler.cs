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
        // Repository-Methode liefert die 100 neuesten Messwerte (Timestamp DESC)
        var entities = await uow.Measurements.GetLast100MeasurementsAsync(cancellationToken);

        return entities.Adapt<IReadOnlyCollection<GetMeasurementDto>>();
    }
}
