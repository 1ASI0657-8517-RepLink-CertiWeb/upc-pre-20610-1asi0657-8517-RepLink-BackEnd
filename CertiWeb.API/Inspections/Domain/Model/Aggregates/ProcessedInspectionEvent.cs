using System.Diagnostics.CodeAnalysis;
using CertiWeb.API.Inspections.Domain.Model.Commands;

namespace CertiWeb.API.Inspections.Domain.Model.Aggregates;

/// <summary>
/// Represents a single inspection.completed message consumed from RabbitMQ, kept as evidence
/// that the asynchronous processing pipeline (AC-03) actually persists received events.
/// </summary>
public class ProcessedInspectionEvent
{
    public int Id { get; private set; }

    /// <summary>
    /// UTC timestamp of when the message was received by the consumer.
    /// </summary>
    public DateTime ReceivedAt { get; private set; }

    /// <summary>
    /// The raw message body exactly as it arrived on the queue.
    /// </summary>
    public required string RawMessage { get; set; }

    /// <summary>
    /// Processing status of the event. Defaults to "processed".
    /// </summary>
    public string Status { get; set; }

    [SetsRequiredMembers]
    public ProcessedInspectionEvent()
    {
        RawMessage = string.Empty;
        Status = "processed";
        ReceivedAt = DateTime.UtcNow;
    }

    [SetsRequiredMembers]
    public ProcessedInspectionEvent(CreateProcessedInspectionEventCommand command)
    {
        ReceivedAt = DateTime.UtcNow;
        RawMessage = command.RawMessage;
        Status = "processed";
    }
}
