using Application.Dtos;
using Application.Interfaces;
using Mapster;
using MediatR;

namespace Application.Features.Sensors.Queries.GetSensorById;

public sealed class GetSensorByIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetSensorByIdQuery, GetSensorDto?>
{
    public async Task<GetSensorDto?> Handle(GetSensorByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await uow.Sensors.GetByIdAsync(request.Id, cancellationToken);
        return entity?.Adapt<GetSensorDto>();
    }
}
