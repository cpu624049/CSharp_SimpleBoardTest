namespace SimpleBoardTest.Models
{
    public class Post
    {
        public int PostId { get; set; } // PK

        public int UserId { get; set; } // FK

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int ViewCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();


    }
}
