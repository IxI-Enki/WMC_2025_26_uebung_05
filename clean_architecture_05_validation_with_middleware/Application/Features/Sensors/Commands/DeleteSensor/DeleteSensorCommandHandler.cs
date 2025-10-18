using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Sensors.Commands.DeleteSensor;

/// <summary>
/// Command-Handler zum L�schen eines Sensors.
/// Gibt bei Erfolg NoContent zur�ck, ansonsten NotFound.
/// </summary>
public sealed class DeleteSensorCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteSensorCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteSensorCommand request, CancellationToken cancellationToken)
    {
        // Sensor laden
        var entity = await uow.Sensors.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Sensor with Id {request.Id} not found.");
        //return Result<bool>.NotFound($"Sensor with Id {request.Id} not found.");

        // Sensor entfernen und �nderungen speichern
        uow.Sensors.Remove(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<bool>.NoContent(true);
    }
}
