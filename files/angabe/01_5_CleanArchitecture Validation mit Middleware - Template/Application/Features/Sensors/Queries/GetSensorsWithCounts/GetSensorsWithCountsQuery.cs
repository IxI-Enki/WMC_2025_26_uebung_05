using Application.Dtos;
using MediatR;

namespace Application.Features.Sensors.Queries.GetSensorsWithCounts;

public record GetSensorsWithCountsQuery : IRequest<IReadOnlyCollection<GetSensorWithNumberOfMeasurementsDto>>;
