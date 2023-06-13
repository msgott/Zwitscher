using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Zwitscher.Data;
using Zwitscher.Models;

namespace Zwitscher.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ZwitscherContext _context;

        public HomeController(ILogger<HomeController> logger, ZwitscherContext zwitscherContext)
        {
            _logger = logger;
            _context = zwitscherContext;
        }

        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                ViewBag.Message = HttpContext.Session.GetString("Username");
                var user = _context.User.Include(p => p.Following).FirstOrDefault(u => u.Id == Guid.Parse(HttpContext.Session.GetString("UserId")));
                if (user != null)
                {
                    var followedPosts = _context.Post.Include(p => p.User).Where(p => user.Following.Contains(p.User)).ToList();
                    return View(followedPosts);
                }
            }

            var posts = _context.Post.Include(p => p.User).ToList();


            return View(posts);
        }

        public IActionResult Public()
        {
            var posts = _context.Post.Include(p => p.User).ToList();
            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}