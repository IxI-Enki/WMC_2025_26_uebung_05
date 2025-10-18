using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Einzelne Messung eines Sensors mit Wert und Zeitstempel.
/// </summary>
public class Measurement : BaseEntity
{
    /// <summary>
    /// Fremdschlüssel auf den Sensor.
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

    private Measurement() { } // Für EF Core

    /// <summary>
    /// Erstellt eine neue Messung für einen Sensor.
    /// </summary>
    public Measurement(Sensor sensor, double value, DateTime timestamp)
    {
        Sensor = sensor;
        SensorId = sensor.Id;
        Value = value;
        Timestamp = timestamp;
    }
}
