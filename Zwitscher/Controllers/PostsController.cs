using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text.Json;
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
            Console.WriteLine(HttpContext.Session.GetString("UserId"));
            var zwitscherContext = _context.Post
                .Include(p => p.User)
                .Include(p => p.Votes)
                .Include(p => p.Comments)
                .Include(p => p.Media)
                .Include(p => p.retweets);


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
                .Include(p => p.retweets)
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

            ViewData["RezwitscherId"] = new SelectList(_context.Post, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile[] files, [Bind("Id,TextContent,IsPublic,UserId,retweetsID")] Post post)
        {

            ModelState.Remove("files");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("User");
            ModelState.Remove("retweets");
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
            ViewData["RezwitscherId"] = new SelectList(_context.Post, "Id", "Id", post.retweetsID);
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
                .Include(p => p.retweets)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            List<Post> tempList = new List<Post> { post };



            ViewData["Votes"] = post.Votes;
            ViewData["Comments"] = post.Comments;
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            ViewData["RezwitscherId"] = new SelectList(_context.Post.ToList().Except(tempList), "Id", "Id", post.retweetsID);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IFormFile[] files, [Bind("Id,CreatedDate,TextContent,IsPublic,UserId,retweetsID")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            ModelState.Remove("files");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("User");
            ModelState.Remove("retweets");
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
            List<Post> tempList = new List<Post> { post };


            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            ViewData["RezwitscherId"] = new SelectList(_context.Post.ToList().Except(tempList), "Id", "Id", post.retweetsID);
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
                .Include(post => post.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                foreach (Comment com in post.Comments)
                {
                    var comment = _context.Comment
                    .Include(c => c.Post)
                    .Include(c => c.User)
                    .Include(c => c.commentedBy)
                    .FirstOrDefault(c => c.Id == com.Id);
                    if (comment is null) return BadRequest();
                    if (comment != null)
                    {

                        RecursiveDelete(comment);

                    }


                }
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
        //-----------------------------------------------MVC User Details----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupPostDetails(Guid postID)
        {
            if (postID == Guid.Empty || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Votes)
                .Include(p => p.retweets)
                .FirstOrDefaultAsync(m => m.Id == postID);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["Votes"] = post.Votes;
            ViewData["Comments"] = post.Comments;


            return PartialView("PopupPostDetails", post);



        }
        //-----------------------------------------------MVC Post Media ----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupAddMedia(Guid postID)
        {

            if (postID == Guid.Empty || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Media)
                .FirstOrDefaultAsync(m => m.Id == postID);
            if (post == null)
            {
                return NotFound();
            }

            return PartialView("PopupAddMedia", post);
        }

        [HttpPost]
        public async Task<IActionResult> PopupRemoveMedia(Guid postID, Guid mediaToRemoveId)
        {
            if (postID == Guid.Empty || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Media)
                .FirstOrDefaultAsync(m => m.Id == postID);
            if (post == null)
            {
                return NotFound();
            }



            ViewData["mediatoremove"] = mediaToRemoveId;
            return PartialView("PopupRemoveMedia", post);

        }

        [HttpPost]
        public async Task<ActionResult> AddMediaToPost(Guid postID, IFormFile[] files) //Just for the MVC Frontend 
        {

            if (postID == Guid.Empty || _context.Post == null || files == null)
            {
                return BadRequest();
            }

            var post = await _context.Post
                 .Include(p => p.Media)
                 .Include(p => p.retweets)
                 .Include(p => p.Comments)
                 .Include(p => p.Votes)
                 .FirstOrDefaultAsync(p => p.Id == postID);
            if (post == null)
            {
                return NotFound();
            }

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




            List<Post> tempList = new List<Post> { post };



            ViewData["Votes"] = post.Votes;
            ViewData["Comments"] = post.Comments;
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            ViewData["RezwitscherId"] = new SelectList(_context.Post.ToList().Except(tempList), "Id", "Id", post.retweetsID);
            return RedirectToAction(nameof(Edit), post);

        }

        [HttpPost]
        public async Task<ActionResult> RemoveMediaFromPost(Guid postID, Guid mediaToRemoveId) //Just for the MVC Frontend 
        {

            if (mediaToRemoveId == Guid.Empty || _context.Post == null)
            {
                return BadRequest();
            }


            var media = await _context.Media
                .Include(m => m.User)
                .Include(m => m.Post)
                .FirstOrDefaultAsync(m => m.Id == mediaToRemoveId);
            if (media != null)
            {
                if (media.User is not null)
                {

                    //media.User.ProfilePicture = null;
                    //media.User.MediaId = null;
                    media.User.ProfilePicture = null;
                    media.User = null;

                }
                if (media.Post is not null)
                {

                    media.Post = null;

                }
                if (Path.Exists(media.FilePath))
                {
                    System.IO.File.Delete(media.FilePath);
                }
                _context.Media.Remove(media);
            }

            await _context.SaveChangesAsync();


            var post = await _context.Post
                 .Include(p => p.Media)
                 .Include(p => p.retweets)
                 .Include(p => p.Comments)
                 .Include(p => p.Votes)
                 .FirstOrDefaultAsync(p => p.Id == postID);
            if (post == null)
            {
                return NotFound();
            }
            List<Post> tempList = new List<Post> { post };



            ViewData["Votes"] = post.Votes;
            ViewData["Comments"] = post.Comments;
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            ViewData["RezwitscherId"] = new SelectList(_context.Post.ToList().Except(tempList), "Id", "Id", post.retweetsID);
            return RedirectToAction(nameof(Edit), post);
        }

        //-----------------------------------------------MVC Post Comment----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupRemoveComment(Guid postID, Guid commentToRemoveId)
        {
            var post = await _context.Post.FirstOrDefaultAsync(post => post.Id == postID);


            ViewData["commentToRemove"] = commentToRemoveId;
            return PartialView("PopupRemoveComment", post);

        }



        [HttpPost]
        public async Task<ActionResult> RemoveCommentFromPost(Guid postID, Guid commentToRemoveId) //Just for the MVC Frontend 
        {

            if (commentToRemoveId == Guid.Empty || _context.Post == null)
            {
                return BadRequest();
            }


            var CommentToRemove = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
                .Include(u => u.Post)
                .Include(u => u.User)
                .FirstAsync(p => p.Id == commentToRemoveId);
            if (CommentToRemove == null)
            {
                return NotFound();

            }

            RecursiveDelete(CommentToRemove);

            await _context.SaveChangesAsync();


            var post = await _context.Post
                 .Include(p => p.Media)
                 .Include(p => p.retweets)
                 .Include(p => p.Comments)
                 .Include(p => p.Votes)
                 .FirstOrDefaultAsync(p => p.Id == postID);
            if (post == null)
            {
                return NotFound();
            }
            List<Post> tempList = new List<Post> { post };



            ViewData["Votes"] = post.Votes;
            ViewData["Comments"] = post.Comments;
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            ViewData["RezwitscherId"] = new SelectList(_context.Post.ToList().Except(tempList), "Id", "Id", post.retweetsID);
            return RedirectToAction(nameof(Edit), post);
        }
        //-----------------------------------------------MVC User Vote----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupRemoveVote(Guid postID, Guid voteToRemoveId)
        {
            var user = await _context.Post.FirstOrDefaultAsync(user => user.Id == postID);


            ViewData["voteToRemove"] = voteToRemoveId;
            return PartialView("PopupRemoveVote", user);

        }



        [HttpPost]
        public async Task<ActionResult> RemoveVoteFromPost(Guid postID, Guid voteToRemoveId) //Just for the MVC Frontend 
        {

            if (voteToRemoveId == Guid.Empty || _context.User == null)
            {
                return BadRequest();
            }


            var voteToRemove = await _context.Vote
                .Include(u => u.User)
                .Include(u => u.Post)
                .FirstAsync(p => p.Id == voteToRemoveId);
            if (voteToRemove == null)
            {
                return NotFound();

            }


            _context.Remove(voteToRemove);
            await _context.SaveChangesAsync();


            var post = await _context.Post
                 .Include(p => p.Media)
                 .Include(p => p.retweets)
                 .Include(p => p.Comments)
                 .Include(p => p.Votes)
                 .FirstOrDefaultAsync(p => p.Id == postID);
            if (post == null)
            {
                return NotFound();
            }
            List<Post> tempList = new List<Post> { post };



            ViewData["Votes"] = post.Votes;
            ViewData["Comments"] = post.Comments;
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", post.UserId);
            ViewData["RezwitscherId"] = new SelectList(_context.Post.ToList().Except(tempList), "Id", "Id", post.retweetsID);
            return RedirectToAction(nameof(Edit), post);
        }

        //----------------------------------------- API --------------------------------------------------
        [HttpGet]
        [Route("API/Posts")]
        public async Task<ActionResult> PostsList()
        {
            if (_context.Post == null)
            {
                return BadRequest();
            }
            var posts = new List<Post>();
            if (HttpContext.Session.GetString("RoleName") == "Administrator" || HttpContext.Session.GetString("RoleName") == "Moderator")
            {
                posts = await _context.Post
                .Include(u => u.User)
                .Include(u => u.Media)
                .Include(u => u.Votes)
                .ThenInclude(v => v.User)
                .Include(u => u.Comments)
                .Include(p => p.retweets)
                .ToListAsync();
            }
            else
            {
                posts = (await _context.Post
                .Include(u => u.User)
                .Include(u => u.Media)
                .Include(u => u.Votes)
                .ThenInclude(v => v.User)
                .Include(u => u.Comments)
                .Include(p => p.retweets)
                .ToListAsync()).FindAll(p => p.IsPublic == true);

                if (HttpContext.Session.GetString("UserId") is not null)
                {
                    User usr = _context.User.Find(Guid.Parse(HttpContext.Session.GetString("UserId")!))!;
                    if (usr != null)
                    {
                        var userSpecificPosts = (await _context.Post
                        .Include(u => u.User)
                        .ThenInclude(u => u.FollowedBy)
                        .Include(u => u.User)
                        .ThenInclude(u => u.Blocking)
                        .Include(u => u.User)
                        .ThenInclude(u => u.BlockedBy)
                        .Include(u => u.Media)
                        .Include(u => u.Votes)
                        .ThenInclude(v => v.User)
                        .Include(u => u.Comments)
                        .Include(p => p.retweets)
                        .ToListAsync()).FindAll(p => p.IsPublic == false && p.User.FollowedBy.Contains(usr) && !p.User.Blocking.Contains(usr) && !p.User.BlockedBy.Contains(usr));

                        posts = posts.Union(userSpecificPosts).ToList();
                    }
                }
            }





            if (posts == null || posts.Count == 0)
            {
                return NotFound();

            }

            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId") is null ? Guid.NewGuid().ToString() : HttpContext.Session.GetString("UserId")!);
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (Post post in posts)
            {
                string postID = post.Id.ToString();
                string user_username = post.User.Username;
                string user_profilePicture = (await _context.Media.FindAsync(post.User.MediaId)) is null ? "" : (await _context.Media.FindAsync(post.User.MediaId))!.FileName;
                DateTime createdDate = post.CreatedDate;
                int rating = post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == true).Count - post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == false).Count;
                int commentCount = post.Comments.Count;
                string postText = post.TextContent;
                bool currentUserVoted = (post.Votes.ToList().Find(v => v.User.Id == userID) is not null && post.Votes.ToList().Find(v => v.User.Id == userID)!.User.Id == userID);
                string userVoteIsUpvote = currentUserVoted ? (post.Votes.ToList().Find(v => v.User.Id == userID)!.isUpVote ? "true" : "false") : "null";
                string retweetsPost = post.retweetsID.ToString()!;
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
                    { "currentUserVoted", currentUserVoted },
                    { "userVoteIsUpvote", userVoteIsUpvote },
                    { "mediaList", mediaList },
                    { "postText", postText },
                    { "retweetsPost", retweetsPost }



                };
                results.Add(result);
            }

            return Json(results);
        }
        [HttpGet]
        [Route("API/Post")]
        public async Task<ActionResult> getSinglePost(Guid id)
        {
            if (_context.Post == null)
            {
                return BadRequest();
            }

            var post = await _context.Post
                .Include(u => u.User)
                .Include(u => u.Media)
                .Include(u => u.Votes)
                .ThenInclude(v => v.User)
                .Include(u => u.Comments)
                .Include(p => p.retweets)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null || post.Id != id)
            {
                return NotFound();

            }

            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId") is null ? Guid.NewGuid().ToString() : HttpContext.Session.GetString("UserId")!);

            string postID = post.Id.ToString();
            string user_username = post.User.Username;
            string user_profilePicture = (await _context.Media.FindAsync(post.User.MediaId)) is null ? "" : (await _context.Media.FindAsync(post.User.MediaId))!.FileName;
            DateTime createdDate = post.CreatedDate;
            int rating = post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == true).Count - post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == false).Count;
            int commentCount = post.Comments.Count;
            string postText = post.TextContent;
            bool currentUserVoted = (post.Votes.ToList().Find(v => v.User.Id == userID) is not null && post.Votes.ToList().Find(v => v.User.Id == userID)!.User.Id == userID);
            string userVoteIsUpvote = currentUserVoted ? (post.Votes.ToList().Find(v => v.User.Id == userID)!.isUpVote ? "true" : "false") : "null";
            string retweetsPost = post.retweetsID.ToString()!;
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
                    { "currentUserVoted", currentUserVoted },
                    { "userVoteIsUpvote", userVoteIsUpvote },
                    { "mediaList", mediaList },
                    { "postText", postText },
                    { "retweetsPost", retweetsPost }



                };



            return Json(result);
        }
        [HttpPost]
        [Route("API/Posts/Add")]
        public async Task<IActionResult> CreatePost(IFormFile[] files, [Bind("Id,TextContent,IsPublic,UserId,retweetsID")] Post post) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (_context.Post == null)
            {
                return BadRequest();
            }

            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            post.UserId = userID;
            ModelState.Remove("files");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("retweets");
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
                return Ok();
            }

            return ValidationProblem();
        }

        [HttpPost]
        [Route("API/Posts/Edit")]
        public async Task<IActionResult> EditPost(Guid postID, string TextContent, bool IsPublic, Guid? retweetsID) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            Guid _userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);

            if (postID == Guid.Empty || _context.Post == null || TextContent == "" || TextContent == null)
            {
                return BadRequest();
            }
            Post post = _context.Post.Find(postID);
            if (post == null) return NotFound();
            if (post.UserId != _userID) return Unauthorized();

            post.TextContent = TextContent;
            post.IsPublic = IsPublic;
            post.retweetsID = retweetsID;

            _context.Update(post);
            await _context.SaveChangesAsync();
            return Ok();




        }
        // POST: Posts/Delete/5
        [HttpDelete]
        [Route("API/Posts/Remove")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (_context.Post == null)
            {
                return BadRequest();
            }

            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);


            var post = await _context.Post
                .Include(post => post.User)
                .Include(post => post.Comments)
                .Include(post => post.Media)
                .Include(post => post.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post is null) return NotFound();
            if (post.Id != id || post.UserId != userID) return Unauthorized();


            foreach (Comment com in post.Comments)
            {
                var comment = _context.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentedBy)
                .FirstOrDefault(c => c.Id == com.Id);
                if (comment is null) return BadRequest();
                if (comment != null)
                {
                    if (comment.UserId != userID && post.UserId != userID) return Unauthorized();
                    RecursiveDelete(comment);

                }


            }

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


            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet]
        [Route("API/Posts/Comments")]
        public async Task<ActionResult> PostsList(Guid? id)
        {
            if (id == null || _context.Post == null)
            {
                return BadRequest();
            }

            var post = await _context.Post
                .Include(u => u.Comments)
                .FirstAsync(u => u.Id == id);
            if (post == null)
            {
                return NotFound();

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();
            List<Comment> comments = (await _context.Comment
                .Include(c => c.User)
                .ToListAsync<Comment>()).FindAll(c => c.PostId == post.Id);
            foreach (Comment c in comments)
            {

                string commentId = c.Id.ToString();
                string user_username = c.User.Username;
                string user_profilePicture = (await _context.Media.FindAsync(c.User.MediaId)) is null ? "" : (await _context.Media.FindAsync(c.User.MediaId))!.FileName;
                DateTime createdDate = c.CreatedDate;
                string commentText = c.CommentText;
                bool loggedInUserIsCreator = c.UserId.ToString() == HttpContext.Session.GetString("UserId");



                Dictionary<string, Object> result = new Dictionary<string, Object>
                {
                    { "commentId", commentId },
                    { "user_username", user_username },
                    { "user_profilePicture", user_profilePicture },
                    { "createdDate", createdDate },
                    { "commentText", commentText },
                    { "loggedInUserIsCreator", loggedInUserIsCreator }



                };
                results.Add(result);
            }

            return Json(results);
        }

        [HttpPost]
        [Route("API/Posts/Vote")]
        public async Task<ActionResult> ManageVotes(Guid? postId, bool IsUpVote = true) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (postId == null || _context.Post == null)
            {
                return BadRequest();
            }


            var post = await _context.Post
                .Include(u => u.Votes)
                .ThenInclude(v => v.User)
                .FirstAsync(p => p.Id == postId);
            if (post == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            List<Vote> postVotes = post.Votes.ToList();
            if (postVotes.Find(v => v.User.Id == userID) != null)
            {//User hat schon Vote für diesen Post abgegeben
                Vote vote = postVotes.Find(v => v.User.Id == userID)!;

                if (vote.isUpVote && IsUpVote)
                {
                    _context.Remove(vote);
                    await _context.SaveChangesAsync();
                }
                else if (!vote.isUpVote && IsUpVote)
                {
                    vote.isUpVote = true;
                    _context.Update(vote);
                    await _context.SaveChangesAsync();
                }
                else if (vote.isUpVote && !IsUpVote)
                {
                    vote.isUpVote = false;
                    _context.Update(vote);
                    await _context.SaveChangesAsync();
                }
                else if (!vote.isUpVote && !IsUpVote)
                {
                    _context.Remove(vote);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {//User hat noch keinen Vote zu diesem Post abgegeben
                Vote vote = new Vote();
                vote.Id = Guid.NewGuid();
                vote.isUpVote = IsUpVote;
                vote.PostId = post.Id;
                vote.UserId = userID;
                _context.Add(vote);
                await _context.SaveChangesAsync();
            }



            return Ok();
        }

        [HttpPost]
        [Route("API/Posts/Comment/Add")]
        public async Task<ActionResult> AddCommentToPost(Guid? postId, string CommentText = "") //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (postId == null || _context.Post == null || CommentText is null)
            {
                return BadRequest();
            }


            var post = await _context.Post
                .Include(u => u.Comments)
                .ThenInclude(c => c.User)
                .FirstAsync(p => p.Id == postId);
            if (post == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            Comment comment = new Comment();
            comment.Id = Guid.NewGuid();
            comment.CommentText = CommentText;
            comment.CreatedDate = DateTime.Now;
            comment.UserId = userID;
            comment.PostId = post.Id;

            comment.Post = post;
            _context.Add(comment);
            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpPost]
        [HttpDelete]
        [Route("API/Posts/Comment/Remove")]
        public async Task<ActionResult> RemoveCommentFromPost1(Guid postId, Guid commentId) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (postId == Guid.Empty || _context.Post == null)
            {
                return BadRequest();
            }


            var post = await _context.Post
                .Include(u => u.Comments)
                .ThenInclude(c => c.User)
                .Include(u => u.Comments)
                .ThenInclude(c => c.commentedBy)
                .Include(u => u.Comments)
                .ThenInclude(c => c.commentsComment)
                .FirstAsync(p => p.Id == postId);
            if (post == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);


            var comment = _context.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentedBy)
                .FirstOrDefault(c => c.Id == commentId);
            if (comment is null) return BadRequest();
            if (comment != null)
            {
                if (comment.UserId != userID && post.UserId != userID) return Unauthorized();
                RecursiveDelete(comment);

            }

            await _context.SaveChangesAsync();



            return Ok();
        }
        //-----------------------------------------------API Post Media ----------------------------------------------------------------------





        [HttpPost]
        [Route("API/Posts/Media/Add")]
        public async Task<ActionResult> AddMediaToPost1(Guid postID, IFormFile[] files)
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            if (postID == Guid.Empty || _context.Post == null || files == null)
            {
                return BadRequest();
            }

            var post = await _context.Post
                 .Include(p => p.Media)
                 .Include(p => p.retweets)
                 .Include(p => p.Comments)
                 .Include(p => p.Votes)
                 .FirstOrDefaultAsync(p => p.Id == postID);
            if (post.UserId != userID) return Unauthorized();
            if (post == null)
            {
                return NotFound();
            }

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





            return Ok();

        }

        [HttpPost]
        [Route("API/Posts/Media/Remove")]
        public async Task<ActionResult> RemoveMediaFromPost1(Guid postID, Guid mediaToRemoveId)
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            if (mediaToRemoveId == Guid.Empty || _context.Post == null)
            {
                return BadRequest();
            }

            var post = await _context.Post
                 .Include(p => p.Media)
                 .Include(p => p.retweets)
                 .Include(p => p.Comments)
                 .Include(p => p.Votes)
                 .FirstOrDefaultAsync(p => p.Id == postID);
            if (post == null)
            {
                return NotFound();
            }

            if (post.UserId != userID) return Unauthorized();
            var media = await _context.Media
                .Include(m => m.User)
                .Include(m => m.Post)
                .FirstOrDefaultAsync(m => m.Id == mediaToRemoveId);
            if (media != null)
            {

                if (media.Post is not null)
                {

                    media.Post = null;

                }
                if (Path.Exists(media.FilePath))
                {
                    System.IO.File.Delete(media.FilePath);
                }
                _context.Media.Remove(media);
            }
            else return NotFound();

            await _context.SaveChangesAsync();




            return Ok();
        }
        private void RecursiveDelete(Comment parent)
        {
            if (parent.commentedBy != null && parent.commentedBy.Count > 0)
            {
                var children = _context.Comment
                    .Include(x => x.commentedBy)
                    .Where(x => x.commentsCommentId == parent.Id).ToList();

                foreach (var child in children)
                {
                    RecursiveDelete(child);
                }
            }

            _context.Remove(parent);
        }
    }
}
