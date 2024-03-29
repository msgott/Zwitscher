﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Zwitscher.Attributes;
using Zwitscher.Data;
using Zwitscher.Hubs;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class UsersController : Controller //Controller Class for dealing with User Objects
    {
        private readonly ZwitscherContext _dbContext;
        private readonly IHubContext<UserHub> _hubContext;
        public UsersController(ZwitscherContext dbcontext, IHubContext<UserHub> hubContext)
        {
            _dbContext = dbcontext; //Dependency Injection of DBContext
            _hubContext = hubContext; //Dependency Injection of SignalR Hubcontext
        }


        #region Base MVC Stuff for Index, Create, Edit, Delete
        //============================================= Base MVC Stuff for Index, Create, Edit, Delete =====================================================

        // GET: Users
        [Moderator]
        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> Index()
        //HTTP Get Index endpoint
        //Serves the View for the User Index page
        {
            var zwitscherContext = _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.Following)
                .Include(u => u.FollowedBy)
                .Include(u => u.ProfilePicture).OrderByDescending(u=>u.CreatedDate);
            return View(await zwitscherContext.ToListAsync());
        }
        [HttpGet]
        [Route("Users2")]
        public JsonResult JSONtest()
        //HTTP Get Test endpoint
        //Not in Use
        //Just for test purposes
        //serves all Users as Json
        {
            Console.WriteLine("called Endpoint");
            List<User> temp = _dbContext.User.ToList();
            //var zwitscherContext = _dbContext.User.Include(u => u.Role);
            return Json(temp);
        }

        // GET: Users
        [Moderator]
        [HttpGet]
        [Route("Users/List")]
        public async void ListAsync(string connectionId)
        //HTTP Get Test endpoint
        //Not in Use
        //Just for test purposes
        //serves all Users with some Relationships as Json
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
        [Moderator]
        [HttpGet]
        [Route("Users/Details")]
        public async Task<IActionResult> Details(Guid? id)
        //HTTP Get Details endpoint
        //takes id
        //serves user Details view
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
                .Include(u=> u.Blocking)
                .Include(u => u.BlockedBy)
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
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
            return View(user);
        }

        // GET: Users/Create
        [Moderator]
        [HttpGet]
        [Route("Users/Create")]
        public IActionResult Create()
        //HTTP Get Create endpoint
        //serves user Create view
        {
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name");
            return View();
        }



        // POST: Users/Create
        [Moderator]
        [HttpPost]
        [Route("Users/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,LastName,FirstName,Gender,Username,Password,Birthday,Biography,isLocked,RoleID")] User user)
        //HTTP User Create endpoint
        //Routed to /Users/Create
        //Takes a a file and a Binded CommentObject from MVC View
        //Creates a new User with the given Properties
        
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
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _dbContext.Add(user);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            return View(user);
        }

        // GET: Users/Edit/5
        [Moderator]
        [HttpGet]
        [Route("Users/Edit")]
        //[Admin] //for Development deactivated
        public async Task<IActionResult> Edit(Guid? id)
        //HTTP Get User Edit endpoint
        //Routed to /Users/Edit
        //Takes a id
        //serves the user edit View based on giben id
        {
            //Role Based Authentication for Mod and Admin
            //if (HttpContext.Session.GetString("RoleName") != "Moderator" && HttpContext.Session.GetString("RoleName") != "Administrator") return Unauthorized();
            //id based Authentication
            //if (Guid.Parse(HttpContext.Session.GetString("UserId")) != id) return Unauthorized();


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
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
            return View(user);
        }

        // POST: Users/Edit/5
        
        [Moderator]
        [HttpPost]
        [Route("Users/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string oldPW, IFormFile file, [Bind("Id,LastName,FirstName,Gender,Username,Password,Birthday,Biography,isLocked,RoleID")] User user)
        //HTTP Post User Edit endpoint
        //Routed to /Users/Edit
        //Takes a id, a old password string for getting around the nessecity of changing the password every time, a file for a profile picture and a bindet edited user Object
        //Edits the User with the id using the given Properties
        {

            if (id != user.Id)
            {
                return NotFound();
            }
            
            ModelState.Remove("file");
            ModelState.Remove("Password");
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
                    if (user.Password == "" || user.Password == null)
                    {
                        

                        user.Password = oldPW;
                        
                    }
                    else
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    }
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
        [Moderator]
        [HttpGet]
        [Route("Users/Delete")]
        public async Task<IActionResult> Delete(Guid? id)
        //HTTP Get User Delete endpoint
        //Routed to /Users/Delete
        //Takes an id
        //serves the user delete view
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
        [Moderator]
        [HttpPost]
        [Route("Users/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        //HTTP Post User Delete endpoint
        //Routed to /Users/Delete
        //Takes a id
        //Deletes the User with the id
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
#endregion
        #region MVC PartialViews and Actions
        //-----------------------------------------------MVC User Details----------------------------------------------------------------------
        // Each of the following Method either opens a Modal for the operation in the Methods name or accepts the submitted Valus of one of the Popups and executes the Action

        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupUserDetails(Guid userID)
        // see region MVC PartialViews an Actions
        {
            if (userID == Guid.Empty || _dbContext.User == null)
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
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstOrDefaultAsync(m => m.Id == userID);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
           
            return PartialView("PopupUserDetails", _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy).ToList().Find(us => us.Id == user.Id));



        }

        //-----------------------------------------------MVC User ProfilePicture----------------------------------------------------------------------

        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupAddMedia(Guid userID)
        // see region MVC PartialViews an Actions
        {

            var user = await _dbContext.User.Include(u => u.ProfilePicture).FirstOrDefaultAsync(user => user.Id == userID);
            
            return PartialView("PopupAddMedia", user);



        }
        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveMedia(Guid userID, Guid mediaToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.ProfilePicture).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["mediatoremove"] = mediaToRemoveId;
            return PartialView("PopupRemoveMedia", user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> AddMediaToUser(Guid userID, IFormFile file)
        // see region MVC PartialViews an Actions
        {

            if (file == null || _dbContext.User == null)
            {
                return BadRequest();
            }


            

            User user = await _dbContext.User.Include(u => u.ProfilePicture).FirstAsync(p => p.Id == userID);
            
            if (user is null) return Unauthorized();


            if (user.ProfilePicture == null)
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
            }
            else
            {

                var media = await _dbContext.Media
                .Include(m => m.User)
                .Include(m => m.Post)
                .FirstOrDefaultAsync(m => m.Id == user.MediaId);
                if (media != null)
                {
                    if (media.User is not null)
                    {

                        //media.User.ProfilePicture = null;
                        //media.User.MediaId = null;
                        media.User.ProfilePicture = null;
                        media.User = null;
                        
                    }
                    
                    if (Path.Exists(media.FilePath))
                    {
                        System.IO.File.Delete(media.FilePath);
                    }
                    _dbContext.Media.Remove(media);
                    await _dbContext.SaveChangesAsync();
                }

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
            }
            
            



            


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
            return RedirectToAction(nameof(Edit), user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveMediaFromUser(Guid userID, Guid mediaToRemoveId)
        // see region MVC PartialViews an Actions
        {
            User user = await _dbContext.User.Include(u => u.ProfilePicture).FirstAsync(p => p.Id == userID);
            var media = await _dbContext.Media
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

                if (Path.Exists(media.FilePath))
                {
                    System.IO.File.Delete(media.FilePath);
                }
                _dbContext.Media.Remove(media);
                await _dbContext.SaveChangesAsync();
            }


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
            return RedirectToAction(nameof(Edit), user);
        }
        //-----------------------------------------------MVC User FollowedBy----------------------------------------------------------------------

        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupAddFollowedBy( Guid userID)
        // see region MVC PartialViews an Actions
        {

            var user = await _dbContext.User.Include(u => u.FollowedBy).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user!.FollowedBy;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList),"Id","");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddFollowedBy", _dbContext.User.Include(p=> p.FollowedBy).ToList().Find(us => us.Id == user.Id));
            

            
        }
        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveFollowedBy(Guid userID, Guid UserToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.FollowedBy).FirstOrDefaultAsync(user => user.Id == userID);
              
            
            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveFollowedBy", user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> AddFollowedByToUser(Guid userID, Guid userToAddId)
        // see region MVC PartialViews an Actions
        {

            if (userToAddId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToAdd = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToAddId);
            if (userToAdd == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.FollowedBy).FirstAsync(p => p.Id == userID);
            if (user.Id == userToAdd.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (user.FollowedBy.Contains(userToAdd)) return NotFound();
            user.FollowedBy.Add(userToAdd);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
            return RedirectToAction(nameof(Edit), user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveFollowedByFromUser(Guid userID, Guid userToRemoveId)
        // see region MVC PartialViews an Actions
        {

            if (userToRemoveId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToRemove = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToRemoveId);
            if (userToRemove == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.FollowedBy).FirstAsync(p => p.Id == userID);
            if (user.Id == userToRemove.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (!user.FollowedBy.Contains(userToRemove)) return NotFound();
            user.FollowedBy.Remove(userToRemove);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
            return RedirectToAction(nameof(Edit), user);
        }
        //-----------------------------------------------MVC User Following----------------------------------------------------------------------
        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupAddFollowing(Guid userID)
        // see region MVC PartialViews an Actions
        {

            var user = await _dbContext.User.Include(u => u.Following).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user!.Following;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList), "Id", "");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddFollowing", _dbContext.User.Include(p => p.Following).ToList().Find(us => us.Id == user.Id));



        }
        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveFollowing(Guid userID, Guid UserToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.Following).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveFollowing", user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> AddFollowingToUser(Guid userID, Guid userToAddId)
        // see region MVC PartialViews an Actions
        {

            if (userToAddId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToAdd = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToAddId);
            if (userToAdd == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.Following).FirstAsync(p => p.Id == userID);
            if (user.Id == userToAdd.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (user.Following.Contains(userToAdd)) return NotFound();
            user.Following.Add(userToAdd);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            ViewData["MediaId"] = new SelectList(_dbContext.Media, "Id", "Id", user.MediaId);
            ViewData["Following"] = user.Following;
            ViewData["FollowedBy"] = user.FollowedBy;
            ViewData["Posts"] = user.Posts;
            ViewData["Comments"] = user.Comments;
            ViewData["Votes"] = user.Votes;
            ViewData["BlockedBy"] = user.BlockedBy;
            ViewData["Blocking"] = user.Blocking;
            return RedirectToAction(nameof(Edit), user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveFollowingFromUser(Guid userID, Guid userToRemoveId)
        // see region MVC PartialViews an Actions
        {

            if (userToRemoveId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToRemove = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToRemoveId);
            if (userToRemove == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.Following).FirstAsync(p => p.Id == userID);
            if (user.Id == userToRemove.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (!user.Following.Contains(userToRemove)) return NotFound();
            user.Following.Remove(userToRemove);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            return RedirectToAction(nameof(Edit), user);
        }
        //-----------------------------------------------MVC User BlockedBy----------------------------------------------------------------------

        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupAddBlockedBy(Guid userID)
        // see region MVC PartialViews an Actions
        {

            var user = await _dbContext.User.Include(u => u.BlockedBy).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user!.BlockedBy;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList), "Id", "");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddBlockedBy", _dbContext.User.Include(p => p.BlockedBy).ToList().Find(us => us.Id == user.Id));



        }
        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveBlockedBy(Guid userID, Guid UserToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.BlockedBy).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveBlockedBy", user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> AddBlockedByToUser(Guid userID, Guid userToAddId)
        // see region MVC PartialViews an Actions
        {

            if (userToAddId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToAdd = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToAddId);
            if (userToAdd == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.BlockedBy).FirstAsync(p => p.Id == userID);
            if (user.Id == userToAdd.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (user.BlockedBy.Contains(userToAdd)) return NotFound();
            user.BlockedBy.Add(userToAdd);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            
            return RedirectToAction(nameof(Edit), user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveBlockedByFromUser(Guid userID, Guid userToRemoveId)
        // see region MVC PartialViews an Actions
        {

            if (userToRemoveId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToRemove = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToRemoveId);
            if (userToRemove == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.BlockedBy).FirstAsync(p => p.Id == userID);
            if (user.Id == userToRemove.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (!user.BlockedBy.Contains(userToRemove)) return NotFound();
            user.BlockedBy.Remove(userToRemove);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            
            return RedirectToAction(nameof(Edit), user);
        }
        //-----------------------------------------------MVC User Blocking----------------------------------------------------------------------
        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupAddBlocking(Guid userID)
        // see region MVC PartialViews an Actions
        {

            var user = await _dbContext.User.Include(u => u.Blocking).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user!.Blocking;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList), "Id", "");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddBlocking", _dbContext.User.Include(p => p.Blocking).ToList().Find(us => us.Id == user.Id));



        }
        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveBlocking(Guid userID, Guid UserToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.Blocking).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveBlocking", user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> AddBlockingToUser(Guid userID, Guid userToAddId)
        // see region MVC PartialViews an Actions
        {

            if (userToAddId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToAdd = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToAddId);
            if (userToAdd == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.Blocking).FirstAsync(p => p.Id == userID);
            if (user.Id == userToAdd.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (user.Blocking.Contains(userToAdd)) return NotFound();
            user.Blocking.Add(userToAdd);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            
            return RedirectToAction(nameof(Edit), user);

        }

        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveBlockingFromUser(Guid userID, Guid userToRemoveId)
        // see region MVC PartialViews an Actions
        {

            if (userToRemoveId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToRemove = await _dbContext.User
                .Include(u => u.Role)
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.Posts)
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.ProfilePicture)
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToRemoveId);
            if (userToRemove == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.Blocking).FirstAsync(p => p.Id == userID);
            if (user.Id == userToRemove.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (!user.Blocking.Contains(userToRemove)) return NotFound();
            user.Blocking.Remove(userToRemove);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            
            return RedirectToAction(nameof(Edit), user);
        }
        //-----------------------------------------------MVC User Post----------------------------------------------------------------------

        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemovePost(Guid userID, Guid postToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.Posts).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["postToRemove"] = postToRemoveId;
            return PartialView("PopupRemovePost", user);

        }

        

        [HttpPost]
        public async Task<ActionResult> RemovePostFromUser(Guid userID, Guid postToRemoveId)
        // see region MVC PartialViews an Actions
        {

            if (postToRemoveId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var postToRemove = await _dbContext.Post
                .Include(u => u.Comments)
                .Include(u => u.Votes)
                .Include(u => u.User)
                .Include(u => u.Media)                
                .FirstAsync(p => p.Id == postToRemoveId);
            if (postToRemove == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.Posts).FirstAsync(p => p.Id == userID);
            
            if (user is null) return Unauthorized();

            if (!user.Posts.Contains(postToRemove)) return NotFound();
            user.Posts.Remove(postToRemove);
            _dbContext.Update(user);
            _dbContext.Remove(postToRemove);
            await _dbContext.SaveChangesAsync();


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            
            return RedirectToAction(nameof(Edit), user);
        }
        //-----------------------------------------------MVC User Comment----------------------------------------------------------------------

        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveComment(Guid userID, Guid commentToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.Posts).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["commentToRemove"] = commentToRemoveId;
            return PartialView("PopupRemoveComment", user);

        }


        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveCommentFromUser(Guid userID, Guid commentToRemoveId)
        // see region MVC PartialViews an Actions
        {

            if (commentToRemoveId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var CommentToRemove = await _dbContext.Comment
                .Include(u => u.commentedBy)
                .Include(u => u.commentsComment)
                .Include(u => u.Post)
                .Include(u => u.User)
                .FirstAsync(p => p.Id == commentToRemoveId);
            if (CommentToRemove == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.Comments).FirstAsync(p => p.Id == userID);

            if (user is null) return Unauthorized();

            if (!user.Comments.Contains(CommentToRemove)) return NotFound();
            user.Comments.Remove(CommentToRemove);
            _dbContext.Update(user);
            RecursiveDelete(CommentToRemove);
            
            await _dbContext.SaveChangesAsync();


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            
            return RedirectToAction(nameof(Edit), user);
        }
        //-----------------------------------------------MVC User Vote----------------------------------------------------------------------

        [Moderator]
        [HttpPost]
        public async Task<IActionResult> PopupRemoveVote(Guid userID, Guid voteToRemoveId)
        // see region MVC PartialViews an Actions
        {
            var user = await _dbContext.User.Include(u => u.Posts).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["voteToRemove"] = voteToRemoveId;
            return PartialView("PopupRemoveVote", user);

        }


        [Moderator]
        [HttpPost]
        public async Task<ActionResult> RemoveVoteFromUser(Guid userID, Guid voteToRemoveId)
        // see region MVC PartialViews an Actions
        {

            if (voteToRemoveId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var voteToRemove = await _dbContext.Vote
                .Include(u => u.User)
                .Include(u => u.Post)                
                .FirstAsync(p => p.Id == voteToRemoveId);
            if (voteToRemove == null)
            {
                return NotFound();

            }

            User user = await _dbContext.User.Include(u => u.Votes).FirstAsync(p => p.Id == userID);

            if (user is null) return Unauthorized();

            if (!user.Votes.Contains(voteToRemove)) return NotFound();
            user.Votes.Remove(voteToRemove);
            _dbContext.Update(user);
            _dbContext.Remove(voteToRemove);
            await _dbContext.SaveChangesAsync();


            ViewData["RoleID"] = new SelectList(_dbContext.Role, "Id", "Name", user.RoleID);
            
            return RedirectToAction(nameof(Edit), user);
        }

        #endregion
        #region API
        //============================================= API =====================================================
        // GET: API/User?id..
        [HttpGet]
        [Route("API/User")]
        public async Task<ActionResult> Profile(Guid? id)
        //HTTP Get single user API endpoint
        //Routed to /API/User
        //Takes a id
        //if a user is found with the id it returns its values as JSON
        {
            if (id == null || _dbContext.User == null)
            {
                return BadRequest();
            }

            var user = await _dbContext.User
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.ProfilePicture)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
                
            }

            string userID = user.Id.ToString();
            string lastname = user.LastName;
            string firstname = user.FirstName;
            string username = user.Username;
            DateTime birthday = user.Birthday;
            string biography = user.Biography is null? "": user.Biography;
            string gender = user.Gender is null ? "" : user.Gender.ToString()!;
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
                { "birthday", birthday.ToString("dd.MM.yyyy") },
                { "biography", biography },
                { "gender", gender },
                { "followedCount", followedCount },
                { "followerCount", followerCount },
                { "pbFileName", pbFileName }
            };

           
            return Json(result);
        }
        [HttpPost]
        [Route("API/Users/Edit")]
        public async Task<IActionResult> EditUser([Bind("userID,LastName,FirstName,Username,Password,Birthday,Biography,Gender")]Guid userID, string LastName, string FirstName, string Username, string Password, string Birthday, string? Biography,int Gender) //Only works while logged in!
        //HTTP Post edit user API endpoint
        //Routed to /API/Users/Edit
        //Takes all editable properties of User Model as Model Binded parameters
        //returns HTTP Result Codes depending on succession
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            Guid _userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);

            if (userID == Guid.Empty || _dbContext.User == null) return BadRequest();                             
            if(LastName == "" || LastName == null) return BadRequest();
            if(FirstName == "" || FirstName == null) return BadRequest();
            if(Username == "" || Username == null) return BadRequest();
            if( Birthday == null) return BadRequest();
            if(Gender == null) return BadRequest();
            
                User user = _dbContext.User.Find(userID);
            if (user == null) return NotFound();

            user.LastName = LastName;
            user.FirstName = FirstName;
            user.Username = Username;
            if (Password != null && Password != "")
            {
                user.Password = Password;
                if (!TryValidateModel(user, nameof(user))) return ValidationProblem();

            }
            user.Birthday = DateTime.Parse(Birthday);
           
                user.Biography = Biography;
               
            
                user.Gender = (User.Genders?)Gender;
            
            if (Password != null && Password != "")
            {
                if (!TryValidateModel(user, nameof(user))) return ValidationProblem();
                user.Password = BCrypt.Net.BCrypt.HashPassword(Password);
            }
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            return Ok();




        }

        [HttpGet]
        [Route("API/Users/Search")]
        public async Task<ActionResult> SearchUser(string searchString)
        //HTTP Get search user API endpoint
        //Routed to /API/Users/Search
        //Takes a string with the typed in search request
        //searches for Users thats username contains the searchString and returns results as Json
        {
            if (_dbContext.User == null)
            {
                return BadRequest();
            }
            if (searchString is null) searchString = "";
            var users = _dbContext.User
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.ProfilePicture)
                .ToList()
                .FindAll(m => m.Username.Contains(searchString) /*|| m.FirstName.Contains(searchString) || m.LastName.Contains(searchString)*/);
            if (users == null)
            {
                return NotFound();

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (User user in users)
            {
                string userID = user.Id.ToString();
                string lastname = user.LastName;
                string firstname = user.FirstName;
                string username = user.Username;
                DateTime birthday = user.Birthday;
                string biography = user.Biography is null ? "" : user.Biography;
                string gender = user.Gender is null ? "" : user.Gender.ToString()!;
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

        // GET: API/Users
        [HttpGet]
        [Route("API/Users")]
        public async Task<ActionResult> UsersList()
        //HTTP Get User List API endpoint
        //Routed to /API/Users
        //returns every User Object currently saved
        {
            if (_dbContext.User == null)
            {
                return BadRequest();
            }

            var users = await _dbContext.User
                .Include(u => u.FollowedBy)
                .Include(u => u.Following)
                .Include(u => u.ProfilePicture)
                .ToListAsync();
            if (users == null || users.Count == 0)
            {
                return NotFound();

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (User user in users) {
                string userID = user.Id.ToString();
                string lastname = user.LastName;
                string firstname = user.FirstName;
                string username = user.Username;
                DateTime birthday = user.Birthday;
                string biography = user.Biography is null ? "" : user.Biography;
                string gender = user.Gender is null ? "" : user.Gender.ToString()!;
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
                    { "birthday", birthday.ToString("dd.MM.yyyy") },
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

        [HttpGet]
        [Route("API/Users/Following")]
        public async Task<ActionResult> GetFollowedUsers(Guid? UserID)
        //HTTP Get user Followed users API endpoint
        //Routed to /API/Users/Following
        //Takes a id
        //returns the followed Users of a User as JSON
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (_dbContext.User == null)
            {
                return BadRequest();
            }



            var userid = new Guid();
            if (UserID is not null)
            {
                userid = (Guid)UserID;
            }
            else
                userid = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User u = await _dbContext.User.Include(u => u.Following).FirstAsync(p => p.Id == userid);

            if (u is null) return Unauthorized();

            var users = u.Following;
            if (users == null)
            {
                return NotFound();

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (User user in users)
            {
                string userID = user.Id.ToString();
                string lastname = user.LastName;
                string firstname = user.FirstName;
                string username = user.Username;
                DateTime birthday = user.Birthday;
                string biography = user.Biography is null ? "" : user.Biography;
                string gender = user.Gender is null ? "" : user.Gender.ToString()!;
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

        [HttpGet]
        [Route("API/Users/FollowedBy")]
        public async Task<ActionResult> GetFollowedByUsers(Guid? UserID)
        //HTTP Get user Followed By users API endpoint
        //Routed to /API/Users/FollowedBy
        //Takes a id
        //returns the FollowedBy Users of a User as JSON
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (_dbContext.User == null)
            {
                return BadRequest();
            }

            var userid = new Guid();
            if (UserID is not null)
            {
                userid = (Guid)UserID;
            }else 
            userid = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User u = await _dbContext.User.Include(u => u.FollowedBy).FirstAsync(p => p.Id == userid);

            if (u is null) return Unauthorized();

            var users = u.FollowedBy;
            if (users == null)
            {
                return NotFound();

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (User user in users)
            {
                string userID = user.Id.ToString();
                string lastname = user.LastName;
                string firstname = user.FirstName;
                string username = user.Username;
                DateTime birthday = user.Birthday;
                string biography = user.Biography is null ? "" : user.Biography;
                string gender = user.Gender is null ? "" : user.Gender.ToString()!;
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

        [HttpPost]
        [Route("API/Users/Following/Add")]
        public async Task<ActionResult> AddFollowingToUser1(Guid userToFollowId)
        //HTTP Post user add Following user API endpoint
        //Routed to /API/Users/Following/Add
        //Takes a id of the user to be added 
        //Only works while logged in
        //returns HTTP Result codes based on succession
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToFollowId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToFollow = await _dbContext.User
                .Include(u => u.Following)
                .Include(u => u.FollowedBy)
                .FirstAsync(p => p.Id == userToFollowId);
            if (userToFollow == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User user = await _dbContext.User.Include(u=>u.Following).FirstAsync(p => p.Id == userID);
            if (user.Id == userToFollow.Id) return BadRequest();
            if (user is null) return Unauthorized();
            
            if(user.Following.Contains(userToFollow)) return NotFound();
            user.Following.Add(userToFollow);
            _dbContext.Update(user);            
            await _dbContext.SaveChangesAsync();


            return Ok();
        }

        [HttpPost]
        [Route("API/Users/Following/Remove")]
        public async Task<ActionResult> RemoveFollowingToUser1(Guid userToUnfollowId)
        //HTTP Post user remove Following user API endpoint
        //Routed to /API/Users/Following/Remove
        //Takes a id of the user to be removed
        //Only works while logged in
        //returns HTTP Result codes based on succession
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToUnfollowId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToUnfollow = await _dbContext.User
                .Include(u => u.Following)
                .Include(u => u.FollowedBy)
                .FirstAsync(p => p.Id == userToUnfollowId);
            if (userToUnfollow == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User user = await _dbContext.User.Include(u => u.Following).FirstAsync(p => p.Id == userID);
            if (user.Id == userToUnfollow.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (!user.Following.Contains(userToUnfollow)) return NotFound();
            user.Following.Remove(userToUnfollow);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            return Ok();
        }

       

        [HttpGet]
        [Route("API/Users/Blocking")]
        public async Task<ActionResult> GetBlockedUsers()
        //HTTP Get users blocked users API endpoint
        //Routed to /API/Users/Blocking
        //Only works while logged in
        //returns all of the currently logged in user blocked users
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (_dbContext.User == null)
            {
                return BadRequest();
            }


            
            Guid userid = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User u = await _dbContext.User.Include(u => u.Blocking).FirstAsync(p => p.Id == userid);
            
            if (u is null) return Unauthorized();

            var users = u.Blocking;
            if (users == null)
            {
                return NotFound();

            }
            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();

            foreach (User user in users)
            {
                string userID = user.Id.ToString();
                string lastname = user.LastName;
                string firstname = user.FirstName;
                string username = user.Username;
                DateTime birthday = user.Birthday;
                string biography = user.Biography is null ? "" : user.Biography;
                string gender = user.Gender is null ? "" : user.Gender.ToString()!;
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
        [HttpGet]
        [Route("API/Users/Posts")]
        public async Task<ActionResult> GetUsersPosts(Guid? id)
        //HTTP Get users Posts API endpoint
        //Routed to /API/Users/Posts
        //Takes a id
        //Only works while logged in
        //returns JSON of all the Users post using the id
        {


            if (_dbContext.User == null)
            {
                return BadRequest();
            }
            var userid = new Guid();
            if (id is not null)
            {
                userid = (Guid)id;
            }
            else userid = Guid.Parse(HttpContext.Session.GetString("UserId"));
            
            User u = await _dbContext.User
                .Include(u => u.Posts)
                .Include(u => u.FollowedBy)
                .FirstAsync(p => p.Id == userid);

            if (u is null) return Unauthorized();

            var posts = await _dbContext.Post
                .Include(u => u.User)
                .Include(u => u.Media)
                .Include(u => u.Votes)
                .ThenInclude(v => v.User)
                .Include(u => u.Comments)
                .Include(p => p.retweets)
                .ToListAsync();
            posts = posts.FindAll(p => p.UserId == userid && (p.IsPublic==true || u.FollowedBy.Find(u => u.Id == userid) != null || p.UserId==userid)).OrderByDescending(p=>p.CreatedDate).ToList();
            if (posts == null)
            {
                return NotFound();

            }

            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();
            foreach (Post post in posts)
            {
                string userID = post.UserId.ToString();
                string postID = post.Id.ToString();
                string user_username = post.User.Username;
                string user_profilePicture = (await _dbContext.Media.FindAsync(post.User.MediaId)) is null ? "" : (await _dbContext.Media.FindAsync(post.User.MediaId)).FileName;
                DateTime createdDate = post.CreatedDate;
                int rating = post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == true).Count - post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == false).Count;
                int commentCount = post.Comments.Count;
                string postText = post.TextContent;
                bool currentUserVoted = (post.Votes.ToList().Find(v => v.User.Id == userid) is not null && post.Votes.ToList().Find(v => v.User.Id == userid).User.Id == userid);
                string userVoteIsUpvote = currentUserVoted ? (post.Votes.ToList().Find(v => v.User.Id == userid).isUpVote ? "true" : "false") : "null";
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
                    { "userID", userID },
                    { "postID", postID },
                    { "user_username", user_username },
                    { "user_profilePicture", user_profilePicture },
                    { "createdDate", createdDate.ToString("dd.MM.yyyy") },
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

        [HttpPost]
        [Route("API/Users/Blocking/Add")]
        public async Task<ActionResult> AddBlockingToUser1(Guid userToBlockId)
        //HTTP Post users add blocking API endpoint
        //Routed to /API/Users/Blocking/Add
        //Takes a id of the user to be blocked
        //Only works while logged in
        //returns HTTP Result Codes based on succession
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToBlockId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToBlock = await _dbContext.User
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToBlockId);
            if (userToBlock == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User user = await _dbContext.User.Include(u => u.Blocking).FirstAsync(p => p.Id == userID);
            if (user.Id == userToBlock.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (user.Blocking.Contains(userToBlock)) return NotFound();
            user.Blocking.Add(userToBlock);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            return Ok();
        }

        [HttpPost]
        [Route("API/Users/Blocking/Remove")]
        public async Task<ActionResult> RemoveBlockingToUser1(Guid userToUnblockId)
        //HTTP Post users remove blocking API endpoint
        //Routed to /API/Users/Blocking/Remove
        //Takes a id of the user to be unblocked
        //Only works while logged in
        //returns HTTP Result Codes based on succession
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToUnblockId == Guid.Empty || _dbContext.User == null)
            {
                return BadRequest();
            }


            var userToUnblock = await _dbContext.User
                .Include(u => u.Blocking)
                .Include(u => u.BlockedBy)
                .FirstAsync(p => p.Id == userToUnblockId);
            if (userToUnblock == null)
            {
                return NotFound();

            }
            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId"));
            User user = await _dbContext.User.Include(u => u.Blocking).FirstAsync(p => p.Id == userID);
            if (user.Id == userToUnblock.Id) return BadRequest();
            if (user is null) return Unauthorized();

            if (!user.Blocking.Contains(userToUnblock)) return NotFound();
            user.Blocking.Remove(userToUnblock);
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();


            return Ok();
        }


        [HttpDelete]
        [Route("API/Users/Remove")]
        public async Task<IActionResult> DeletePost(Guid id)
        //HTTP Delete users API endpoint
        //Routed to /API/Users/Remove
        //Takes a id of the user to be removed
        //Only works while logged in
        //returns HTTP Result Codes based on succession
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            if (_dbContext.User == null)
            {
                return BadRequest();
            }

            Guid userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);


            var user = await _dbContext.User
                .Include(user => user.ProfilePicture)
                .Include(user => user.Role)
                .Include(user => user.FollowedBy)
                .Include(user => user.Following)
                .Include(user => user.BlockedBy)
                .Include(user => user.Blocking)
                .Include(user => user.Posts)
                .Include(user => user.Comments)
                .Include(user => user.Votes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (user is null) return NotFound();
            if (user.Id != id || user.Id != userID) return Unauthorized();

            //-------------Delete direct Comments
            foreach (Comment com in user.Comments)
            {
                var comment = _dbContext.Comment
                .Include(c => c.Post)
                .Include(c => c.User)
                .Include(c => c.commentsComment)
                .Include(c => c.commentedBy)
                .FirstOrDefault(c => c.Id == com.Id);
                
                if (comment != null)
                {
                    
                    RecursiveDelete(comment);

                }


            }
            //-------------Delete Posts and their Comments and Media
            foreach (Post pos in user.Posts)
            {
                var post = await _dbContext.Post
                .Include(post => post.User)
                .Include(post => post.Comments)
                .Include(post => post.Media)
                .Include(post => post.Votes)
                .FirstOrDefaultAsync(p => p.Id == pos.Id);

                foreach (Comment com in pos.Comments)
                {
                    var comment = _dbContext.Comment
                    .Include(c => c.Post)
                    .Include(c => c.User)
                    .Include(c => c.commentsComment)
                    .Include(c => c.commentedBy)
                    .FirstOrDefault(c => c.Id == com.Id);

                    if (comment != null)
                    {
                        
                        RecursiveDelete(comment);

                    }


                }
                foreach (Media m in pos.Media)
                {
                    m.Post = null;
                    if (Path.Exists(m.FilePath))
                    {
                        System.IO.File.Delete(m.FilePath);
                    }
                    _dbContext.Media.Remove(m);
                }
                pos.Media.Clear();
                _dbContext.Post.Remove(pos);

            }
            _dbContext.Remove(user);
            await _dbContext.SaveChangesAsync();
            HttpContext.Session.Clear();
            return Ok();
        }

        //-----------------------------------------------API User ProfilePicture----------------------------------------------------------------------

      

        [HttpPost]
        [Route("API/Users/Media/Add")]
        public async Task<ActionResult> AddMediaToUser1(Guid userID, IFormFile file)
        //HTTP Add media to user API endpoint
        //Routed to /API/Users/Media/Add
        //Takes a id of the user to be added to and a file to be added
        //Only works while logged in
        //returns HTTP Result Codes based on succession
        {

            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            Guid _userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            if (userID != _userID) return Unauthorized();
            if (file == null || _dbContext.User == null)
            {
                return BadRequest();
            }




            User user = await _dbContext.User.Include(u => u.ProfilePicture).FirstAsync(p => p.Id == userID);

            if (user is null) return Unauthorized();


            if (user.ProfilePicture == null)
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
            }
            else
            {

                var media = await _dbContext.Media
                .Include(m => m.User)
                .Include(m => m.Post)
                .FirstOrDefaultAsync(m => m.Id == user.MediaId);
                if (media != null)
                {
                    if (media.User is not null)
                    {

                        //media.User.ProfilePicture = null;
                        //media.User.MediaId = null;
                        media.User.ProfilePicture = null;
                        media.User = null;

                    }

                    if (Path.Exists(media.FilePath))
                    {
                        System.IO.File.Delete(media.FilePath);
                    }
                    _dbContext.Media.Remove(media);
                    await _dbContext.SaveChangesAsync();
                }

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
            }



            return Ok();

        }

        [HttpPost]
        [Route("API/Users/Media/Remove")]
        public async Task<ActionResult> RemoveMediaFromUser1(Guid userID, Guid mediaToRemoveId)
        //HTTP Remove media from user API endpoint
        //Routed to /API/Users/Media/Remove
        //Takes a id of the user to be removed from to and a file id
        //Only works while logged in
        //returns HTTP Result Codes based on succession
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")!))) is null) return Unauthorized();
            Guid _userID = Guid.Parse(HttpContext.Session.GetString("UserId")!);
            if (userID != _userID) return Unauthorized();
            User user = await _dbContext.User.Include(u => u.ProfilePicture).FirstAsync(p => p.Id == userID);
            var media = await _dbContext.Media
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

                if (Path.Exists(media.FilePath))
                {
                    System.IO.File.Delete(media.FilePath);
                }
                _dbContext.Media.Remove(media);
                await _dbContext.SaveChangesAsync();
            }
            else return NotFound();


           
            return Ok();
        }
        #endregion
        #region Helper Methods
        //============================================= Helper Methods =====================================================
        private void RecursiveDelete(Comment parent)
            //not an Endpoint
            //takes a Comment object
            //Deletes the comment and all of its child Comments recursively
        {
            if (parent.commentedBy != null && parent.commentedBy.Count > 0)
            {
                var children = _dbContext.Comment
                    .Include(x => x.commentedBy)
                    .Where(x => x.commentsCommentId == parent.Id).ToList();

                foreach (var child in children)
                {
                    RecursiveDelete(child);
                }
            }

            _dbContext.Remove(parent);
        }
        #endregion

    }


}
