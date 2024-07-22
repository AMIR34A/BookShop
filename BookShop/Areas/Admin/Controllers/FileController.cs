using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        public async Task<IActionResult> Upload(IEnumerable<IFormFile> files)
        {
            foreach (var file in files)
            {
                string path = string.Concat(_webHostEnvironment.WebRootPath, "\\Files");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string extension = Path.GetExtension(file.FileName);
                string newPath = string.Concat(path, @"\", Guid.NewGuid(), extension);
                using (FileStream fileStream = new FileStream(newPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            ViewBag.Alert = "آپلود فایل ها با موفقیت انجام شد.";
            return View();
        }
    }
}
