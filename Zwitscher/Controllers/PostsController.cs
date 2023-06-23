using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class PostsController : Controller
    {
        private readonly ZwitscherContext _context;

        public PostsController(ZwitscherContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var zwitscherContext = _context.Post
                .Include(p => p.User)
                .Include(p => p.Votes)
                .Include(p => p.Comments)
                .Include(p => p.Media);
            
            
            return View(await zwitscherContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["Votes"] = post.Votes;
            ViewData["Comments"] = post.Comments;
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName");

            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile[] files, [Bind("Id,TextContent,UserId")] Post post)
        {
            
            ModelState.Remove("files");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                foreach (IFormFile file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        Guid tempID = Guid.NewGuid();

                        string fileName = tempID.ToString() + Path.GetExtension(file.FileName);
                        //string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine("wwwroot", "Media", fileName);

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
                        post.Media.Add(image);
                    }
                }

                post.Id = Guid.NewGuid();
                post.CreatedDate = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Media)
                .FirstOrDefaultAsync(p=> p.Id==id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IFormFile[] files, [Bind("Id,CreatedDate,TextContent,UserId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            ModelState.Remove("files");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                try
                {
                    foreach (IFormFile file in files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            Guid tempID = Guid.NewGuid();

                            string fileName = tempID.ToString() + Path.GetExtension(file.FileName);
                            //string fileName = Path.GetFileName(file.FileName);
                            string filePath = Path.Combine("wwwroot", "Media", fileName);

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
                            post.Media.Add(image);
                        }
                    }
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Post == null)
            {
                return Problem("Entity set 'ZwitscherContext.Post'  is null.");
            }
            var post = await _context.Post
                .Include(post => post.User)
                .Include(post => post.Comments)
                .Include(post => post.Media)
                .FirstOrDefaultAsync(p=> p.Id == id);
            if (post != null)
            {
                /*foreach (Comment c in post.Comments)
                {
                    c.User = null;
                    post.Comments.Remove(c);
                    _context.Comment.Remove(c);
                }*/
                foreach (Media m in post.Media)
                {
                    m.Post = null;
                    if (Path.Exists(m.FilePath))
                    {
                        System.IO.File.Delete(m.FilePath);
                    }
                    _context.Media.Remove(m);
                }
                post.Media.Clear();
                _context.Post.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(Guid id)
        {
          return (_context.Post?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //-------------------------------------------------------------------------------------------
        [HttpGet]
        [Route("API/Posts")]
        public async Task<JsonResult> PostsList()
        {
            if (_context.Post == null)
            {
                return Json("Error - Context not there");
            }

            var posts = await _context.Post
                .Include(u => u.User)
                .Include(u => u.Media)
                .Include(u => u.Votes)
                .Include(u => u.Comments)
                .ToListAsync();
            if (posts == null || posts.Count == 0)
            {
                return Json("Error - No Posts");

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (Post post in posts)
            {
                string postID = post.Id.ToString();                
                string user_username = post.User.Username;
                string user_profilePicture = (await _context.Media.FindAsync(post.User.MediaId)) is null? "" : (await _context.Media.FindAsync(post.User.MediaId)).FileName;
                DateTime createdDate = post.CreatedDate;
                int rating = post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == true).Count - post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == false).Count;
                int commentCount = post.Comments.Count;

                List<string> mediaList = new List<string>();
                    
                if (post.Media is not null)
                {
                    foreach (Media media in post.Media)
                    {
                        mediaList.Add(media.FileName);
                    }
                    
                }

                Dictionary<string, Object> result = new Dictionary<string, Object>
                {
                    { "postID", postID },
                    { "user_username", user_username },
                    { "user_profilePicture", user_profilePicture },
                    { "createdDate", createdDate },
                    { "rating", rating },
                    { "commentCount", commentCount },
                    { "mediaList", mediaList }


                };
                results.Add(result);
            }

            return Json(results);
        }


        [HttpGet]
        [Route("API/Posts/Comments")]
        public async Task<JsonResult> PostsList(Guid? id)
        {
            if (id == null || _context.Post == null)
            {
                return Json("Error - Context not there");
            }

            var post = await _context.Post                
                .Include(u => u.Comments)
                .FirstAsync(u => u.Id == id);
            if (post == null)
            {
                return Json("Error - No Post");

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();
            List<Comment> comments = (await _context.Comment
                .Include(c => c.User)
                .ToListAsync<Comment>()).FindAll(c => c.PostId == post.Id);
            foreach (Comment c in comments)
            {

                string commentId = c.Id.ToString();
                string user_username = c.User.Username;
                string user_profilePicture = (await _context.Media.FindAsync(c.User.MediaId)) is null ? "" : (await _context.Media.FindAsync(c.User.MediaId)).FileName;
                DateTime createdDate = c.CreatedDate;
                string commentText = c.CommentText;

               

                Dictionary<string, Object> result = new Dictionary<string, Object>
                {
                    { "commentId", commentId },
                    { "user_username", user_username },
                    { "user_profilePicture", user_profilePicture },
                    { "createdDate", createdDate },
                    { "commentText", commentText }
                    


                };
                results.Add(result);
            }

            return Json(results);
        }
    }
}
