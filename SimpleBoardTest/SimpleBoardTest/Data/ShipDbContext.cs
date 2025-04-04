using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Models;

namespace SimpleBoardTest.Data
{
    public class ShipDbContext : DbContext
    {
        public ShipDbContext(DbContextOptions<ShipDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Like> Likes => Set<Like>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ 실제 DB 테이블명 수동 지정
            modelBuilder.Entity<User>().ToTable("Board_Users");
            modelBuilder.Entity<Post>().ToTable("Board_Posts");
            modelBuilder.Entity<Comment>().ToTable("Board_Comments");
            modelBuilder.Entity<Like>().ToTable("Board_Likes");

            // 자기 참조 (Post-ReplyPost)
            modelBuilder.Entity<Post>().HasOne(p => p.ParentPost).WithMany(p => p.Replies).HasForeignKey(p => p.ParentPostId).OnDelete(DeleteBehavior.Restrict);
            // 자기 참조 (comment-comment)
            modelBuilder.Entity<Comment>().HasOne(c => c.ParentComment).WithMany(c => c.Replies).HasForeignKey(c => c.ParentCommentId).OnDelete(DeleteBehavior.Restrict);
            // Like 중복 방지
            modelBuilder.Entity<Like>().HasIndex(l => new { l.TargetType, l.TargetId, l.UserId }).IsUnique();
        }
    }
}
