namespace CertiWeb.API.Inspections.Interfaces.REST.Resources;

/// <summary>
/// REST resource representing a processed inspection event.
/// </summary>
/// <param name="Id">Unique identifier of the event.</param>
/// <param name="ReceivedAt">UTC timestamp of when the message was received.</param>
/// <param name="RawMessage">The raw message body exactly as it arrived on the queue.</param>
/// <param name="Status">Processing status of the event.</param>
public record InspectionEventResource(
    int Id,
    DateTime ReceivedAt,
    string RawMessage,
    string Status);
