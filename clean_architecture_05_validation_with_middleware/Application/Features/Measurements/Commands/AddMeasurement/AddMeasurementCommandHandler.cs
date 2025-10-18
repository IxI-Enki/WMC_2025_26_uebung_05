using Application.Common.Models;
using Application.Dtos;
using Application.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Measurements.Commands.AddMeasurement;

/// <summary>
/// Handler für AddMeasurementCommand.
/// Erstellt bei Bedarf einen neuen Sensor und fügt dann die Messung hinzu.
/// </summary>
public sealed class AddMeasurementCommandHandler(IUnitOfWork uow, ISensorUniquenessChecker uniquenessChecker)
    : IRequestHandler<AddMeasurementCommand, Result<GetMeasurementDto>>
{
    public async Task<Result<GetMeasurementDto>> Handle(AddMeasurementCommand request,
            CancellationToken cancellationToken)
    {
        // Sensor per Location und Name suchen
        var sensor = await uow.Sensors.GetByLocationAndNameAsync(
            request.Location.Trim(),
            request.Name.Trim(),
            cancellationToken);

        // Falls Sensor nicht existiert, neuen Sensor erstellen
        if (sensor is null)
        {
            // Sensor.CreateAsync führt Domain-Validierung durch (Uniqueness Check)
            sensor = await Sensor.CreateAsync(
                request.Location,
                request.Name,
                uniquenessChecker,
                cancellationToken);

            await uow.Sensors.AddAsync(sensor, cancellationToken);

            // SaveChanges VOR Measurement, damit Sensor.Id für FK verfügbar ist
            await uow.SaveChangesAsync(cancellationToken);
        }

        // Measurement erstellen (Domain-Validierung erfolgt im Konstruktor)
        var measurement = new Measurement(sensor, request.Value, request.Timestamp);
        await uow.Measurements.AddAsync(measurement, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<GetMeasurementDto>.Created(measurement.Adapt<GetMeasurementDto>());
    }
}
