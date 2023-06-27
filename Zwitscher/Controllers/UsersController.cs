using System;
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

//--------------------------------------------------------------------------------------------------------------------
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
        public async Task<ActionResult> AddFollowingToUser(Guid userToFollowId) //Only works while logged in!
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
        public async Task<ActionResult> RemoveFollowingToUser(Guid userToUnfollowId) //Only works while logged in!
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

        [HttpPost]
        [Route("API/Users/Blocking/Add")]
        public async Task<ActionResult> AddBlockingToUser(Guid userToBlockId) //Only works while logged in!
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
        public async Task<ActionResult> RemoveBlockingToUser(Guid userToUnblockId) //Only works while logged in!
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
