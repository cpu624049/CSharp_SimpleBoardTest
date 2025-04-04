using System.ComponentModel.DataAnnotations;

namespace SimpleBoardTest.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        [Required]
        public string TargetType { get; set; } = null!; // Post or Comment
        public int TargetId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual User User { get; set; } = null!;
    }
}
