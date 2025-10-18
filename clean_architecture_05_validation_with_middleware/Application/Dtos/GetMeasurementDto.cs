namespace Application.Dtos;

/// <summary>
/// Datenübertragungsobjekt (DTO) für Messungen.
/// </summary>
public sealed record GetMeasurementDto( int Id , int SensorId , double Value , DateTime Timestamp );
