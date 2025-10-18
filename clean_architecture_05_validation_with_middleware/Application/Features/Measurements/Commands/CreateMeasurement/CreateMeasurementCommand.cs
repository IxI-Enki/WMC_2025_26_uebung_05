using Application.Common.Models;
using Application.Dtos;
using MediatR;

namespace Application.Features.Measurements.Commands.CreateMeasurement;

/// <summary>
/// Command zum Erstellen einer neuen Messung für einen Sensor.
/// </summary>
public readonly record struct CreateMeasurementCommand(int SensorId, double Value, DateTime Timestamp)
    : IRequest<Result<GetMeasurementDto>>;
