using Domain.Contracts;
using Domain.Exceptions;

namespace Domain.Specifications;

/// <summary>
/// Sensor Specifications - enthält NUR Domain-spezifische Validierungen.
/// Format-Validierungen (nicht leer, Mindestlänge, etc.) erfolgen im Application Layer via FluentValidation.
/// </summary>
public static class SensorSpecifications
{
      /// <summary>
      /// Prüft die Eindeutigkeit eines Sensors (Business-Regel).
      /// Diese Validierung kann nur im Domain Layer erfolgen, da sie Datenbank-Zugriff erfordert.
      /// </summary>
      public static async Task ValidateEntityExternal( int id , string location , string name , ISensorUniquenessChecker uniquenessChecker , CancellationToken ct = default )
      {
            if(!await uniquenessChecker.IsUniqueAsync( id , location , name , ct ))
            {
                  throw new DomainValidationException( "Sensor" , "Ein Sensor mit der gleichen Location und dem gleichen Namen existiert bereits." );
            }
      }
}
