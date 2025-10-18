using Application.Dtos;
using Application.Interfaces;
using Mapster;
using MediatR;

namespace Application.Features.Measurements.Queries.GetAllMeasurements;

public sealed class GetAllMeasurementsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllMeasurementsQuery, IReadOnlyCollection<GetMeasurementDto>>
{
    public async Task<IReadOnlyCollection<GetMeasurementDto>> Handle(GetAllMeasurementsQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await uow.Measurements.GetAllAsync(cancellationToken);
        return entities.Adapt<IReadOnlyCollection<GetMeasurementDto>>();
    }
}
