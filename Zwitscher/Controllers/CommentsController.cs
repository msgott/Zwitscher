using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ZwitscherContext _context;

        public CommentsController(ZwitscherContext context)
        {
            _context = context;
        }

        // GET: Comments
        [HttpGet]
        [Route("Comments")]
        public async Task<IActionResult> Index()
        {
            var zwitscherContext = _context.Comment.Include(c => c.Post).Include(c => c.User);
            return View(await zwitscherContext.ToListAsync());
        }

        // GET: Comments/Details/5
        [HttpGet]
        [Route("Comments/Details")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentedBy)
                .Include(c => c.commentsComment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["commentedBy"] = comment.commentedBy;
            
            return View(comment);
        }

        // GET: Comments/Create
        [HttpGet]
        [Route("Comments/Create")]
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Comments/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PostId,CommentText")] Comment comment)
        {
            ModelState.Remove("Post");
            ModelState.Remove("User");
            if (ModelState.IsValid)
            {
                comment.Id = Guid.NewGuid();
                comment.CreatedDate = DateTime.Now;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", comment.PostId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        [HttpGet]
        [Route("Comments/Edit")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", comment.PostId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Comments/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,PostId,CreatedDate,CommentText")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", comment.PostId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        [HttpGet]
        [Route("Comments/Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost]
        [Route("Comments/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Comment == null)
            {
                return Problem("Entity set 'ZwitscherContext.Comment'  is null.");
            }
            var comment = await _context.Comment
                .Include (c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentedBy)
                .Include(c => c.commentsComment)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comment != null)
            {
                if (comment.User is not null)
                {

                    //media.User.ProfilePicture = null;
                    //media.User.MediaId = null;
                    comment.User.Comments.Remove(comment);
                    comment.User = null;
                }
                if (comment.Post is not null)
                {
                    comment.Post.Comments.Remove(comment);
                    comment.Post = null;
                }

                _context.Comment.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(Guid id)
        {
          return (_context.Comment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        //---------------------------------------------API------------------------------------------------
        [HttpPost]
        [Route("API/Comments/Edit")]
        
        public async Task<IActionResult> EditComment(Guid id, string CommentText )
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (id == null || _context.Comment == null || CommentText is null || CommentText.Length < 1)
            {
                return BadRequest();
            }


            var c = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
            .ThenInclude(c => c.User)
                .FirstAsync(p => p.Id == id);
            if (c == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            
            if (c.UserId != userID) return Unauthorized();

            c.CommentText = CommentText;
            _context.Update(c);
            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpGet]
        [Route("API/Comments/Comments")]
        public async Task<ActionResult> CommentsList(Guid? id)
        {
            if (id == null || _context.Comment == null)
            {
                return BadRequest();
            }

            var c = await _context.Comment
                .Include(u => u.commentedBy)
                .ThenInclude(c => c.User)
                .FirstAsync(u => u.Id == id);
            if (c == null)
            {
                return NotFound();

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();
            List<Comment> comments = c.commentedBy;
            foreach (Comment comment in comments)
            {

                string commentId = comment.Id.ToString();
                string user_username = comment.User.Username;
                string user_profilePicture = (await _context.Media.FindAsync(comment.User.MediaId)) is null ? "" : (await _context.Media.FindAsync(comment.User.MediaId)).FileName;
                DateTime createdDate = comment.CreatedDate;
                string commentText = comment.CommentText;
                bool loggedInUserIsCreator = comment.UserId.ToString() == HttpContext.Session.GetString("UserId");



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
        [Route("API/Comments/Comment/Add")]
        public async Task<ActionResult> AddCommentToComment(Guid? commentId, string CommentText = "") //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (commentId == null || _context.Comment == null || CommentText is null)
            {
                return BadRequest();
            }


            var comment = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
                .ThenInclude(c => c.User)
                .FirstAsync(p => p.Id == commentId);
            if (comment == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            Comment c = new Comment();
            c.Id = Guid.NewGuid();
            c.CommentText = CommentText;
            c.UserId = userID;
            c.commentsCommentId = comment.Id;

            c.commentsComment = comment;
            _context.Add(c);
            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpPost]
        [Route("API/Comments/Comment/Remove")]
        public async Task<ActionResult> RemoveCommentFromPost(Guid commentId, Guid commentToRemoveId) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (commentId == null || _context.Comment == null)
            {
                return BadRequest();
            }


            var c = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
                .ThenInclude(c => c.User)
                .FirstAsync(p => p.Id == commentId);
            if (c == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            Comment comment = c.commentedBy.ToList().Find(c => c.Id == commentToRemoveId);
            if (comment is null) return BadRequest();
            if (comment.UserId != userID && c.UserId != userID) return Unauthorized();

            c.commentedBy.Remove(comment);
            _context.Remove(comment);
            await _context.SaveChangesAsync();


            return Ok();
        }
    }

}
