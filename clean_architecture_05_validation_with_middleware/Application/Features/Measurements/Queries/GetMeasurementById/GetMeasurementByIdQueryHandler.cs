using Application.Dtos;
using Application.Interfaces;
using Mapster;
using MediatR;

namespace Application.Features.Measurements.Queries.GetMeasurementById;

public sealed class GetMeasurementByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMeasurementByIdQuery, GetMeasurementDto?>
{
    public async Task<GetMeasurementDto?> Handle(GetMeasurementByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await uow.Measurements.GetByIdAsync(request.Id, cancellationToken);
        return entity?.Adapt<GetMeasurementDto>();
    }
}
