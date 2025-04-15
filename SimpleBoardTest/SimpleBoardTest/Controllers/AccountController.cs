using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleBoardTest.Data;
using SimpleBoardTest.Models;
using SimpleBoardTest.ViewModels;

namespace SimpleBoardTest.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShipDbContext _DbContext;

        public AccountController(ShipDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        // 로그인 페이지
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Account/Login.cshtml");
        }

        // 로그인 처리
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashPassword(model.Password);
                var user = _DbContext.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == hashedPassword);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserName", user.UserName);

                    return RedirectToAction("Index", "Board");
                }

                ModelState.AddModelError("", "이메일 또는 비밀번호가 일치하지 않습니다.");
            }

            await Task.CompletedTask; // 비동기 메서드로 변경
            return View("~/Views/Account/Login.cshtml", model);
        }

        // 회원가입 페이지
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Account/Register.cshtml");
        }

        // 회원가입 처리
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _DbContext.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "이미 사용 중인 이메일 주소입니다.");

                    return View("~/Views/Account/Register.cshtml", model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _DbContext.Users.Add(user);
                await _DbContext.SaveChangesAsync();

                // 회원가입 후 자동 로그인
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserName", user.UserName);

                return RedirectToAction("Index", "Board");
            }

            return View("~/Views/Account/Register.cshtml", model);
        }

        // 로그아웃 처리
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        // 회원정보 수정 페이지
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _DbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserEditViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email
            };

            return View("~/Views/Account/Edit.cshtml", model);
        }

        // 회원정보 수정 처리
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Account/Edit.cshtml", model);
            }

            var user = await _DbContext.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // 로그인한 사용자가 본인 정보만 수정 가능하게 제약
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId != user.UserId)
            {
                return Unauthorized();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.UpdatedAt = DateTime.Now;

            // 비밀번호 수정이 요청된 경우만 변경
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.Password = HashPassword(model.NewPassword);
            }

            await _DbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "회원정보가 수정되었습니다.";
            HttpContext.Session.SetString("UserName", user.UserName); // 세션 업데이트

            return RedirectToAction("Edit");
        }


        // 비밀번호 해시 함수
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
