using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Measurements.Commands.DeleteMeasurement;

/// <summary>
/// Command-Handler zum Löschen einer Messung.
/// </summary>
public sealed class DeleteMeasurementCommandHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteMeasurementCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteMeasurementCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.Measurements.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException($"Messung mit Id {request.Id} wurde nicht gefunden.");
        }

        uow.Measurements.Remove(entity);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
