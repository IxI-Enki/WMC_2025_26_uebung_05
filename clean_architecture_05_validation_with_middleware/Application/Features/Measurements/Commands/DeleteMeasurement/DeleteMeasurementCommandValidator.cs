using FluentValidation;

namespace Application.Features.Measurements.Commands.DeleteMeasurement;

public class DeleteMeasurementCommandValidator : AbstractValidator<DeleteMeasurementCommand>
{
    public DeleteMeasurementCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id muss größer als 0 sein.");
    }
}
