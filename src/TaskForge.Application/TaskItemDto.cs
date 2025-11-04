using TaskForge.Domain.Enum;

namespace TaskForge.Domain
{
    public class TaskItemDto
    {
        #region Props

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskItemStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        #endregion
    }
}
