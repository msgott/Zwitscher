using System;
using System.Collections.Generic;
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
        [HttpGet]
        [Route("Users/Edit")]
        //[Admin] //for Development deactivated
        public async Task<IActionResult> Edit(Guid? id)
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
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
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
        //-----------------------------------------------MVC User FollowedBy----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupAddFollowedBy( Guid userID)
        {
           
            var user = await _dbContext.User.Include(u => u.FollowedBy).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user.FollowedBy;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList),"Id","");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddFollowedBy", _dbContext.User.Include(p=> p.FollowedBy).ToList().Find(us => us.Id == user.Id));
            

            
        }
        [HttpPost]
        public async Task<IActionResult> PopupRemoveFollowedBy(Guid userID, Guid UserToRemoveId)
        {
            var user = await _dbContext.User.Include(u => u.FollowedBy).FirstOrDefaultAsync(user => user.Id == userID);
              
            
            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveFollowedBy", user);

        }

        [HttpPost]
        public async Task<ActionResult> AddFollowedByToUser(Guid userID, Guid userToAddId) //Just for the MVC Frontend 
        {

            if (userToAddId == null || _dbContext.User == null)
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

        [HttpPost]
        public async Task<ActionResult> RemoveFollowedByFromUser(Guid userID, Guid userToRemoveId) //Just for the MVC Frontend 
        {

            if (userToRemoveId == null || _dbContext.User == null)
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
        [HttpPost]
        public async Task<IActionResult> PopupAddFollowing(Guid userID)
        {
            
            var user = await _dbContext.User.Include(u => u.Following).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user.Following;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList), "Id", "");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddFollowing", _dbContext.User.Include(p => p.Following).ToList().Find(us => us.Id == user.Id));



        }
        [HttpPost]
        public async Task<IActionResult> PopupRemoveFollowing(Guid userID, Guid UserToRemoveId)
        {
            var user = await _dbContext.User.Include(u => u.Following).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveFollowing", user);

        }

        [HttpPost]
        public async Task<ActionResult> AddFollowingToUser(Guid userID, Guid userToAddId) //Just for the MVC Frontend 
        {

            if (userToAddId == null || _dbContext.User == null)
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

        [HttpPost]
        public async Task<ActionResult> RemoveFollowingFromUser(Guid userID, Guid userToRemoveId) //Just for the MVC Frontend 
        {

            if (userToRemoveId == null || _dbContext.User == null)
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
        //-----------------------------------------------MVC User BlockedBy----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupAddBlockedBy(Guid userID)
        {

            var user = await _dbContext.User.Include(u => u.BlockedBy).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user.BlockedBy;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList), "Id", "");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddBlockedBy", _dbContext.User.Include(p => p.BlockedBy).ToList().Find(us => us.Id == user.Id));



        }
        [HttpPost]
        public async Task<IActionResult> PopupRemoveBlockedBy(Guid userID, Guid UserToRemoveId)
        {
            var user = await _dbContext.User.Include(u => u.BlockedBy).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveBlockedBy", user);

        }

        [HttpPost]
        public async Task<ActionResult> AddBlockedByToUser(Guid userID, Guid userToAddId) //Just for the MVC Frontend 
        {

            if (userToAddId == null || _dbContext.User == null)
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

        [HttpPost]
        public async Task<ActionResult> RemoveBlockedByFromUser(Guid userID, Guid userToRemoveId) //Just for the MVC Frontend 
        {

            if (userToRemoveId == null || _dbContext.User == null)
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
        //-----------------------------------------------MVC User Blocking----------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> PopupAddBlocking(Guid userID)
        {

            var user = await _dbContext.User.Include(u => u.Blocking).FirstOrDefaultAsync(user => user.Id == userID);
            //can send some data also.  
            List<User> tempList = user.Blocking;
            tempList.Add(user);
            SelectList tempSelectList = new SelectList(_dbContext.User.ToList().Except(tempList), "Id", "");
            ViewData["users"] = tempSelectList;
            return PartialView("PopupAddBlocking", _dbContext.User.Include(p => p.Blocking).ToList().Find(us => us.Id == user.Id));



        }
        [HttpPost]
        public async Task<IActionResult> PopupRemoveBlocking(Guid userID, Guid UserToRemoveId)
        {
            var user = await _dbContext.User.Include(u => u.Blocking).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["usertoremove"] = UserToRemoveId;
            return PartialView("PopupRemoveBlocking", user);

        }

        [HttpPost]
        public async Task<ActionResult> AddBlockingToUser(Guid userID, Guid userToAddId) //Just for the MVC Frontend 
        {

            if (userToAddId == null || _dbContext.User == null)
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

        [HttpPost]
        public async Task<ActionResult> RemoveBlockingFromUser(Guid userID, Guid userToRemoveId) //Just for the MVC Frontend 
        {

            if (userToRemoveId == null || _dbContext.User == null)
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
        //-----------------------------------------------MVC User Post----------------------------------------------------------------------
        
        [HttpPost]
        public async Task<IActionResult> PopupRemovePost(Guid userID, Guid postToRemoveId)
        {
            var user = await _dbContext.User.Include(u => u.Posts).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["postToRemove"] = postToRemoveId;
            return PartialView("PopupRemovePost", user);

        }

        

        [HttpPost]
        public async Task<ActionResult> RemovePostFromUser(Guid userID, Guid postToRemoveId) //Just for the MVC Frontend 
        {

            if (postToRemoveId == null || _dbContext.User == null)
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
        //-----------------------------------------------MVC User Comment----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupRemoveComment(Guid userID, Guid commentToRemoveId)
        {
            var user = await _dbContext.User.Include(u => u.Posts).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["commentToRemove"] = commentToRemoveId;
            return PartialView("PopupRemoveComment", user);

        }



        [HttpPost]
        public async Task<ActionResult> RemoveCommentFromUser(Guid userID, Guid commentToRemoveId) //Just for the MVC Frontend 
        {

            if (commentToRemoveId == null || _dbContext.User == null)
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
            _dbContext.Remove(CommentToRemove);
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
        //-----------------------------------------------MVC User Vote----------------------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> PopupRemoveVote(Guid userID, Guid voteToRemoveId)
        {
            var user = await _dbContext.User.Include(u => u.Posts).FirstOrDefaultAsync(user => user.Id == userID);


            ViewData["voteToRemove"] = voteToRemoveId;
            return PartialView("PopupRemoveVote", user);

        }



        [HttpPost]
        public async Task<ActionResult> RemoveVoteFromUser(Guid userID, Guid voteToRemoveId) //Just for the MVC Frontend 
        {

            if (voteToRemoveId == null || _dbContext.User == null)
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


        //-----------------------------------------------API---------------------------------------------------------------------
        // GET: API/User?id..
        [HttpGet]
        [Route("API/User")]
        public async Task<ActionResult> Profile(Guid? id)
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

        [HttpGet]
        [Route("API/Users/Search")]
        public async Task<ActionResult> SearchUser(string searchString)
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
                .FindAll(m => m.Username.Contains(searchString) || m.FirstName.Contains(searchString) || m.LastName.Contains(searchString));
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

        // GET: API/Users
        [HttpGet]
        [Route("API/Users")]
        public async Task<ActionResult> UsersList()
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

        [HttpGet]
        [Route("API/Users/Following")]
        public async Task<ActionResult> GetFollowedUsers(Guid? UserID) //Only works while logged in!
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

        [HttpGet]
        [Route("API/Users/FollowedBy")]
        public async Task<ActionResult> GetFollowedByUsers(Guid? UserID) //Only works while logged in!
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

        [HttpPost]
        [Route("API/Users/Following/Add")]
        public async Task<ActionResult> AddFollowingToUser1(Guid userToFollowId) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToFollowId == null || _dbContext.User == null)
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
        public async Task<ActionResult> RemoveFollowingToUser1(Guid userToUnfollowId) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToUnfollowId == null || _dbContext.User == null)
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
        public async Task<ActionResult> GetBlockedUsers() //Only works while logged in!
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
        [HttpGet]
        [Route("API/Users/Posts")]
        public async Task<ActionResult> GetUsersPosts(Guid? id) 
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
            posts = posts.FindAll(p => p.UserId == userid);
            if (posts == null || posts.Count == 0)
            {
                return NotFound();

            }

            List<Dictionary<string, Object>> results = new List<Dictionary<string, Object>>();
            foreach (Post post in posts)
            {
                string postID = post.Id.ToString();
                string user_username = post.User.Username;
                string user_profilePicture = (await _dbContext.Media.FindAsync(post.User.MediaId)) is null ? "" : (await _dbContext.Media.FindAsync(post.User.MediaId)).FileName;
                DateTime createdDate = post.CreatedDate;
                int rating = post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == true).Count - post.Votes.ToList<Vote>().FindAll(v => v.isUpVote == false).Count;
                int commentCount = post.Comments.Count;
                string postText = post.TextContent;
                bool currentUserVoted = (post.Votes.ToList().Find(v => v.User.Id == userid) is not null && post.Votes.ToList().Find(v => v.User.Id == userid).User.Id == userid);
                string userVoteIsUpvote = currentUserVoted ? (post.Votes.ToList().Find(v => v.User.Id == userid).isUpVote ? "true" : "false") : "null";
                string retweetsPost = post.retweetsID.ToString();
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

        [HttpPost]
        [Route("API/Users/Blocking/Add")]
        public async Task<ActionResult> AddBlockingToUser1(Guid userToBlockId) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToBlockId == null || _dbContext.User == null)
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
        public async Task<ActionResult> RemoveBlockingToUser1(Guid userToUnblockId) //Only works while logged in!
        {
            if (HttpContext.Session.GetString("UserId") is null) return Unauthorized();
            if ((await _dbContext.User.FindAsync(Guid.Parse(HttpContext.Session.GetString("UserId")))) is null) return Unauthorized();
            if (userToUnblockId == null || _dbContext.User == null)
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


    }


}
