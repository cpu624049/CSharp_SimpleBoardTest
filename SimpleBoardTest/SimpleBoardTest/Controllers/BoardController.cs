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
            var posts = await _DbContext.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View("~/Views/Board/BoardHome/BoardIndex.cshtml", posts);
        }

        // 게시글 상세 보기
        public async Task<IActionResult> Details(int id)
        {
            var post = await _DbContext.Posts
                .Include(p => p.User) // ✅ 게시글 작성자
                .Include(p => p.ParentPost) // ✅ 원글
                .Include(p => p.Replies) // ✅ 답글
                    .ThenInclude(r => r.User) // ✅ 답글 작성자
                .Include(p => p.Comments) // ✅ 댓글
                .FirstOrDefaultAsync(p => p.PostId == id); // 게시글 ID로 조회

            if (post == null)
            {
                return NotFound();
            }

            // 조회수 증가
            post.ViewCount++;
            await _DbContext.SaveChangesAsync();

            return View("~/Views/Board/BoardHome/BoardDetail.cshtml", post);
        }
    }
}
