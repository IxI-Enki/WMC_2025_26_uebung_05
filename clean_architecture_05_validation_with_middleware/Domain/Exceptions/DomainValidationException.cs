namespace Domain.Exceptions;

/// <summary>
/// Ausnahme f�r Verletzungen von Domain-Invarianten / Validierungsregeln innerhalb des Domain-Modells.
/// </summary>
public class DomainValidationException( string property , string message ) : Exception( message )
{
      public string Property { get; } = property;
}
