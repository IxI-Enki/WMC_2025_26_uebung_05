namespace Application.Dtos;

/// <summary>
/// Daten�bertragungsobjekt (DTO) f�r Sensoren ohne RowVersion/Concurrency-Token.
/// Als Referenztyp (record class), um Nullability sauber zu erzwingen.
/// </summary>
public sealed record GetSensorDto( int Id , string Location , string Name );
