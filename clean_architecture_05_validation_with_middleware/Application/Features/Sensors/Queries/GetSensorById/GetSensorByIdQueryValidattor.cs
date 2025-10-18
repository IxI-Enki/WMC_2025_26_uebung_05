using FluentValidation;

namespace Application.Features.Sensors.Queries.GetSensorById;

public class GetSensorByIdQueryValidator : AbstractValidator<GetSensorByIdQuery>
{
      public GetSensorByIdQueryValidator( )
      {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id muss größer als 0 sein.");
      }
}
