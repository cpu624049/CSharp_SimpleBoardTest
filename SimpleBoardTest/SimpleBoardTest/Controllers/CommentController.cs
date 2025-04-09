using Microsoft.AspNetCore.Mvc;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;

namespace SimpleBoardTest.Controllers
{
    public class CommentController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public CommentController(ShipDbContext dbContext)
        {
            _DbContext = dbContext;
        }

        // ✅ 댓글 등록
        [HttpPost]
        public async Task<IActionResult> Create(int postId, int? parentCommentId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "내용을 입력해주세요.";

                return RedirectToAction("Detail", "Board", new { id = postId });
            }

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId.Value,
                Content = content,
                ParentCommentId = parentCommentId,
                CreatedAt = DateTime.Now
            };

            _DbContext.Comments.Add(comment);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Board", new { id = postId });
        }

        // ✅ 댓글 수정
        [HttpPost]
        public async Task<IActionResult> Edit(int commentId, string content)
        {
            var comment = await _DbContext.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (comment.UserId != userId)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "내용을 입력해주세요.";

                return RedirectToAction("Detail", "Board", new { id = comment.PostId });
            }

            comment.Content = content;
            comment.UpdatedAt = DateTime.Now;

            _DbContext.Comments.Update(comment);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Board", new { id = comment.PostId });
        }

        // ✅ 댓글 삭제
        [HttpPost]
        public async Task<IActionResult> Delete(int commentId)
        {
            var comment = await _DbContext.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsDeleted = true;
            comment.Content = "[삭제된 댓글입니다.]";

            await _DbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Board", new { id = comment.PostId });
        }
    }
}
