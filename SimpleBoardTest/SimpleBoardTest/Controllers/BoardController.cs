using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;

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

            var sortedPosts = SortPostsHierarchically(posts);

            return View("~/Views/Board/BoardHome/BoardIndex.cshtml", sortedPosts);
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

        // 게시글을 계층 구조로 정렬하는 재귀 메서드
        private List<Post> SortPostsHierarchically(List<Post> allPosts, int? parentId = null, int depth = 0)
        {
            List<Post> sorted = new();

            var children = allPosts
                .Where(p => p.ParentPostId == parentId)
                .OrderByDescending(p => p.CreatedAt) // 최신순 정렬
                .ToList();

            foreach (var post in children)
            {
                string indent = string.Concat(Enumerable.Repeat("&nbsp;&nbsp;&nbsp;&nbsp;", depth));
                post.Title = $"{indent}{(depth > 0 ? "👉 Re: " : "")}{post.Title}";
                sorted.Add(post);
                sorted.AddRange(SortPostsHierarchically(allPosts, post.PostId, depth + 1));
            }

            return sorted;
        }
    }
}
