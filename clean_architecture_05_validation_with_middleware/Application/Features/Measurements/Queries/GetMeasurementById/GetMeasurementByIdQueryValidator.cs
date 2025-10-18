using FluentValidation;

namespace Application.Features.Measurements.Queries.GetMeasurementById;

public class GetMeasurementByIdQueryValidator : AbstractValidator<GetMeasurementByIdQuery>
{
    public GetMeasurementByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id muss größer als 0 sein.");
    }
}
