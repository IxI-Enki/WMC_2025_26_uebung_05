using Domain.Common;
using Domain.Contracts;
using Domain.Exceptions;

namespace Domain.Specifications;

/// <summary>
/// Sensor Specifications - enthält Domain-spezifische Validierungen.
/// </summary>
public static class SensorSpecifications
{
      public const int NameMinLength = 2;

      /// <summary>
      /// Prüft, ob Location nicht leer ist.
      /// </summary>
      public static DomainValidationResult CheckLocation( string location ) =>
          string.IsNullOrWhiteSpace( location )
              ? DomainValidationResult.Failure( "Location" , "Location darf nicht leer sein." )
              : DomainValidationResult.Success( "Location" );

      /// <summary>
      /// Prüft, ob Name nicht leer ist und Mindestlänge erfüllt.
      /// </summary>
      public static DomainValidationResult CheckName( string name )
      {
            if(string.IsNullOrWhiteSpace( name ))
            {
                  return DomainValidationResult.Failure( "Name" , "Name darf nicht leer sein." );
            }
            if(name.Trim( ).Length < NameMinLength)
            {
                  return DomainValidationResult.Failure( "Name" , $"Name muss mindestens {NameMinLength} Zeichen haben." );
            }
            return DomainValidationResult.Success( "Name" );
      }

      /// <summary>
      /// Prüft, ob Name nicht der Location entspricht.
      /// </summary>
      public static DomainValidationResult CheckNameNotEqualLocation( string name , string location ) =>
          string.Equals( name.Trim( ) , location.Trim( ) , StringComparison.OrdinalIgnoreCase )
              ? DomainValidationResult.Failure( "Name" , "Name darf nicht der Location entsprechen." )
              : DomainValidationResult.Success( "Name" );

      /// <summary>
      /// Führt alle internen Validierungen aus (ohne Datenbank-Zugriff).
      /// Wirft DomainValidationException beim ersten Fehler.
      /// </summary>
      public static void ValidateEntityInternal( string location , string name )
      {
            var validationResults = new List<DomainValidationResult>
            {
                  CheckLocation( location ),
                  CheckName( name ),
                  CheckNameNotEqualLocation( name , location )
            };
            foreach(var result in validationResults)
            {
                  if(!result.IsValid)
                  {
                        throw new DomainValidationException( result.Property , result.ErrorMessage! );
                  }
            }
      }

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
