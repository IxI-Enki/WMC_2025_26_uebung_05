using Domain.Contracts;
using Domain.Exceptions;
using Domain.Specifications;

namespace Domain.Entities;

/// <summary>
/// Repr�sentiert einen physischen Sensor an einem Standort.
/// </summary>
public class Sensor : BaseEntity
{
    /// <summary>
    /// Standort/Ort des Sensors, z.B. "Wohnzimmer".
    /// </summary>
    public string Location { get; private set; } = string.Empty;

    /// <summary>
    /// Anzeigename des Sensors, z.B. "Temperatur".
    /// </summary>  
    public string Name { get; private set; } = string.Empty;

    public ICollection<Measurement> Measurements { get; private set; } = default!;

    private Sensor() { } // F�r EF Core notwendig (parameterloser Konstruktor)

    /// <summary>
    /// Erzeugt einen neuen Sensor.
    /// Im Fehlerfall wird beim ersten Fehler eine DomainValidationException geworfen.
    /// </summary>
    private Sensor(string location, string name)
    {
        SensorSpecifications.ValidateEntityInternal(location, name);
        Location = location;
        Name = name;
    }


    /// <summary>
    /// Asynchronously creates a new <see cref="Sensor"/> instance with the specified location and name.
    /// </summary>
    /// <remarks>This method validates the provided location and name both internally and externally using the
    /// specified <paramref name="uniquenessChecker"/>. The resulting <see cref="Sensor"/> instance is initialized with
    /// the trimmed values of the location and name.</remarks>
    /// <param name="location">The location of the sensor. Cannot be null or empty, and leading or trailing whitespace will be trimmed.</param>
    /// <param name="name">The name of the sensor. Cannot be null or empty, and leading or trailing whitespace will be trimmed.</param>
    /// <param name="uniquenessChecker">An implementation of <see cref="ISensorUniquenessChecker"/> used to ensure the sensor's uniqueness.</param>
    /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the newly created <see
    /// cref="Sensor"/> instance.</returns>
    public static async Task<Sensor> CreateAsync(string location, string name, ISensorUniquenessChecker uniquenessChecker, CancellationToken ct = default)
    {
        var trimmedLocation = (location ?? string.Empty).Trim();
        var trimmedName = (name ?? string.Empty).Trim();
        SensorSpecifications.ValidateEntityInternal(trimmedLocation, trimmedName);
        await SensorSpecifications.ValidateEntityExternal(0, trimmedLocation, trimmedName, uniquenessChecker, ct);
        return new Sensor(trimmedLocation, trimmedName);
    }

    /// <summary>
    /// Aktualisiert die Eigenschaften des Sensors.
    /// </summary>
    public async Task UpdateAsync(string location, string name, ISensorUniquenessChecker uniquenessChecker, CancellationToken ct = default)
    {
        var trimmedLocation = (location ?? string.Empty).Trim();
        var trimmedName = (name ?? string.Empty).Trim();
        SensorSpecifications.ValidateEntityInternal(trimmedLocation, trimmedName);
        await SensorSpecifications.ValidateEntityExternal(Id, trimmedLocation, trimmedName, uniquenessChecker, ct);
        Location = trimmedLocation;
        Name = trimmedName;
    }

    public override string ToString() => $"{Location}_{Name}";
}
