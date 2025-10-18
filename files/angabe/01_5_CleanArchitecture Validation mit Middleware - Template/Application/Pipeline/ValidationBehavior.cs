using FluentValidation;
using MediatR;

namespace Application.Pipeline;

/// <summary>
/// F�hrt alle FluentValidation-Validatoren f�r eine Anfrage aus, bevor der Handler aufgerufen wird.
/// Ziel: Einheitliches Fehlerverhalten (400 Bad Request) �ber die Middleware.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, 
                    cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToList();
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next(cancellationToken);

        //try
        //{
        //    return await next(cancellationToken);
        //}
        //catch (DomainValidationException ex)
        //{
        //    // Map domain validation to FluentValidation for unified handling in middleware
        //    var failures = new List<ValidationFailure> { new(string.Empty, ex.Message) };
        //    throw new ValidationException(failures);
        //}
    }
}
