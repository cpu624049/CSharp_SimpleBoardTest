using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleBoardTest.Models
{
    public class Post
    {
        public int PostId { get; set; } // PK
        public int? ParentPostId { get; set; } // NULL 이면 원글, 값 있으면 답글
        public int UserId { get; set; } // FK
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

        // 자기참조 (답글)
        [ForeignKey("ParentPostId")]
        public virtual Post? ParentPost { get; set; }
        public virtual ICollection<Post> Replies { get; set; } = new List<Post>();
    }
}
