using Application.Interfaces;
using MediatR;
using Mapster;
using Application.Dtos;

namespace Application.Features.Sensors.Queries.GetSensorsWithCounts;

public sealed class GetSensorsWithCountsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetSensorsWithCountsQuery, IReadOnlyCollection<GetSensorWithNumberOfMeasurementsDto>>
{
    public async Task<IReadOnlyCollection<GetSensorWithNumberOfMeasurementsDto>> Handle(GetSensorsWithCountsQuery request, CancellationToken cancellationToken)
    {
        var records = await uow.Sensors.GetAllWithNumberOfMeasurementsAsync(cancellationToken);
        return records.Adapt<IReadOnlyCollection<GetSensorWithNumberOfMeasurementsDto>>();
    }
}
