namespace SimpleBoardTest.Models
{
    public class CommentLike
    {
        public int CommentLikeId { get; set; } // PK
        public int CommentId { get; set; } // FK
        public int UserId { get; set; } // FK
        public DateTime CreatedAt { get; set; }

        public virtual Comment Comment { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
