using Application.Common.Models;
using Application.Dtos;
using Application.Interfaces;
using Domain.Contracts;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Features.Sensors.Commands.CreateSensor;

/// <summary>
/// Command-Handler zum Erstellen eines neuen Sensors.
/// Dom�nenvalidierungen erfolgen in der Entit�t (Factory-Methode) und werden ggf. bis zur API geschleust.
/// </summary>
public sealed class CreateSensorCommandHandler(IUnitOfWork uow, ISensorUniquenessChecker uniquenessChecker) 
    : IRequestHandler<CreateSensorCommand, Result<GetSensorDto>>
{
    public async Task<Result<GetSensorDto>> Handle(CreateSensorCommand request, 
            CancellationToken cancellationToken)
    {
        // Sensor �ber Dom�nenlogik erstellen und persistieren
        var entity = await Sensor.CreateAsync(request.Location, request.Name, 
                uniquenessChecker, cancellationToken);
        await uow.Sensors.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetSensorDto>.Created(entity.Adapt<GetSensorDto>());
    }
}
