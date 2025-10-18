using Api.Extensions;
using Application.Dtos;
using Application.Features.Measurements.Commands.AddMeasurement;
using Application.Features.Measurements.Commands.CreateMeasurement;
using Application.Features.Measurements.Commands.DeleteMeasurement;
using Application.Features.Measurements.Queries.GetAllMeasurements;
using Application.Features.Measurements.Queries.GetLast100Measurements;
using Application.Features.Measurements.Queries.GetMeasurementById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Endpunkte für Messungen.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MeasurementsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Liefert alle Messungen.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetMeasurementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var dtos = await mediator.Send(new GetAllMeasurementsQuery(), ct);
        return Ok(dtos);
    }

    /// <summary>
    /// Liefert eine Messung per Id, falls vorhanden.
    /// Sonst 404 Not Found.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetMeasurementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var dto = await mediator.Send(new GetMeasurementByIdQuery(id), ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Legt eine neue Messung an.
    /// </summary>
    /// <remarks>
    /// Regeln:
    /// - SensorId muss existieren
    /// - Value muss zwischen -100 und 100 liegen
    /// - Timestamp darf nicht in der Zukunft liegen und muss innerhalb der letzten Stunde liegen
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(GetMeasurementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateMeasurementCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, createdAtAction: nameof(GetById),
            routeValues: new { id = result?.Value?.Id });
    }

    /// <summary>
    /// Löscht eine Messung.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteMeasurementCommand(id), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Fügt eine neue Messung hinzu mit automatischer Sensor-Erstellung.
    /// </summary>
    /// <remarks>
    /// Falls ein Sensor mit der angegebenen Location und Name noch nicht existiert,
    /// wird er automatisch angelegt.
    ///
    /// Regeln:
    /// - Location darf nicht leer sein (max. 100 Zeichen)
    /// - Name muss mindestens 2 Zeichen lang sein (max. 100 Zeichen)
    /// - Value muss zwischen -100 und 100 liegen
    /// - Timestamp darf nicht in der Zukunft liegen und muss innerhalb der letzten Stunde liegen
    /// </remarks>
    [HttpPost("add")]
    [ProducesResponseType(typeof(GetMeasurementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMeasurement([FromBody] AddMeasurementCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, createdAtAction: nameof(GetById),
            routeValues: new { id = result?.Value?.Id });
    }

    /// <summary>
    /// Liefert die 100 neuesten Messwerte sortiert nach Timestamp absteigend.
    /// </summary>
    [HttpGet("last100")]
    [ProducesResponseType(typeof(IReadOnlyCollection<GetMeasurementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLast100Measurements(CancellationToken ct)
    {
        var measurements = await mediator.Send(new GetLast100MeasurementsQuery(), ct);
        return Ok(measurements);
    }
}
