using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class AuthController : Controller // Controller for login and register
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ZwitscherContext _context;

        public AuthController(ZwitscherContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Auth
        public async Task<IActionResult> Index()
        {
            var zwitscherContext = _context.User.Include(u => u.Role);
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        // This method is responsible for the login. To do this, it compares the data from the input field with the database entries.
        // If a user can be found, his or her data is stored in the HttpContext.Session and can then be called up throughout the application.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username,Password")] String Username, String Password)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.User.FirstOrDefault(u => u.Username == Username);
                if (userInDb != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(Password, userInDb.Password) && userInDb.isLocked == false)
                    {
                        HttpContext.Session.SetString("UserId", userInDb.Id.ToString());
                        HttpContext.Session.SetString("Username", userInDb.Username);
                        HttpContext.Session.SetString("RoleID", userInDb.RoleID.ToString());
                        HttpContext.Session.SetString("FirstName", userInDb.FirstName);
                        HttpContext.Session.SetString("LastName", userInDb.LastName);
                        HttpContext.Session.SetString("Birthday", userInDb.Birthday.ToString());
                        _logger.LogInformation($"User {userInDb.Username} logged in.");
                        return RedirectToAction(nameof(Details));
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
        //public async Task<IActionResult> Register([Bind("LastName,FirstName,Username,Password,Birthday")] User user)
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
                    user.RoleID = Guid.Parse("735a4145-1525-42fa-f8c5-08db522222ed");
                    user.isLocked = false;
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
    }
}
