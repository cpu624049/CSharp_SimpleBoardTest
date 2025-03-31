using Microsoft.AspNetCore.Mvc;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;

namespace SimpleBoardTest.Controllers
{
    public class ReplyPostController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public ReplyPostController(ShipDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        // 답글 작성 화면
        [HttpGet]
        public IActionResult ReplyPostCreate(int parentPostId)
        {
            ViewBag.ParentPostId = parentPostId;

            return View("~/Views/Board/ReplyPost/ReplyPostCreate.cshtml");
        }

        // 답글 저장 처리
        [HttpPost]
        public async Task<IActionResult> ReplyPostCreate(ReplyPost replyPost)
        {
            if (ModelState.IsValid)
            {
                replyPost.CreatedAt = DateTime.Now;

                _DbContext.ReplyPosts.Add(replyPost);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("BoardDetails", "Board", new { id = replyPost.ParentPostId });
            }

            return View("~/Views/Board/BoardHome/BoardDetail.cshtml", replyPost);
        }

        // 답글 삭제
        [HttpPost]
        public async Task<IActionResult> ReplyPostDelete(int id)
        {
            var reply = await _DbContext.ReplyPosts.FindAsync(id);
            if (reply == null)
            {
                return NotFound();
            }

            _DbContext.ReplyPosts.Remove(reply);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("BoardDetails", "Board", new { id = reply.ParentPostId });
        }
    }
}
