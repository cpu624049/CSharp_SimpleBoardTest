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
        public async Task<IActionResult> Index()
        {
            var posts = await _DbContext.Posts
                .Include(p => p.User)
                .Include(p => p.Comments) // ✅ 댓글
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var sortedPosts = SortPostsHierarchically(posts);

            return View("~/Views/Board/Post/Index.cshtml", sortedPosts);
        }

        // 게시글 상세 보기
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _DbContext.Posts
                .Include(p => p.User)                       // ✅ 게시글 작성자
                .Include(p => p.ParentPost)                 // ✅ 원글
                .Include(p => p.Replies)                    // ✅ 답글
                    .ThenInclude(r => r.User)                   // ✅ 답글 작성자
                .Include(p => p.Comments)                   // ✅ 댓글
                    .ThenInclude(c => c.User)                   // ✅ 댓글 작성자
                .Include(p => p.Comments)                   // ✅ 댓글
                    .ThenInclude(c => c.Replies)                // ✅ 대댓글
                        .ThenInclude(rc => rc.User)                 // ✅ 대댓글 작성자
                .FirstOrDefaultAsync(p => p.PostId == id);  // 게시글 ID로 조회

            if (post == null)
            {
                return NotFound();
            }

            // 조회수 증가
            post.ViewCount++;
            await _DbContext.SaveChangesAsync();

            // 댓글 정렬
            Dictionary<int, int> commentDepths = new();
            post.Comments = SortCommentsHierarchically(post.Comments.ToList(), null, 0, commentDepths);
            ViewBag.CommentDepths = commentDepths;

            return View("~/Views/Board/Post/Detail.cshtml", post);
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
                string indent = string.Concat(Enumerable.Repeat("&nbsp;&nbsp;&nbsp;&nbsp;", depth)); // 들여쓰기
                post.Title = $"{indent}{(depth > 0 ? "👉 Re: " : "")}{post.Title}";
                sorted.Add(post);
                sorted.AddRange(SortPostsHierarchically(allPosts, post.PostId, depth + 1));
            }

            return sorted;
        }

        // 댓글을 계층 구조로 정렬하는 재귀 메서드
        private List<Comment> SortCommentsHierarchically(List<Comment> allComments, int? parentId, int depth, Dictionary<int, int> depthMap)
        {
            var sorted = new List<Comment>();

            var children = allComments
                .Where(c => c.ParentCommentId == parentId)
                .OrderByDescending(c => c.CreatedAt) // 최신순 정렬
                .ToList();

            foreach (var comment in children)
            {
                depthMap[comment.CommentId] = depth;
                sorted.Add(comment);
                sorted.AddRange(SortCommentsHierarchically(allComments, comment.CommentId, depth + 1, depthMap));
            }

            return sorted;
        }
    }
}
