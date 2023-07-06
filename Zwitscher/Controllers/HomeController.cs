using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Zwitscher.Attributes;
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
                var user = _context.User.Include(p => p.Following).FirstOrDefault(u => u.Id == Guid.Parse(HttpContext.Session.GetString("UserId")!));
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

        //[Moderator]
        [Route("")]
        public IActionResult Dashboard()
        {
            // Total User
            int totalUsers = _context.User.Count();
            ViewBag.TotalUsers = totalUsers;

            // NUMBER OF NEW USERS - 14 DAYS
            DateTime fourteenDaysAgo = DateTime.Now.AddDays(-14);

            int totalNewUsersFourteen = _context.User.Count(p => p.CreatedDate >= fourteenDaysAgo);
            ViewBag.totalNewUsersFourteen = totalNewUsersFourteen;

            // MODERATORS
            int totalModerators = _context.User.Include(u => u.Role).Count(u => u.Role.Name == "Moderator"); // Count the number of users with the "Moderator" role
            ViewBag.totalModerators = totalModerators;

            // ADMINS
            int totalAdmins = _context.User.Include(u=> u.Role).Count(u => u.Role.Name== "Administrator");
            ViewBag.totalAdmins = totalAdmins;

            // Total Posts (Zwitschers) 
            int totalPosts = _context.Post.Count();
            ViewBag.totalPosts = totalPosts;


            // NEW ZWITSCHERS - 14 DAYS

            int newZwitschersFourteen = _context.Post.Count(p => p.CreatedDate >= fourteenDaysAgo);
            ViewBag.newZwitschersFourteen = newZwitschersFourteen;

            // AVG. ZWITSCHER RATE - 30 DAYS
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-900); // change to 30 whenever there are some posts available

            int totalPostsThirty = _context.Post.Count(p => p.CreatedDate >= thirtyDaysAgo);
            int numberOfDays = (DateTime.Now - thirtyDaysAgo).Days; 
            double averagePosts = (double)totalPostsThirty / numberOfDays;
            averagePosts = Math.Round(averagePosts, 3); // max. three decimal places

            ViewBag.AveragePosts = averagePosts;

            // BANNED USERS
            int totalBanned = _context.User.Count(u => u.isLocked == true); 
            ViewBag.totalBanned = totalBanned;

            //MOST FAMOUS USER
            List<User> userOrderedByFollowedBy = _context.User.OrderBy(u => u.FollowedBy.Count).ToList<User>();
            string mostFamousUser = userOrderedByFollowedBy.Last().Username;
            ViewBag.mostFamousUser = mostFamousUser;

            //GENDER STATISTIC
            int maleUsers=_context.User.Count(u => u.Gender == Models.User.Genders.Männlich);
            int femaleUsers = _context.User.Count(u => u.Gender == Models.User.Genders.Weiblich);
            int diverseUsers = _context.User.Count(u => u.Gender == Models.User.Genders.Divers);
           

            ViewBag.GenderStatisticMale = maleUsers;
            ViewBag.GenderStatisticFemale = femaleUsers;
            ViewBag.GenderStatisticDiverse = diverseUsers;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}