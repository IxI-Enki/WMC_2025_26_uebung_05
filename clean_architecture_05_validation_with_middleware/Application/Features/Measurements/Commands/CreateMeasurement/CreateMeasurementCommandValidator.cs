using FluentValidation;

namespace Application.Features.Measurements.Commands.CreateMeasurement;

public class CreateMeasurementCommandValidator : AbstractValidator<CreateMeasurementCommand>
{
    public CreateMeasurementCommandValidator()
    {
        RuleFor(x => x.SensorId)
            .GreaterThan(0)
            .WithMessage("SensorId muss größer als 0 sein.");

        RuleFor(x => x.Value)
            .InclusiveBetween(-100, 100)
            .WithMessage("Wert muss zwischen -100 und 100 liegen.");

        RuleFor(x => x.Timestamp)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Zeitstempel darf nicht in der Zukunft liegen.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddHours(-1))
            .WithMessage("Zeitstempel muss innerhalb der letzten Stunde liegen.");
    }
}
