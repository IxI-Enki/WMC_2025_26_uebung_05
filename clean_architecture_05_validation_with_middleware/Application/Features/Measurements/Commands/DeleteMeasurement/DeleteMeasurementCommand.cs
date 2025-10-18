using Application.Common.Models;
using MediatR;

namespace Application.Features.Measurements.Commands.DeleteMeasurement;

public readonly record struct DeleteMeasurementCommand(int Id) : IRequest<Result<bool>>;
