using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;

namespace SimpleBoardTest.Controllers
{
    public class LikeController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public LikeController(ShipDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(string targetType, int targetId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Json(new { success = false, message = "로그인 후 이용해주세요." });
            }

            var existing = await _DbContext.Likes
                .FirstOrDefaultAsync(l => l.TargetType == targetType && l.TargetId == targetId && l.UserId == userId.Value);

            if (existing != null)
            {
                _DbContext.Likes.Remove(existing);
                await _DbContext.SaveChangesAsync();
            }
            else
            {
                var like = new Like
                {
                    TargetType = targetType,
                    TargetId = targetId,
                    UserId = userId.Value,
                    CreatedAt = DateTime.Now
                };
                _DbContext.Likes.Add(like);
                await _DbContext.SaveChangesAsync();
            }

            // 좋아요 수 재계산
            var likeCount = await _DbContext.Likes
                .CountAsync(l => l.TargetType == targetType && l.TargetId == targetId);

            var isLiked = await _DbContext.Likes
                .AnyAsync(l => l.TargetType == targetType && l.TargetId == targetId && l.UserId == userId.Value);

            return Json(new
            {
                success = true,
                likeCount,
                isLiked
            });
        }
    }
}
