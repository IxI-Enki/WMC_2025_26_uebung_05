using System.Text.Json;
using FluentValidation;
using Domain.Exceptions;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Api.Middleware;

/// <summary>
/// Middleware, die Validierungs- und Domänenfehler abfängt und in konsistente HTTP-Antworten (Problem Details-ähnlich) umwandelt.
/// Fehler werden der Middleware über Ausnahmen übergeben. Der Typ der Ausnahme bestimmt den HTTP-Statuscode.
/// </summary>
public class ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger = logger;


    /// <summary>
    /// Wird vom Mediator je Command/Query aufgerufen.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            // Fehler aus FluentValidation: 400 mit Fehlerschlüssel je Property zurückgeben
            _logger.LogWarning(ex, "Application Validation failed");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            var payload = new
            {
                title = "Application validation failed",
                status = StatusCodes.Status400BadRequest,
                errors
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
        catch (DomainValidationException ex)
        {
            // Fehler aus der Domänenschicht: ebenfalls als 400 zurückgeben
            _logger.LogWarning(ex, "Domain validation failed");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                title = "Domain validation failed",
                status = StatusCodes.Status400BadRequest,
                //errors =new Dictionary<string, string[]> { { ex.Property, new[] { ex.Message } } },
                errors = new Dictionary<string, string[]>
                {
                    [ex.Property] = [ex.Message]
                }
            };

            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
        catch (NotFoundException ex)
        {
            // Ressource nicht gefunden: 404
            _logger.LogInformation(ex, "Resource not found");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                title = "Not Found",
                status = StatusCodes.Status404NotFound,
                errors = new Dictionary<string, string[]>
                {
                    ["NotFound"] = [ex.Message]
                }
            };
            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }

        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogInformation(ex, "Resource not found");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                title = "DB Concurrency exception",
                status = StatusCodes.Status409Conflict,
                errors = new Dictionary<string, string[]>
                {
                    ["DbConcurrency"] = ["Die Daten wurden zwischenzeitlich geändert. Bitte erneut versuchen."]
                }
            };
            var json = JsonSerializer.Serialize(payload);
            await context.Response.WriteAsync(json);
        }
    }
}
