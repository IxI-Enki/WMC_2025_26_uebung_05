using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Specifications
{
      /// <summary>
      /// Measurement Specifications - enthält NUR Domain-spezifische Validierungen.
      /// Format-Validierungen (Value-Bereich, Timestamp-Regeln) erfolgen im Application Layer via FluentValidation.
      /// </summary>
      public static class MeasurementSpecifications
      {
            /// <summary>
            /// Prüft die kritische Domain-Invariante: Eine Messung muss immer einem Sensor zugeordnet sein.
            /// Dies ist eine fundamentale Business-Regel, die IMMER gelten muss.
            /// </summary>
            public static void ValidateEntityInternal( Sensor sensor , double value , DateTime timestamp , CancellationToken ct = default )
            {
                  if(sensor is null)
                  {
                        throw new DomainValidationException( nameof( sensor ) , "Sensor darf nicht null sein." );
                  }
                  // Value und Timestamp werden im Application Layer validiert (FluentValidation)
            }
      }
}
