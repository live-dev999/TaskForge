#nullable disable

namespace TaskForge.Domain;

/// <summary>
/// Data transfer object for task change events sent to EventProcessor service and RabbitMQ.
/// </summary>
public class TaskChangeEventDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the task.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Gets or sets the type of event (Created, Updated, Deleted).
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the task.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the task.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the event occurred.
    /// </summary>
    public DateTime EventTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the task was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the task was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

