namespace CertiWeb.API.Inspections.Domain.Model.Queries;

/// <summary>
/// Query for retrieving the most recently processed inspection events, newest first.
/// </summary>
/// <param name="Count">Maximum number of entries to return.</param>
public record GetRecentProcessedInspectionEventsQuery(int Count);
