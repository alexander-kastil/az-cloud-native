using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Writer.Models;

namespace Writer.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IConfiguration config){
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile upload)
        {
            if (upload != null && upload.Length > 0)
            {
                var fileName = Path.GetFileName(upload.FileName);
                var target = Configuration["IMG_FOLDER_NAME"];
                var directory = Path.Combine(Directory.GetCurrentDirectory(), target);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var filePath = Path.Combine(directory, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await upload.CopyToAsync(stream);
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}