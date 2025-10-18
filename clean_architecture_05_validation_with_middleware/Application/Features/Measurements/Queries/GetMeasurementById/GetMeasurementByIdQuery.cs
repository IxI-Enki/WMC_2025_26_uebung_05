using Application.Dtos;
using MediatR;

namespace Application.Features.Measurements.Queries.GetMeasurementById;

public readonly record struct GetMeasurementByIdQuery(int Id) : IRequest<GetMeasurementDto?>;
