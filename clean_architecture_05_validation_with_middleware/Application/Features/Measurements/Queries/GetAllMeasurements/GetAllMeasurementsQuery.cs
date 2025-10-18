using Application.Dtos;
using MediatR;

namespace Application.Features.Measurements.Queries.GetAllMeasurements;

public record GetAllMeasurementsQuery : IRequest<IReadOnlyCollection<GetMeasurementDto>>;
