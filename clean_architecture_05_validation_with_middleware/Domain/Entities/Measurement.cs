using Domain.Specifications;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Domain.Entities;

/// <summary>
/// Einzelne Messung eines Sensors mit Wert und Zeitstempel.
/// </summary>
public class Measurement : BaseEntity
{
      /// <summary>
      /// Fremdschl�ssel auf den Sensor.
      /// </summary>
      public int SensorId { get; private set; }

      /// <summary>
      /// Navigation zum Sensor. Wird von EF geladen, wenn Include verwendet wird.
      /// </summary>
      public Sensor Sensor { get; private set; } = null!;

      /// <summary>
      /// Gemessener Wert (z.B. Temperatur als double).
      /// </summary>
      public double Value { get; private set; }

      /// <summary>
      /// Zeitpunkt der Messung.
      /// </summary>
      public DateTime Timestamp { get; private set; }

      private Measurement( ) { } // F�r EF Core

      /// <summary>
      /// Erstellt eine neue Messung f�r einen Sensor.
      /// </summary>
      public Measurement( Sensor sensor , double value , DateTime timestamp )
      {
            MeasurementSpecifications.ValidateEntityInternal( sensor , value , timestamp );

            Sensor = sensor;
            SensorId = sensor.Id;
            Value = value;
            Timestamp = timestamp;
      }
}
