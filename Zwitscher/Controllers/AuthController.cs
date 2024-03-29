﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Data;
using Zwitscher.Models;
using System.Text.Json;
using System.Data;

namespace Zwitscher.Controllers
{
    public class AuthController : Controller // Controller for login and register
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ZwitscherContext _context;

        //Constructor
        public AuthController(ZwitscherContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region MVC Frontend
        //============================================= MVC Frontend =====================================================

        // GET: Auth
        // For MVC Frontend
        // Returns the Login View
        public async Task<IActionResult> Index()
        {
            var zwitscherContext = _context.User.Include(u => u.Role);
            return View();
        }


        // GET: Auth/Login
        // For MVC Frontend
        // Returns the Login View
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        // For MVC Frontend
        // This method is responsible for the login. To do this, it compares the data from the input field with the database entries.
        // If a user can be found, his or her data is stored in the HttpContext.Session and can then be called up throughout the application.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username,Password")] String Username, String Password)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.User.Include(u => u.ProfilePicture).FirstOrDefault(u => u.Username == Username);
                if (userInDb != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(Password, userInDb.Password) && userInDb.isLocked == false)
                    {
                        var role = _context.Role.FirstOrDefault(r => r.Id == userInDb.RoleID);

                        HttpContext.Session.SetString("UserId", userInDb.Id.ToString());
                        HttpContext.Session.SetString("Username", userInDb.Username);
                        HttpContext.Session.SetString("RoleID", userInDb.RoleID.ToString());
                        HttpContext.Session.SetString("RoleName", role.Name);
                        HttpContext.Session.SetString("FirstName", userInDb.FirstName);
                        HttpContext.Session.SetString("LastName", userInDb.LastName);
                        HttpContext.Session.SetString("Birthday", userInDb.Birthday.ToString());
                        if (userInDb.ProfilePicture is not null)
                        {
                            HttpContext.Session.SetString("pbFileName", userInDb.ProfilePicture.FileName);
                        }
                        else
                        {
                            HttpContext.Session.SetString("pbFileName", "real-placeholder.png");
                        }
                        _logger.LogInformation($"User {userInDb.Username} logged in.");
                        return Redirect("/Zwitscher");
                    }
                    else
                    {
                        _logger.LogInformation($"User {userInDb.Username} tried to log in with wrong password or is locked.");
                        ModelState.AddModelError(nameof(Models.User.Password), "Wrong Password!");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(Models.User.Password), "Login failed. Please try again.");
                }
            }
            ViewData["Error"] = "Login fehlgeschlagen";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        // GET: Auth/Create
        public IActionResult Register()
        {
            return View();
        }

        //This method is used for registration. When a user goes to this page, he or she can create a new account,
        //which automatically gets the role User. An error is thrown for an existing username.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("LastName,FirstName,Username,Password,Birthday")] String LastName, String FirstName, int Gender, String Username, String Password, DateTime Birthday)
        {
            User user = new User();
            if (ModelState.IsValid)
            {
                user.LastName = LastName;
                user.FirstName = FirstName;
                user.Gender = (Models.User.Genders)Gender;
                user.Username = Username;
                user.Password = Password;
                user.Birthday = Birthday;
                var check = _context.User.FirstOrDefault(u => u.Username == user.Username);
                if (check == null)
                {
                    user.Id = Guid.NewGuid();
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    var role = _context.Role.FirstOrDefault(u => u.Name == "User");
                    user.RoleID = role.Id;
                    user.isLocked = false;
                    user.CreatedDate = DateTime.Now;
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                { 
                    ModelState.AddModelError("Username", "Username already exists!");
                }
            }
            return View(user);
        }

        private bool UserExists(Guid id)
        {
          return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {

            if (HttpContext.Session.GetString("UserId") != null)
            {
                var session_user = await _context.User.FirstOrDefaultAsync(m => m.Id == Guid.Parse(HttpContext.Session.GetString("UserId")));
                return View(session_user);
            }

            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
                return View(user);
            }
        }
        #endregion

        #region API Endpoint stuff
        //============================================= API Endpoint stuff =====================================================

        [HttpPost]
        [Route("Api/Login")]
        public string LoginUser(String Username, String Password)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.User.Include(u => u.ProfilePicture).FirstOrDefault(u => u.Username == Username);
                if (userInDb != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(Password, userInDb.Password) && userInDb.isLocked == false)
                    {
                        var role = _context.Role.FirstOrDefault(r => r.Id == userInDb.RoleID);

                        HttpContext.Session.SetString("UserId", userInDb.Id.ToString());
                        HttpContext.Session.SetString("Username", userInDb.Username);
                        HttpContext.Session.SetString("RoleID", userInDb.RoleID.ToString());
                        HttpContext.Session.SetString("RoleName", role.Name);
                        HttpContext.Session.SetString("FirstName", userInDb.FirstName);
                        HttpContext.Session.SetString("LastName", userInDb.LastName);
                        HttpContext.Session.SetString("Birthday", userInDb.Birthday.ToString());
                        if (userInDb.ProfilePicture is not null)
                        {
                            HttpContext.Session.SetString("pbFileName", userInDb.ProfilePicture.FileName);
                        }
                        else
                        {
                            HttpContext.Session.SetString("pbFileName", "real-placeholder.png");
                        }
                        _logger.LogInformation($"User {userInDb.Username} logged in.");

                        var result = new { Username = userInDb.Username, ProfilePicture = HttpContext.Session.GetString("pbFileName"), RoleName = role.Name, Success = true };

                        return JsonSerializer.Serialize(result);
                    }
                    else
                    {
                        _logger.LogInformation($"User {userInDb.Username} tried to log in with wrong password or is locked.");
                        ModelState.AddModelError("LoginFailed", "Wrong Password!");
                    }
                }
                else
                {
                    ModelState.AddModelError("LoginFailed", "Login failed. Please try again.");
                }
            }
            return JsonSerializer.Serialize(new { Username = "", ProfilePicture = "", RoleName = "", Success = false });
        }

        [HttpPost]
        [HttpGet]
        [Route("Api/Logout")]
        public void LogoutUser()
        {
            HttpContext.Session.Clear();
            Redirect("/Zwitscher");
        }

        [HttpPost]
        [Route("Api/Register")]
        public async Task<string> RegisterUser(string LastName, String FirstName, int Gender, String Username, String Password, DateTime Birthday)
        {
            User user = new User();
            if (ModelState.IsValid)
            {
                user.LastName = LastName;
                user.FirstName = FirstName;
                user.Gender = (Models.User.Genders)Gender;
                user.Username = Username;
                user.Password = Password;
                user.Birthday = Birthday;
                user.CreatedDate = DateTime.Now;
                var check = _context.User.FirstOrDefault(u => u.Username == user.Username);
                if (check == null)
                {
                    user.Id = Guid.NewGuid();
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    var role = _context.Role.FirstOrDefault(u => u.Name == "User");
                    user.RoleID = role.Id;
                    user.isLocked = false;
                    user.CreatedDate = DateTime.Now;
                    _context.Add(user);
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("RoleID", user.RoleID.ToString());
                    HttpContext.Session.SetString("RoleName", role.Name);
                    HttpContext.Session.SetString("FirstName", user.FirstName);
                    HttpContext.Session.SetString("LastName", user.LastName);
                    HttpContext.Session.SetString("Birthday", user.Birthday.ToString());
                    if (user.ProfilePicture is not null)
                    {
                        HttpContext.Session.SetString("pbFileName", user.ProfilePicture.FileName);
                    }
                    else
                    {
                        HttpContext.Session.SetString("pbFileName", "real-placeholder.png");
                    }

                    var result = new { Username = Username, ProfilePicture = HttpContext.Session.GetString("pbFileName"), RoleName = role.Name, Success = true };
                    return JsonSerializer.Serialize(result);
                }
                else
                {
                    ModelState.AddModelError("Username", "Username already exists!");
                }
            }
            return JsonSerializer.Serialize(new { Username = "", RoleName = "", Success = false });
        }

        // Just for testing purposes
        [HttpGet]
        [Route("Api/UserDetails")]
        public async Task<string> DetailsApi()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var session_user = await _context.User.FirstOrDefaultAsync(m => m.Id == Guid.Parse(HttpContext.Session.GetString("UserId")));
                var profilePicture = HttpContext.Session.GetString("pbFileName");

                return JsonSerializer.Serialize(new { Username = HttpContext.Session.GetString("Username"), ProfilePicture = profilePicture, RoleName = HttpContext.Session.GetString("RoleName"), Success = true, userID=session_user.Id });
            }

            return JsonSerializer.Serialize(new { Username = "", ProfilePicture = "", RoleName = "", Success = false });
        }
        #endregion
    }
}
