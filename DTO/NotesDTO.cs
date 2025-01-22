using System;
using System.ComponentModel.DataAnnotations;

namespace NoteTakingApp.DTOs
{
    public class NoteDTO
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool? Status { get; set; }
    }
}
