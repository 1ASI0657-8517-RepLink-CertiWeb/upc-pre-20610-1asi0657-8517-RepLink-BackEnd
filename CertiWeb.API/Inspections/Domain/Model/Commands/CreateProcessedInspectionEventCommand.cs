namespace CertiWeb.API.Inspections.Domain.Model.Commands;

/// <summary>
/// Command to persist an inspection.completed message consumed from RabbitMQ.
/// </summary>
/// <param name="RawMessage">The raw message body exactly as it arrived on the queue.</param>
public record CreateProcessedInspectionEventCommand(string RawMessage);
