using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;
using SimpleBoardTest.ViewModels;

namespace SimpleBoardTest.Controllers
{
    public class PostController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public PostController(ShipDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        // 게시글/답글 작성 화면
        [HttpGet]
        public IActionResult Create(int? parentPostId = null)
        {
            var model = new PostViewModel
            {
                ParentPostId = parentPostId
            };

            return View("~/Views/Board/Post/Create.cshtml");
        }

        // 게시글/답글 작성 처리
        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentPostId = model.ParentPostId;
                return View("~/Views/Board/Post/Create.cshtml", model);
            }

            var userId = HttpContext.Session.GetInt32("UserId"); // 세션에서 UserId 가져오기

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // 로그인 상태 확인
            }

            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                UserId = userId.Value, // 세션에서 가져온 UserId
                ParentPostId = model.ParentPostId, // 답글일 경우 원글 ID
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _DbContext.Posts.Add(post);
            await _DbContext.SaveChangesAsync();

            return RedirectToAction("Detail", "Board", new { id = post.PostId });
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

            return View("~/Views/Board/Post/Edit.cshtml", post);
        }

        // 게시글 수정 처리
        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            var existingPost = await _DbContext.Posts.FindAsync(post.PostId);

            if (existingPost == null)
            {
                return NotFound();
            }

            // 본인 글만 수정 가능
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || existingPost.UserId != userId)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                existingPost.Title = post.Title;
                existingPost.Content = post.Content;
                existingPost.UpdatedAt = DateTime.Now;

                _DbContext.Update(existingPost); // 수정된 기존 엔티티 저장
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("Detail", "Board", new { id = post.PostId });
            }

            return RedirectToAction("Detail", "Board", new { id = post.PostId });
        }

        // 게시글 삭제 처리
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _DbContext.Posts.Include(p => p.Replies).FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || post.UserId != userId)
            {
                return Unauthorized();
            }

            if (post.Replies.Any())
            {
                // 답글 존재 시 내용 대체
                post.Title = "[삭제된 글입니다]";
                post.Content = "[삭제된 글입니다]";
                post.UpdatedAt = DateTime.Now;

                _DbContext.Posts.Update(post);
            }
            else
            {
                _DbContext.Posts.Remove(post);
            }

            await _DbContext.SaveChangesAsync();

            if (post.ParentPostId.HasValue)
            {
                return RedirectToAction("Detail", "Board", new { id = post.ParentPostId });
            }

            return RedirectToAction("Index", "Board");
        }
    }
}
