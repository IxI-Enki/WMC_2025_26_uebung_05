using Api.Extensions;
using Application.Dtos;
using Application.Features.Sensors.Commands.CreateSensor;
using Application.Features.Sensors.Commands.DeleteSensor;
using Application.Features.Sensors.Commands.UpdateSensor;
using Application.Features.Sensors.Queries.GetAllSensors;
using Application.Features.Sensors.Queries.GetSensorById;
using Application.Features.Sensors.Queries.GetSensorsWithCounts;
using Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Endpunkte rund um Sensoren.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SensorsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Liefert alle Sensoren sortiert nach Location und Name.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetSensorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        // Über MediatR die Query ausführen und DTOs aus der Anwendungsschicht abrufen
        var dtos = await mediator.Send(new GetAllSensorsQuery(), ct);
        return Ok(dtos);
    }

    /// <summary>
    /// Liefert einen Sensor per Id, falls vorhanden.
    /// Sonst 404 Not Found.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GetSensorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        // Einzelnen Sensor per Id laden; 404 zurückgeben, falls nicht vorhanden
        var dto = await mediator.Send(new GetSensorByIdQuery(id), ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Liefert alle Sensoren sortiert nach Location und Name.
    /// </summary>
    [HttpGet("withnumberofmeasurements")]
    [ProducesResponseType(typeof(IEnumerable<GetSensorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllWithNumberOfMeasurements(CancellationToken ct)
    {
        // Sensoren inkl. Anzahl der Messungen laden
        var dtos = await mediator.Send(new GetSensorsWithCountsQuery(), ct);
        return Ok(dtos);
    }

    /// <summary>
    /// Legt einen neuen Sensor an.
    /// </summary>
    /// <remarks>
    /// Regeln:
    /// - Name mindestens 2 Zeichen
    /// - Name darf nicht der Location entsprechen (case-insensitive)
    /// - Kombination (Location, Name) muss eindeutig sein
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(GetSensorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSensorCommand command, CancellationToken ct)
    {
        // Erstellung anstoßen; Result in passenden HTTP-Status umwandeln (Created etc.)
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, createdAtAction: nameof(GetById), 
            routeValues: new { id = result?.Value?.Id });
        //try
        //{
        //    var result = await mediator.Send(command, ct);
        //    var created = result;
        //    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        //}
        //catch (DomainValidationException ex)
        //{
        //    return BadRequest(new { error = ex.Message });
        //}
    }

    /// <summary>
    /// Aktualisiert einen Sensor.
    /// </summary>
    /// <remarks>
    /// Regeln wie beim Erstellen: Name min. 2, Name != Location, (Location, Name) eindeutig.
    /// </remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GetSensorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSensorCommand command, CancellationToken ct)
    {
        if (id != command.Id)
        {
            // Falls Route-ID und Body-ID nicht übereinstimmen, wird eine Validierungs-Exception ausgelöst
            var failures = new List<ValidationFailure>
                {
                    new(nameof(command.Id), "The route ID does not match the sensor ID in the request body.")};
            throw new ValidationException(failures);
        }
        // Update ausführen. Das Result wird in einen HTTP-Response gemappt.
        var result = await mediator.Send(new UpdateSensorCommand(id, command.Location, command.Name), ct);
        return result.ToActionResult(this);
        //try
        //{
        //    var updated = await mediator.Send(new UpdateSensorCommand(id, command.Location, command.Name), ct);
        //    return Ok(updated);
        //}
        //catch (DomainValidationNotFoundException ex)
        //{
        //    return NotFound(new { error = ex.Message });
        //}
        //catch (DomainValidationException ex)
        //{
        //    return BadRequest(new { error = ex.Message });
        //}
        //catch (DbUpdateConcurrencyException)
        //{
        //    return Conflict(new { error = "Der Sensor wurde zwischenzeitlich von einem anderen Benutzer geändert. Bitte erneut versuchen." });
        //}
        //catch (Exception ex)
        //{
        //    return Problem(
        //        detail: ex.Message,
        //        statusCode: 500,
        //        title: "An unexpected error occurred."
        //    );
        //}
    }

    /// <summary>
    /// Löscht einen Sensor.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteSensorCommand(id), ct);
        return result.ToActionResult(this);
    }
}
