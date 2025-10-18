using Application.Dtos;
using Application.Interfaces;
using Mapster;
using MediatR;

namespace Application.Features.Sensors.Queries.GetAllSensors;

public sealed class GetAllSensorsQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAllSensorsQuery, 
    IReadOnlyCollection<GetSensorDto>>
{
    public async Task<IReadOnlyCollection<GetSensorDto>> Handle(GetAllSensorsQuery request, 
        CancellationToken cancellationToken)
    {
        var sensors = await uow.Sensors.GetAllAsync(cancellationToken,
            orderBy: q => q.OrderBy(s => s.Location).ThenBy(s => s.Name));
        return sensors.Adapt<IReadOnlyCollection<GetSensorDto>>();
    }
}
