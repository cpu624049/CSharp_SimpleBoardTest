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

        // Navigation
        public virtual Post Post { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public virtual Comment? ParentComment { get; set; }

        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
    }
}
