using System.ComponentModel.DataAnnotations;

namespace TaskManager.Persistence.Models
{
    public class TaskPriority
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}