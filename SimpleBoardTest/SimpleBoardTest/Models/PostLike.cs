namespace SimpleBoardTest.Models
{
    public class PostLike
    {
        public int PostLikeId { get; set; } // PK

        public int PostId { get; set; } // FK

        public int UserId { get; set; } // FK

        public DateTime CreatedAt { get; set; }

        public virtual Post Post { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}
