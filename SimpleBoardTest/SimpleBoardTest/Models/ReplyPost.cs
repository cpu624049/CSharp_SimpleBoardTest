using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBoardTest.Models
{
    public class ReplyPost
    {
        public int ReplyPostId { get; set; } // PK

        public int ParentPostId { get; set; } // FK

        public int UserId { get; set; } // FK

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ForeignKey("ParentPostId")]
        public virtual Post ParentPost { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
