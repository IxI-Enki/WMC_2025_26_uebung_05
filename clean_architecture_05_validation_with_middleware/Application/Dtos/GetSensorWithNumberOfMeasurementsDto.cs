namespace Application.Dtos;

/// <summary>
/// Daten�bertragungsobjekt (DTO) f�r Sensoren inkl. Anzahl der Messungen.
/// Als Referenztyp (record class), um Nullability sauber zu erzwingen.
/// </summary>
public sealed record GetSensorWithNumberOfMeasurementsDto( int Id , string Location , string Name , int NumberOfMeasurements );
