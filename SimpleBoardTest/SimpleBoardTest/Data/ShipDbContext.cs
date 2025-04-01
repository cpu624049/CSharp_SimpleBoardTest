using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Models;

namespace SimpleBoardTest.Data
{
    public class ShipDbContext : DbContext
    {
        public ShipDbContext(DbContextOptions<ShipDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
        public DbSet<ReplyPost> ReplyPosts => Set<ReplyPost>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ 실제 DB 테이블명 수동 지정
            modelBuilder.Entity<User>().ToTable("Board_Users");
            modelBuilder.Entity<Post>().ToTable("Board_Posts");
            modelBuilder.Entity<PostLike>().ToTable("Board_PostLikes");
            modelBuilder.Entity<Comment>().ToTable("Board_Comments");
            modelBuilder.Entity<CommentLike>().ToTable("Board_CommentLikes");
            modelBuilder.Entity<ReplyPost>().ToTable("Board_ReplyPosts");

            // PostLike 중복 방지
            modelBuilder.Entity<PostLike>().HasIndex(PostLike => new { PostLike.PostId, PostLike.UserId }).IsUnique();
            // CommentLike 중복 방지
            modelBuilder.Entity<CommentLike>().HasIndex(CommentLike => new { CommentLike.CommentId, CommentLike.UserId }).IsUnique();
            // 댓글 (Comment-ParentComment 관계, Self FK)
            modelBuilder.Entity<Comment>().HasOne(Comment => Comment.ParentComment).WithMany(Comment => Comment.Replies).HasForeignKey(Comment => Comment.ParentCommentId).OnDelete(DeleteBehavior.Restrict);
            // 답글 (ReplyPost-ParentPost 관계)
            modelBuilder.Entity<ReplyPost>().HasOne(ReplyPost => ReplyPost.ParentPost).WithMany().HasForeignKey(r => r.ParentPostId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
