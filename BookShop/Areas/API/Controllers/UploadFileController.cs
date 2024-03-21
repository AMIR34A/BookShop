using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadFileController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public UploadFileController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    public async Task<string> UploadImage([FromBody]string base64Image)
    {
        var bytes = Convert.FromBase64String(base64Image);
        string path = $"{_webHostEnvironment.WebRootPath}\\Files\\{Guid.NewGuid()}.jpg";
        await System.IO.File.WriteAllBytesAsync(path, bytes);
        return "عکس با موفقیت آپلود شد";
    }
}
