﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Data;
using Zwitscher.Hubs;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class UsersController : Controller //Controller for Users --> Shoud be used for Admins only
    {
        private readonly ZwitscherContext _dbContext;
        private readonly IHubContext<UserHub> _hubContext;
        public UsersController(ZwitscherContext dbcontext, IHubContext<UserHub> hubContext)
        {
            _dbContext = dbcontext;
            _hubContext = hubContext;
        }

        

        // GET: Users
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> Index()
        {
            var zwitscherContext = _dbContext.User.Include(u => u.Role);
            return View(await zwitscherContext.ToListAsync());
        }
        [HttpGet]
        [Route("Users2")]
        public JsonResult JSONtest()
        {
            Console.WriteLine("called Endpoint");
            List<User> temp = _dbContext.User.ToList();
            //var zwitscherContext = _dbContext.User.Include(u => u.Role);
            return Json(temp);
        }

        // GET: Users
        [HttpGet]
        [Route("Users/List")]
        public async void ListAsync(string connectionId)
        {

            //var customContext = _context.User.Include(u => u.Role);
            var customContext = _dbContext.User
                                    .Include(u => u.ProfilePicture)
                                    .Include(u => u.FollowedBy)
                                    .Include(u => u.Following)
                                    .Include(u => u.ProfilePicture);
            List<User> UserList = await customContext.ToListAsync();
            //_hubContext.Clients.Client(connectionId).InvokeAsync("")
            return;
            
        }


        // GET: Users/Details/5
        [HttpGet]
        [Route("Users/Details")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _dbContext.User == null)
            {
                return NotFound();
            }

            var user = await _dbContext.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [HttpGet]
        [Route("Users/Create")]
        public IActionResult Create()
        {
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name");
            return View();
        }

        

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Users/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,Username,Password,Birthday,Biography,isLocked,RoleID")] User user)
        {
            Console.WriteLine("Post Endpoint called, Modelstate: " + ModelState.IsValid);
            Console.WriteLine(ModelState.Values.SelectMany(v => v.Errors));
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                _dbContext.Add(user);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            return View(user);
        }

        // GET: Users/Edit/5
        [HttpGet]
        [Route("Users/Edit")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _dbContext.User == null)
            {
                return NotFound();
            }

            var user = await _dbContext.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Users/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,LastName,FirstName,Username,Password,Birthday,Biography,isLocked,RoleID")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            return View(user);
        }

        // GET: Users/Delete/5
        [HttpGet]
        [Route("Users/Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _dbContext.User == null)
            {
                return NotFound();
            }

            var user = await _dbContext.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpDelete]
        [Route("Users/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_dbContext.User == null)
            {
                return Problem("Entity set 'ZwitscherContext.User'  is null.");
            }
            var user = await _dbContext.User.FindAsync(id);
            if (user != null)
            {
                _dbContext.User.Remove(user);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
          return (_dbContext.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
