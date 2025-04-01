namespace SimpleBoardTest.Models
{
    public class User
    {
        public int UserId { get; set; } // PK
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!; // 암호화 필요
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
        public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
    }
}
