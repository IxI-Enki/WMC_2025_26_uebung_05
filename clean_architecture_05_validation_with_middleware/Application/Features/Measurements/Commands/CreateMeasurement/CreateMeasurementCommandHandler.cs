using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Measurements.Commands.CreateMeasurement;

/// <summary>
/// Command-Handler zum Erstellen einer neuen Messung.
/// Domänenvalidierungen erfolgen in der Entität und werden ggf. bis zur API geschleust.
/// </summary>
public sealed class CreateMeasurementCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateMeasurementCommand, Result<GetMeasurementDto>>
{
    public async Task<Result<GetMeasurementDto>> Handle(CreateMeasurementCommand request,
            CancellationToken cancellationToken)
    {
        // Sensor muss existieren
        var sensor = await uow.Sensors.GetByIdAsync(request.SensorId, cancellationToken);
        if (sensor is null)
        {
            throw new NotFoundException($"Sensor mit Id {request.SensorId} wurde nicht gefunden.");
        }

        // Measurement über Domänenlogik erstellen und persistieren
        var entity = new Measurement(sensor, request.Value, request.Timestamp);
        await uow.Measurements.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<GetMeasurementDto>.Created(entity.Adapt<GetMeasurementDto>());
    }
}
