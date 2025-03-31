using Microsoft.AspNetCore.Mvc;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;

namespace SimpleBoardTest.Controllers
{
    public class PostController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public PostController(ShipDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        // 게시글 작성 화면
        [HttpGet]
        public IActionResult PostCreate()
        {
            return View("~/Views/Board/Post/PostCreate.cshtml");
        }

        // 게시글 작성 처리
        [HttpPost]
        public async Task<IActionResult> PostCreate(Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                _DbContext.Posts.Add(post);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("BoardIndex", "Board");
            }

            return View("~/Views/Board/BoardHome/BoardDetail.cshtml", post);
        }

        // 게시글 수정 화면
        [HttpGet]
        public async Task<IActionResult> PostEdit(int id)
        {
            var post = await _DbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View("~/Views/Board/Post/PostEdit.cshtml", post);
        }

        // 게시글 수정 처리
        [HttpPost]
        public async Task<IActionResult> PostEdit(Post post)
        {
            if (ModelState.IsValid)
            {
                _DbContext.Update(post);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("BoardIndex", "Board");
            }

            return View("~/Views/Board/BoardHome/BoardDetail.cshtml", post);
        }

        // 게시글 삭제 처리
        [HttpPost]
        public async Task<IActionResult> PostDelete(int id)
        {
            var post = await _DbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _DbContext.Posts.Remove(post);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("BoardIndex", "Board");
        }
    }
}
