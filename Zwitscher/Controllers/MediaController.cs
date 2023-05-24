using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Zwitscher.Data;
using Zwitscher.Models;
namespace Zwitscher.Controllers
{

    public class MediaController : Controller
    {


        private readonly ZwitscherContext _context;

        public MediaController(ZwitscherContext context)
        {
            _context = context;
        }

        // GET: Media
        [HttpGet]
        
        public ActionResult Upload()
        {
            Console.WriteLine("Media Index");
            return View();
        }

        // POST: Media/Upload
        [HttpPost]
        [Route("Media/Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine("Media", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Media image = new Media
                {
                    FileName = fileName,
                    FilePath = filePath
                };

                _context.Media.Add(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
