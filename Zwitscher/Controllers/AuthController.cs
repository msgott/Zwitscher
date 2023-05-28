using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class AuthController : Controller // Controller for login and register
    {
        private readonly ZwitscherContext _context;

        public AuthController(ZwitscherContext context)
        {
            _context = context;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username,Password")] User user)
        {
            Console.WriteLine("Entering Login"+ user.ToString());
            if (ModelState.IsValid)
            {
                Console.WriteLine("Logging in user: " + user.Username);
                var userInDb = _context.User.FirstOrDefault(u => u.Username == user.Username);
                if (userInDb != null)
                {
                    Console.WriteLine("User found: " + userInDb.Username);
                    if (BCrypt.Net.BCrypt.Verify(user.Password, userInDb.Password))
                    //if (user.Password == userInDb.Password)
                    {
                        HttpContext.Session.SetString("UserId", userInDb.Id.ToString());
                        HttpContext.Session.SetString("Username", userInDb.Username);
                        HttpContext.Session.SetString("RoleID", userInDb.RoleID.ToString());
                        HttpContext.Session.SetString("FirstName", userInDb.FirstName);
                        HttpContext.Session.SetString("LastName", userInDb.LastName);
                        HttpContext.Session.SetString("Birthday", userInDb.Birthday.ToString());
                    }
                    else
                    {
                        ModelState.AddModelError("LoginFailed", "Wrong Password!");
                    }
                    return RedirectToAction(nameof(Details));
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

        // POST: Auth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("LastName,FirstName,Username,Password,Birthday")] User user)
        {
            Console.WriteLine("Entering Registration" + user);
            if (ModelState.IsValid)
            {
                Console.WriteLine("Registering new user: " + user.Username);
                var check = _context.User.FirstOrDefault(u => u.Username == user.Username);
                if (check == null)
                {
                    user.Id = Guid.NewGuid();
                    Console.WriteLine("User Id " + user.Id);
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    user.RoleID = Guid.Parse("735a4145-1525-42fa-f8c5-08db522222ed");
                    user.isLocked = false;
                    Console.WriteLine(user.Password);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                { 
                    Console.WriteLine("User already exists: " + user.Username);
                    //ModelState.AddModelError("Username", "Username already exists!");
                }
            }
            return View(user);
        }

        private bool UserExists(Guid id)
        {
          return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Route("Users/Details")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || HttpContext.Session.Id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
