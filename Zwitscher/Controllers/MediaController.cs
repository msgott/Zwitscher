
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Zwitscher.Attributes;
using Zwitscher.Data;
using Zwitscher.Models;
namespace Zwitscher.Controllers
{

    public class MediaController : Controller 
        //Controller Class for mainly handling Media Objects
        //Note: All of the real API Endpoints are in the other controllers
    {

        
        private readonly ZwitscherContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        #region Base MVC Stuff for Index, Create, Edit, Delete
        //============================================= Base MVC Stuff for Index, Upload, Delete =====================================================
        public MediaController(ZwitscherContext context, IWebHostEnvironment hostingEnvironment)
        //Injecting needed contexts
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        

        [Moderator]
        [HttpGet]
        [Route("Media")]
        public async Task<ActionResult> Index()
        //HTTP Get Index endpoint
        //Serves the View for the Media Index page
        {
            var zwitscherContext = _context.Media;
            Console.WriteLine("Media Index");
            return View(await zwitscherContext.ToListAsync());
        }

        // GET: Media/Upload
        [Moderator]
        [HttpGet]
        public ActionResult Upload()
        //HTTP Get Index endpoint
        //Serves the View for the Media Upload when in MVC Frontend
        {
            Console.WriteLine("Media Upload");
            return View();
        }

        [HttpGet]
        [Route("Media2/{imageName}")]
        public ActionResult GetImage(string imageName)
        //HTTP Get endpoint for returning an Image
        //just for test purposes and not anymore needed because files are now static
        //currently not used
        //Serves an Image
        {
            Console.WriteLine(Directory.GetFiles("Media")[0]);
            Console.WriteLine(System.IO.File.Exists(Directory.GetFiles("Media")[0]));
            //System.IO.File.
            string imagePath = Directory.GetFiles("Media")[0];
            return File(imagePath, "image/jpg"); // Adjust the MIME type based on the image file type
        }

        // POST: Media/Upload
        [Moderator]
        [HttpPost]
        [Route("Media/Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        //HTTP Post endpoint for uploading an Image
        //Takes the Image as parameter
        //only single Image supported
        //redirects client after Upload to media Index view
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

        [Moderator]
        [HttpGet]
        [Route("Media/Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        //HTTP Get endpoint for deleting an Image
        //Takes the mediaId as parameter
        //serves the View if the requested id was found
        {
            if (id == null || _context.Media == null)
            {
                return NotFound();
            }

            var media = await _context.Media
                .Include(m => m.User)
                .Include(m => m.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (media == null)
            {
                return NotFound();
            }

            return View(media);
        }

        [Moderator]
        [HttpPost]
        [Route("Media/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        //HTTP Post endpoint for deleting an Image
        //Takes the mediaId as parameter
        //redirects client to Media index view
        {
            if (_context.Media == null)
            {
                return Problem("Entity set 'ZwitscherContext.Media'  is null.");
            }
            var media = await _context.Media
                .Include (m => m.User)
                .Include(m => m.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (media != null)
            {
                if(media.User is not null)
                {
                    
                    //media.User.ProfilePicture = null;
                    //media.User.MediaId = null;
                    media.User.ProfilePicture = null;
                    media.User = null;
                    await _context.SaveChangesAsync();
                }
                if (media.Post is not null)
                {
                    
                    media.Post = null;
                    await _context.SaveChangesAsync();
                }
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
    #endregion
}
