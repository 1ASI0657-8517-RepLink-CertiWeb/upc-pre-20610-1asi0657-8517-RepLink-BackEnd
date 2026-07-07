using System.Net.Mime;
using CertiWeb.API.Inspections.Domain.Model.Queries;
using CertiWeb.API.Inspections.Domain.Services;
using CertiWeb.API.Inspections.Interfaces.REST.Resources;
using CertiWeb.API.Inspections.Interfaces.REST.Transform;
using CertiWeb.API.Users.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CertiWeb.API.Inspections.Interfaces.REST;

/// <summary>
/// REST API controller for reviewing processed inspection.completed events (AC-03: asynchronous
/// processing evidence). Restricted to admin users only.
/// </summary>
[ApiController]
[Route("api/v1/inspection-events")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin-only endpoint for reviewing processed inspection.completed events.")]
[AuthorizeAdmin]
public class InspectionEventsController(IProcessedInspectionEventQueryService processedInspectionEventQueryService) : ControllerBase
{
    /// <summary>
    /// Retrieves the most recently processed inspection events, newest first.
    /// </summary>
    /// <param name="count">Maximum number of entries to return (default 50).</param>
    [HttpGet]
    [SwaggerOperation(Summary = "List the most recently processed inspection.completed events (admin only)")]
    [SwaggerResponse(StatusCodes.Status200OK, "The most recently processed inspection events")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "No valid authentication token was supplied")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The authenticated user does not have the admin role")]
    public async Task<ActionResult<IEnumerable<InspectionEventResource>>> GetRecentEvents([FromQuery] int count = 50)
    {
        var query = new GetRecentProcessedInspectionEventsQuery(count);
        var events = await processedInspectionEventQueryService.Handle(query);
        var resources = events.Select(InspectionEventResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
