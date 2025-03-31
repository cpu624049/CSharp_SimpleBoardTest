using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Data;

namespace SimpleBoardTest.Controllers
{
    public class BoardController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public BoardController(ShipDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        // 게시글 목록
        public async Task<IActionResult> BoardIndex()
        {
            var posts = await _DbContext.Posts.Include(p => p.User).OrderByDescending(p => p.CreatedAt).ToListAsync();

            return View(posts);
        }

        // 게시글 상세 보기
        public async Task<IActionResult> Details(int id)
        {
            var post = await _DbContext.Posts.Include(p => p.User).Include(p => p.Comments).FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            // 조회수 증가
            post.ViewCount++;
            await _DbContext.SaveChangesAsync();

            return View(post);
        }
    }
}
