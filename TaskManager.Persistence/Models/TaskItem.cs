using System.ComponentModel.DataAnnotations;

namespace TaskManager.Persistence.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public TaskCategory? Category { get; set; }

        public int? PriorityId { get; set; }
        public TaskPriority? Priority { get; set; }

        public int? StatusId { get; set; }
        public TaskState? Status { get; set; }
    }
}