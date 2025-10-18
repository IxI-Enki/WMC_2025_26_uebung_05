using Application.Dtos;
using MediatR;

namespace Application.Features.Sensors.Queries.GetAllSensors;

public record GetAllSensorsQuery : IRequest<IReadOnlyCollection<GetSensorDto>>;
