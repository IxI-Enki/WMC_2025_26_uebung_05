using Application.Common.Models;
using Application.Dtos;
using MediatR;

namespace Application.Features.Measurements.Commands.AddMeasurement;

/// <summary>
/// Command zum Hinzufügen einer Messung mit automatischer Sensor-Erstellung.
/// </summary>
/// <remarks>
/// Falls ein Sensor mit der angegebenen Location und Name noch nicht existiert,
/// wird er automatisch angelegt.
/// </remarks>
public readonly record struct AddMeasurementCommand(
    string Location,
    string Name,
    DateTime Timestamp,
    double Value)
    : IRequest<Result<GetMeasurementDto>>;
