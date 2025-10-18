using FluentValidation;

namespace Application.Features.Sensors.Commands.CreateSensor;

public class CreateSensorCommandValidator : AbstractValidator<CreateSensorCommand>
{
      public CreateSensorCommandValidator( )
      {
            RuleFor(x => x.Location)
                .NotEmpty()
                .WithMessage("Location darf nicht leer sein.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name darf nicht leer sein.")
                .MinimumLength(2)
                .WithMessage("Name muss mindestens 2 Zeichen haben.")
                .Must((cmd, name) => !string.Equals(name, cmd.Location, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Name darf nicht der Location entsprechen.");
      }
}
