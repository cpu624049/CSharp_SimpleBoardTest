using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;
using SimpleBoardTest.ViewModels;

namespace SimpleBoardTest.Controllers
{
    public class BoardController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public BoardController(ShipDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        // ✅ 게시글 목록
        public async Task<IActionResult> Index()
        {
            var posts = await _DbContext.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var sortedPosts = SortPostsHierarchically(posts);
            return View("~/Views/Board/Post/Index.cshtml", sortedPosts);
        }

        // ✅ 게시글 상세 보기
        public async Task<IActionResult> Detail(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var post = await _DbContext.Posts
                .Include(p => p.User)
                .Include(p => p.ParentPost)
                .Include(p => p.Replies).ThenInclude(r => r.User)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Comments).ThenInclude(c => c.Replies).ThenInclude(rc => rc.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null) return NotFound();

            // ✅ 게시글 좋아요
            var likeCount = await _DbContext.Likes
                .CountAsync(l => l.TargetType == "Post" && l.TargetId == post.PostId);

            var isLiked = userId.HasValue &&
                await _DbContext.Likes.AnyAsync(l => l.TargetType == "Post" && l.TargetId == post.PostId && l.UserId == userId);

            ViewBag.PostLike = new LikeViewModel
            {
                TargetType = "Post",
                TargetId = post.PostId,
                LikeCount = likeCount,
                IsLiked = isLiked
            };

            // ✅ 댓글 좋아요
            Dictionary<int, int> commentLikes = new();
            List<int> likedCommentIds = new();

            var commentIds = post.Comments.Select(c => c.CommentId).ToList();

            if (commentIds.Any())
            {
                var likeList = await _DbContext.Likes
                    .Where(l => l.TargetType == "Comment" && commentIds.Contains(l.TargetId))
                    .ToListAsync();

                commentLikes = likeList
                    .GroupBy(l => l.TargetId)
                    .ToDictionary(g => g.Key, g => g.Count());

                if (userId.HasValue)
                {
                    likedCommentIds = likeList
                        .Where(l => l.UserId == userId)
                        .Select(l => l.TargetId)
                        .Distinct()
                        .ToList();
                }
            }

            ViewBag.CommentLikeCounts = commentLikes;
            ViewBag.CommentLikedIds = likedCommentIds;

            // ✅ 조회수 증가
            post.ViewCount++;
            await _DbContext.SaveChangesAsync();

            // ✅ 댓글 정렬 (계층 구조)
            Dictionary<int, int> commentDepths = new();
            post.Comments = SortCommentsHierarchically(post.Comments.ToList(), null, 0, commentDepths);
            ViewBag.CommentDepths = commentDepths;

            return View("~/Views/Board/Post/Detail.cshtml", post);
        }

        // ✅ 게시글 계층 정렬
        private List<Post> SortPostsHierarchically(List<Post> allPosts, int? parentId = null, int depth = 0)
        {
            List<Post> sorted = new();

            var children = allPosts
                .Where(p => p.ParentPostId == parentId)
                .OrderByDescending(p => p.CreatedAt)
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

        // ✅ 댓글 계층 정렬
        private List<Comment> SortCommentsHierarchically(List<Comment> allComments, int? parentId, int depth, Dictionary<int, int> depthMap)
        {
            var sorted = new List<Comment>();

            var children = allComments
                .Where(c => c.ParentCommentId == parentId)
                .OrderByDescending(c => c.CreatedAt)
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
