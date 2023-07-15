using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Attributes;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class CommentsController : Controller //Controller Class for dealing with Comments Objects
    {
        private readonly ZwitscherContext _context;

        public CommentsController(ZwitscherContext context)
        {
            _context = context; //Dependency Injection of DBContext
        }
        #region Base MVC Stuff for Index, Create, Edit, Delete
        //============================================= Base MVC Stuff for Index, Create ,Edit, Delete =====================================================
        // GET: Comments
        [Moderator]
        [HttpGet]
        [Route("Comments")]
        //HTTP Get Index endpoint
        //Routed to /Comments
        //Serves the View for the Comment Index page
        public async Task<IActionResult> Index()
        {
            var zwitscherContext = _context.Comment.Include(c => c.Post).Include(c => c.User).OrderByDescending(u => u.CreatedDate);
            return View(await zwitscherContext.ToListAsync());
        }

        // GET: Comments/Details/5
        [Moderator]
        [HttpGet]
        [Route("Comments/Details")]
        public async Task<IActionResult> Details(Guid? id)
        //HTTP Get Details endpoint
        //Routed to /Comments/Details
        //Takes a commentId as input
        //Serves the View for the Comment Details page if the id was found
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
        [Moderator]
        [HttpGet]
        [Route("Comments/Create")]
        public IActionResult Create()
        //HTTP Get Create endpoint
        //Routed to /Comments/Create
        //Serves the View for the Comment Create page
        {
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Comments/Create
        [Moderator]
        [HttpPost]
        [Route("Comments/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PostId,CommentText")] Comment comment)
        //HTTP Comment Create endpoint
        //Routed to /Comments/Create
        //Takes a Binded CommentObject from MVC View
        //Creates a new Comment with the given Properties

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
        [Moderator]
        [HttpGet]
        [Route("Comments/Edit")]
        public async Task<IActionResult> Edit(Guid? id)
        //HTTP Get Edit endpoint
        //Routed to /Comments/Edit
        //Takes a commentId
        //Serves the Comment Edit View based on given Id
        //serves Edit Comment View

        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
                .FirstOrDefaultAsync(c => c.Id == id);
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
        [Moderator]
        [HttpPost]
        [Route("Comments/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,PostId,CreatedDate,CommentText")] Comment comment)
        //HTTP Post Edit endpoint
        //Routed to /Comments/Edit
        //Takes a edited comment Object
        //Updates the comment based on given comment Object
        //Redirects user to comment index view if Edition was successful

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
        [Moderator]
        [HttpGet]
        [Route("Comments/Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        //HTTP Get Delete endpoint
        //Routed to /Comments/Delete
        //Takes a commentId
        //Deletes the comment based on the given id
        //serves Delete Comment View
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
        [Moderator]
        [HttpPost]
        [Route("Comments/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        //HTTP Post Delete endpoint
        //Routed to /Comments/Delete
        //Takes a commentId
        //Deletes the comment based on the given id
        //Redirects User to comment Index after Deletion
        {
            if (_context.Comment == null)
            {
                return Problem("Entity set 'ZwitscherContext.Comment'  is null.");
            }
            var comment = _context.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentedBy)
                .FirstOrDefault(c => c.Id == id);
            if (comment != null)
            {

                RecursiveDelete(comment);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void RecursiveDelete(Comment parent)
        //Helper function
        //Not an Endpoint
        //deletes a given Comment and all of its child Comments recursively
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


        private bool CommentExists(Guid id)
        //Helper function
        //Not an Endpoint
        //takes commentId
        //Checks if a comment exist based on given id and returns result
        {
            return (_context.Comment?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        #endregion

        #region MVC Comment Comment
        //============================================= MVC Comment Comment =====================================================


        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveComment(Guid commentID, Guid commentToRemoveId)
        //HTTP Post Endpoint for serving the PartialView for the RemoveComment modal
        //takes commentId and the id of the comment to be removed
        //servers Partialview
        {
            var comment = await _context.Comment.FirstOrDefaultAsync(c => c.Id == commentID);


            ViewData["commentToRemove"] = commentToRemoveId;
            return PartialView("PopupRemoveComment", comment);

        }


        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveCommentFromComment(Guid commentID, Guid commentToRemoveId)
        //HTTP Post Endpoint for deleting child comment after PopupRemoveComment was submitted
        //takes commentId and the id of the comment to be removed
        //deletes Comment
        //redirects user to parents edit view
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

            var comment = await _context.Comment
                 .Include(p => p.Post)
                 .Include(p => p.User)
                 .Include(p => p.commentedBy)
                 .Include(p => p.commentsComment)
                 .FirstOrDefaultAsync(p => p.Id == commentID);
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", comment.PostId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", comment.UserId);

            return RedirectToAction(nameof(Edit), comment);
        }
        #endregion
        #region API
        //---------------------------------------------API------------------------------------------------
        [HttpPost]
        [Route("API/Comments/Edit")]
        public async Task<IActionResult> EditComment(Guid id, string CommentText)
        //HTTP Post Edit API endpoint
        //Routed to API/Comments/Edit
        //Takes a commentId and a new CommentText
        //Edits the comment with the given Properties
        //Return HTTP Resultcodes based on result
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (id == Guid.Empty || _context.Comment == null || CommentText is null || CommentText.Length < 1)
            {
                return BadRequest();
            }


            var c = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
            .ThenInclude(c => c!.User)
                .FirstAsync(p => p.Id == id);
            if (c == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);

            if (c.UserId != userID) return Unauthorized();

            c.CommentText = CommentText;
            _context.Update(c);
            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpGet]
        [Route("API/Comments/Comments")]
        public async Task<ActionResult> CommentsList(Guid? id)
        //HTTP Get Comments API endpoint
        //Routed to API/Comments/Comments
        //Takes a commentId 
        //Searches for child comments of Comment with the given id and returns it as json
        //Return HTTP Resultcodes based on result
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
            List<Comment> comments = c.commentedBy.OrderByDescending(c => c.CreatedDate).ToList();
            foreach (Comment comment in comments)
            {

                string commentId = comment.Id.ToString();
                string user_username = comment.User.Username;
                string user_profilePicture = (await _context.Media.FindAsync(comment.User.MediaId)) is null ? "" : (await _context.Media.FindAsync(comment.User.MediaId))!.FileName;
                DateTime createdDate = comment.CreatedDate;
                string commentText = comment.CommentText;
                bool loggedInUserIsCreator = comment.UserId.ToString() == HttpContext.Session.GetString("UserId");



                Dictionary<string, Object> result = new Dictionary<string, Object>
                {
                    { "commentId", commentId },
                    { "user_username", user_username },
                    { "user_profilePicture", user_profilePicture },
                    { "createdDate", createdDate.ToString("dd.MM.yyyy") },
                    { "commentText", commentText },
                    { "loggedInUserIsCreator", loggedInUserIsCreator }



                };
                results.Add(result);
            }

            return Json(results);
        }

        [HttpPost]
        [Route("API/Comments/Comment/Add")]
        public async Task<ActionResult> AddCommentToComment(Guid? commentId, string CommentText = "")
        //HTTP Post Add Comment to Comment API endpoint
        //Routed to API/Comments/Comment/Add
        //Takes a commentId and a commentText
        //Appends a new comment to the comment with the given id
        //Return HTTP Resultcodes based on result
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (commentId == null || _context.Comment == null || CommentText is null)
            {
                return BadRequest();
            }


            var comment = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
                .ThenInclude(c => c!.User)
                .FirstAsync(p => p.Id == commentId);
            if (comment == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            Comment c = new Comment();
            c.Id = Guid.NewGuid();
            c.CommentText = CommentText;
            c.UserId = userID;
            c.commentsCommentId = comment.Id;
            c.CreatedDate = DateTime.Now;

            c.commentsComment = comment;
            _context.Add(c);
            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpPost]
        [Route("API/Comments/Comment/Remove")]
        public async Task<ActionResult> RemoveCommentFromComment1(Guid commentId, Guid commentToRemoveId)
        //HTTP Post Remove Comment from Comment API endpoint
        //Routed to API/Comments/Comment/Remove
        //Takes a commentId and a id of the child comment to be removed
        //Deletes the comment with commentToRemoveId from comment with commentId
        //Return HTTP Resultcodes based on result
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (commentId == Guid.Empty || _context.Comment == null)
            {
                return BadRequest();
            }


            var c = await _context.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
                .ThenInclude(c => c!.User)
                .FirstAsync(p => p.Id == commentId);
            if (c == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);

            var comment = _context.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentedBy)
                .Include(c => c.commentsComment)
                .FirstOrDefault(c => c.Id == commentToRemoveId);
            if (comment is null) return BadRequest();
            if (comment != null)
            {
                if (comment.UserId != userID && c.UserId != userID) return Unauthorized();
                RecursiveDelete(comment);

            }

            await _context.SaveChangesAsync();




            return Ok();
        }

        [HttpDelete]
        [Route("API/Comments/Remove")]
        public async Task<ActionResult> RemoveComment1(Guid commentToRemoveId)
        //HTTP Delete Remove Comment API endpoint
        //Routed to API/Comments/Remove
        //Takes a a id of the comment to be removed
        //Deletes the comment the id 
        //Return HTTP Resultcodes based on result
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _context.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (_context.Comment == null)
            {
                return BadRequest();
            }



            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);

            var comment = _context.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentedBy)
                .Include(c => c.commentsComment)
                .FirstOrDefault(c => c.Id == commentToRemoveId);
            if (comment is null) return BadRequest();
            if (comment != null)
            {
                if (comment.UserId != userID) return Unauthorized();
                RecursiveDelete(comment);

            }

            await _context.SaveChangesAsync();








            return Ok();
        }
        #endregion
    }

}
