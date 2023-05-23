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
            if (ModelState.IsValid)
            {
                var userInDb = _context.User.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
                if (userInDb != null)
                {
                    //ISession session = HttpContext.Session;
                    //session.SetString("UserId", userInDb.Id.ToString());
                    //session.SetString("Username", userInDb.Username);
                    _context.User.Update(userInDb);
                    return RedirectToAction(nameof(Details));
                }
                else
                {
                    ModelState.AddModelError("LoginFailed", "Login failed. Please try again.");
                }
            }
            //ViewData["RoleID"] = new SelectList(_context.Role, "Id", "Id", user.RoleID);
            return RedirectToAction(nameof(Register));
        }

        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Index));
        }


        // GET: Auth/Create
        public IActionResult Register()
        {
            //ViewData["RoleID"] = new SelectList(_context.Role, "Id", "Id");
            return View();
        }

        // POST: Auth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("LastName,FirstName,Username,Password,Birthday")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}
