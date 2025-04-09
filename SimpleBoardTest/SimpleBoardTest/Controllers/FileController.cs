using Microsoft.AspNetCore.Mvc;

namespace SimpleBoardTest.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
            {
                return BadRequest("업로드된 파일이 없습니다.");
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{upload.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(fileStream);
            }

            var fileUrl = Url.Content($"~/uploads/{uniqueFileName}");

            return Json(new
            {
                uploaded = true,
                url = fileUrl
            });
        }
    }
}
