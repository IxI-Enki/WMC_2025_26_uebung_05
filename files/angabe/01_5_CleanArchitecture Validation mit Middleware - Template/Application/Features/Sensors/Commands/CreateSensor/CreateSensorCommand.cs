using Application.Common.Models;
using Application.Dtos;
using MediatR;

namespace Application.Features.Sensors.Commands.CreateSensor;

// Validation �ber Exception ==> Resulttype ist Dto
public readonly record struct CreateSensorCommand(string Location, string Name) : IRequest<Result<GetSensorDto>>;
