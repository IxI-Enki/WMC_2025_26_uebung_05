using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Dtos;
using Application.Interfaces;
using Domain.Contracts;
using Domain.Exceptions;
using Mapster;
using MediatR;

namespace Application.Features.Sensors.Commands.UpdateSensor;

/// <summary>
/// Command-Handler zum Aktualisieren eines vorhandenen Sensors.
/// Nutzt Domänenlogik für Validierungen und gibt ein Result für eine saubere API-Antwort zurück.
/// </summary>
public sealed class UpdateSensorCommandHandler(IUnitOfWork uow, ISensorUniquenessChecker uniquenessChecker) 
    : IRequestHandler<UpdateSensorCommand, Result<GetSensorDto>>
{
    //// Kommunikation mit API mittels Exceptions
    //public async Task<GetSensorDto> Handle(UpdateSensorCommand request, CancellationToken cancellationToken)
    //{
    //    var entity = await uow.Sensors.GetByIdAsync(request.Id, cancellationToken) ?? throw new DomainValidationNotFoundException($"Sensor with id {request.Id} not found.");

    //    // Update via domain method (added on Sensor)
    //    entity.Update(request.Location, request.Name);

    //    uow.Sensors.Update(entity);
    //    await uow.SaveChangesAsync(cancellationToken);

    //    return entity.Adapt<GetSensorDto>();
    //}

    // Kommunikation mittels Result-Objekt  ==> Exceptions abfangen und in Result umwandeln
    public async Task<Result<GetSensorDto>> Handle(UpdateSensorCommand request, CancellationToken cancellationToken)
    {
        // Sensor laden
        var entity = await uow.Sensors.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null) throw new NotFoundException($"Sensor with ID {request.Id} not found.");
        // return Result<GetSensorDto>.NotFound($"Sensor with ID {request.Id} not found.");

        // Update via domain method (added on Sensor)
        // Eindeutigkeit (Location, Name) wird über ISensorUniquenessChecker geprüft
        await entity.UpdateAsync(request.Location, request.Name, uniquenessChecker, cancellationToken);
        uow.Sensors.Update(entity);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<GetSensorDto>.Success(entity.Adapt<GetSensorDto>());

    }
}
