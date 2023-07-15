using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Attributes;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    [Moderator]
    public class VotesController : Controller //Controller Class for dealing with Votes Objects
    {
        private readonly ZwitscherContext _context;

        public VotesController(ZwitscherContext context)
        {
            _context = context;
        }

        #region Base MVC Stuff for Index, Create, Edit, Delete
        //============================================= Base MVC Stuff for Index, Create ,Edit, Delete =====================================================
        // GET: Votes
        public async Task<IActionResult> Index()
        //HTTP Get Index endpoint
        //Routed to /Votes
        //Serves the View for the Vote Index page
        {
            var zwitscherContext = _context.Vote.Include(v => v.Post).Include(v => v.User);
            return View(await zwitscherContext.ToListAsync());
        }

        // GET: Votes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        //HTTP Get Details endpoint
        //Routed to /Votes/Details
        //takes an id
        //Serves the View for the Vote Details page
        {
            if (id == null || _context.Vote == null)
            {
                return NotFound();
            }

            var vote = await _context.Vote
                .Include(v => v.Post)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vote == null)
            {
                return NotFound();
            }

            return View(vote);
        }

        // GET: Votes/Create
        public IActionResult Create()
        //HTTP Get Create endpoint
        //Routed to /Votes/Create
        //Serves the View for the Vote Create page
        {
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName");
            return View();
        }

        // POST: Votes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,isUpVote,UserId,PostId")] Vote vote)
        //HTTP Post Create endpoint
        //Routed to /Votes/Create
        //takes a bindet vote Object
        //Creates a new Vote based on given properties
        {


            if (ModelState.IsValid)
            {
                vote.Id = Guid.NewGuid();
                _context.Add(vote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", vote.PostId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", vote.UserId);
            return View(vote);
        }

        // GET: Votes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        //HTTP Get Edit endpoint
        //Routed to /Votes/Edit
        //takes a id
        //serves the comment edit view
        {
            if (id == null || _context.Vote == null)
            {
                return NotFound();
            }

            var vote = await _context.Vote.FindAsync(id);
            if (vote == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", vote.PostId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", vote.UserId);
            return View(vote);
        }

        // POST: Votes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,isUpVote,UserId,PostId")] Vote vote)
        //HTTP Post Edit endpoint
        //Routed to /Votes/Edit
        //takes a id and a binded and edited vote object
        //edits the Vote with the given id based on submitted Properties
        {
            if (id != vote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoteExists(vote.Id))
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
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id", vote.PostId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "FirstName", vote.UserId);
            return View(vote);
        }

        // GET: Votes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        //HTTP Get Delete endpoint
        //Routed to /Votes/Delete
        //takes a id 
        //serves the vote delete view
        {
            if (id == null || _context.Vote == null)
            {
                return NotFound();
            }

            var vote = await _context.Vote
                .Include(v => v.Post)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vote == null)
            {
                return NotFound();
            }

            return View(vote);
        }

        // POST: Votes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        //HTTP Post Delete endpoint
        //Routed to /Votes/Delete
        //takes a id 
        //Deletes the Vote with the given id
        //redirects user to Vote index if Deletion succeded
        {
            if (_context.Vote == null)
            {
                return Problem("Entity set 'ZwitscherContext.Vote'  is null.");
            }
            var vote = await _context.Vote.FindAsync(id);
            if (vote != null)
            {
                _context.Vote.Remove(vote);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoteExists(Guid id)
        //Helper Function
        //No Endpoint
        //takes a id 
        //Checks if a Vote exists and returns result
        {
            return (_context.Vote?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        #endregion
    }
}
