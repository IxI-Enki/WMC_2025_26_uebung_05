using Application.Dtos;
using MediatR;

namespace Application.Features.Measurements.Queries.GetLast100Measurements;

/// <summary>
/// Query zum Abrufen der 100 neuesten Messwerte.
/// </summary>
public record GetLast100MeasurementsQuery : IRequest<IReadOnlyCollection<GetMeasurementDto>>;
