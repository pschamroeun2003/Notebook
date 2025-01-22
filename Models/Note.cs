using System.ComponentModel.DataAnnotations;

namespace NoteTakingApp.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string UserId { get; set; }
        public bool Status { get; set; } = true; 
    }
}
