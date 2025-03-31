using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Azure.Identity;
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

                    return RedirectToAction("BoardIndex", "Board");
                }

                ModelState.AddModelError("", "이메일 또는 비밀번호가 일치하지 않습니다.");
            }

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
                if (_DbContext.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "이미 사용 중인 이메일 주소입니다.");

                    return View("~/Views/Account/Register.cshtml", model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    CreatedAt = DateTime.Now
                };

                _DbContext.Users.Add(user);
                await _DbContext.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View("~/Views/Account/Register.cshtml", model);
        }

        // 로그아웃 처리
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
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
