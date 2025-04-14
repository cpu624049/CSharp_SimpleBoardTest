namespace SimpleBoardTest.Models
{
    public class Comment
    {
        public int CommentId { get; set; } // PK
        public int PostId { get; set; } // FK
        public int UserId { get; set; } // FK
        public int? ParentCommentId { get; set; } // FK
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft delete

        // Navigation
        public virtual Post Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Comment? ParentComment { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
