

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Zwitscher.Data;
using Zwitscher.Models;
namespace Zwitscher.Controllers
{

    public class MediaController : Controller
    {


        private readonly ZwitscherContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MediaController(ZwitscherContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: Media
        [HttpGet]
        [Route("Media")]
        public async Task<ActionResult> Index()
        {
            var zwitscherContext = _context.Media;
            Console.WriteLine("Media Index");
            return View(await zwitscherContext.ToListAsync());
        }

        // GET: Media/Upload
        [HttpGet]
        public ActionResult Upload()
        {
            Console.WriteLine("Media Upload");
            return View();
        }

        [HttpGet]
        [Route("Media2/{imageName}")]
        public ActionResult GetImage(string imageName)
        {
            Console.WriteLine(Directory.GetFiles("Media")[0]);
            Console.WriteLine(System.IO.File.Exists(Directory.GetFiles("Media")[0]));
            //System.IO.File.
            string imagePath = Directory.GetFiles("Media")[0];
            return File(imagePath, "image/jpg"); // Adjust the MIME type based on the image file type
        }

        // POST: Media/Upload
        [HttpPost]
        [Route("Media/Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                Guid tempID = Guid.NewGuid();
                
                string fileName = tempID.ToString() + Path.GetExtension(file.FileName);
                //string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine("wwwroot","Media", fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Media image = new Media
                {
                    Id = tempID, 
                    FileName = fileName,
                    FilePath = filePath
                };

                _context.Media.Add(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Media/Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Media == null)
            {
                return NotFound();
            }

            var media = await _context.Media
                .FirstOrDefaultAsync(m => m.Id == id);
            if (media == null)
            {
                return NotFound();
            }

            return View(media);
        }

        [HttpPost]
        [Route("Media/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Media == null)
            {
                return Problem("Entity set 'ZwitscherContext.Media'  is null.");
            }
            var media = await _context.Media.FindAsync(id);
            if (media != null)
            {
                if (Path.Exists(media.FilePath))
                {
                    System.IO.File.Delete(media.FilePath);
                }
                _context.Media.Remove(media);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
