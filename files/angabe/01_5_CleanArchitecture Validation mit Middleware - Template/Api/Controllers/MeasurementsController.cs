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
}
