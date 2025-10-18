using FluentValidation;

namespace Application.Features.Sensors.Commands.DeleteSensor;

public class DeleteSensorCommandValidator : AbstractValidator<DeleteSensorCommand>
{
      public DeleteSensorCommandValidator( )
      {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id muss größer als 0 sein.");
      }
}
