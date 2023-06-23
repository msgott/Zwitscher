﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Differencing;
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
            var zwitscherContext = _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.Following)
                .Include(u => u.FollowedBy)
                .Include(u => u.ProfilePicture);
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
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
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
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,LastName,FirstName,Gender,Username,Password,Birthday,Biography,isLocked,RoleID")] User user)
        {
            ModelState.Remove("file");
            if (ModelState.IsValid)
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

                    _dbContext.Media.Add(image);
                    user.ProfilePicture = image;
                }

                user.Id = Guid.NewGuid();
                user.CreatedDate = DateTime.Now;
                _dbContext.Add(user);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
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

            var user = await _dbContext.User
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Users/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IFormFile file, [Bind("Id,LastName,FirstName,Gender,Username,Password,Birthday,Biography,isLocked,RoleID")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            
            ModelState.Remove("file");
            if (ModelState.IsValid)
            {
                
                try
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

                    _dbContext.Media.Add(image);
                    user.ProfilePicture = image;
                    }
                    user.Role = await _dbContext.Role.FindAsync(user.RoleID);
                    
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
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
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
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [Route("Users/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_dbContext.User == null)
            {
                return Problem("Entity set 'ZwitscherContext.User'  is null.");
            }
            var user = await _dbContext.User
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                user.FollowedBy.Clear();
                user.Following.Clear();
                if (user.ProfilePicture != null)
                {
                    user.ProfilePicture.User = null;
                    if (Path.Exists(user.ProfilePicture.FilePath))
                    {
                        System.IO.File.Delete(user.ProfilePicture.FilePath);
                    }
                    _dbContext.Media.Remove(user.ProfilePicture);
                    user.ProfilePicture = null;
                }
                _dbContext.User.Remove(user);
            }
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
          return (_dbContext.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }

//--------------------------------------------------------------------------------------------------------------------
        // GET: API/User?id..
        [HttpGet]
        [Route("API/User")]
        public async Task<JsonResult> Profile(Guid? id)
        {
            if (id == null || _dbContext.User == null)
            {
                return Json("Error - Context not there");
            }

            var user = await _dbContext.User
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return Json("Error - ID not found");
                
            }

            string userID = user.Id.ToString();
            string lastname = user.LastName;
            string firstname = user.FirstName;
            string username = user.Username;
            DateTime birthday = user.Birthday;
            string biography = user.Biography is null? "": user.Biography;
            string gender = user.Gender is null ? "" : user.Gender.ToString();
            int followedCount = user.Following.Count();
            int followerCount = user.FollowedBy.Count();
            string pbFileName = "";
            if (user.ProfilePicture is not null) {
                pbFileName = user.ProfilePicture.FileName; 
            }

            Dictionary<string, Object> result = new Dictionary<string, Object>
            {
                { "userID", userID },
                { "lastname", lastname },
                { "firstname", firstname },
                { "username", username },
                { "birthday", birthday },
                { "biography", biography },
                { "gender", gender },
                { "followedCount", followedCount },
                { "followerCount", followerCount },
                { "pbFileName", pbFileName }
            };

           
            return Json(result);
        }

        // GET: API/Users
        [HttpGet]
        [Route("API/Users")]
        public async Task<JsonResult> UsersList()
        {
            if (_dbContext.User == null)
            {
                return Json("Error - Context not there");
            }

            var users = await _dbContext.User
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.ProfilePicture)
                .ToListAsync();
            if (users == null || users.Count == 0)
            {
                return Json("Error - No Users");

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (User user in users) {
                string userID = user.Id.ToString();
                string lastname = user.LastName;
                string firstname = user.FirstName;
                string username = user.Username;
                DateTime birthday = user.Birthday;
                string biography = user.Biography is null ? "" : user.Biography;
                string gender = user.Gender is null ? "" : user.Gender.ToString();
                int followedCount = user.Following.Count();
                int followerCount = user.FollowedBy.Count();
                string pbFileName = "";
                if (user.ProfilePicture is not null)
                {
                    pbFileName = user.ProfilePicture.FileName;
                }

                Dictionary<string, Object> result = new Dictionary<string, Object>
                {
                    { "userID", userID },
                    { "lastname", lastname },
                    { "firstname", firstname },
                    { "username", username },
                    { "birthday", birthday },
                    { "biography", biography },
                    { "gender", gender },
                    { "followedCount", followedCount },
                    { "followerCount", followerCount },
                    { "pbFileName", pbFileName }

                };
                results.Add(result);
            }
        
            return Json(results);
        }


    }


}
