using FluentValidation;

namespace Application.Features.Measurements.Commands.AddMeasurement;

/// <summary>
/// Validator für AddMeasurementCommand.
/// Prüft Use-Case spezifische Regeln (Application-Layer Validierung).
/// </summary>
public class AddMeasurementCommandValidator : AbstractValidator<AddMeasurementCommand>
{
    public AddMeasurementCommandValidator()
    {
        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location darf nicht leer sein.")
            .MaximumLength(100)
            .WithMessage("Location darf maximal 100 Zeichen lang sein.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name darf nicht leer sein.")
            .MinimumLength(2)
            .WithMessage("Name muss mindestens 2 Zeichen lang sein.")
            .MaximumLength(100)
            .WithMessage("Name darf maximal 100 Zeichen lang sein.");

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
