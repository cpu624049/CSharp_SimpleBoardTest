using Microsoft.AspNetCore.Mvc;
using SimpleBoardTest.Data;

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
        public IActionResult Create(int? parentId = null)
        {
            ViewBag.ParentId = parentId; // 답글일 경우 부모 ID

            return View();
        }

        // 게시글 작성 처리
        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now;
                post.UpdatedAt = DateTime.Now;
                _DbContext.Posts.Add(post);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("HomeIndex", "Home");
            }

            return View(post);
        }

        // 게시글 수정 화면
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _DbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // 게시글 수정 처리
        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                _DbContext.Update(post);
                await _DbContext.SaveChangesAsync();
                
                return RedirectToAction("HomeIndex", "Home");
            }

            return View(post);
        }

        // 게시글 삭제 처리
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _DbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _DbContext.Posts.Remove(post);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("HomeIndex", "Home");
        }
    }
}
