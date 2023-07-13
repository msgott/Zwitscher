using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Text;
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

        [User]
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

        [Moderator]
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
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30); // change to 30 whenever there are some posts available

            int totalPostsThirty = _context.Post.Count(p => p.CreatedDate >= thirtyDaysAgo);
            int numberOfDays = (DateTime.Now - thirtyDaysAgo).Days; 
            double averagePosts = (double)totalPostsThirty / numberOfDays;
            averagePosts = Math.Round(averagePosts, 3); // max. three decimal places

            ViewBag.AveragePosts = averagePosts;

            // BANNED USERS
            int totalBanned = _context.User.Count(u => u.isLocked == true); 
            ViewBag.totalBanned = totalBanned;

            //MOST FAMOUS USER
            
            User mostLiked = null;
            int highestFollowedBy = 0;
            foreach (User user in _context.User.Include(u => u.FollowedBy))
            {
                if (user.FollowedBy.Count > highestFollowedBy)
                {
                    highestFollowedBy = user.FollowedBy.Count;
                    mostLiked = user;
                }

            }
            string mostLikedUser = "-";
            if (mostLiked != null)
            {
                mostLikedUser = mostLiked.Username;
            }


            ViewData["mostLikedUser"] = mostLikedUser;


            List<User> userOrderedByFollowedBy = _context.User.Include(u => u.FollowedBy).OrderBy(u => u.FollowedBy.Count).ToList<User>();
            string mostFamousUser = userOrderedByFollowedBy.Last().Username;
            ViewBag.mostFamousUser = mostFamousUser;

            //MOST HATED USER
            User mostHated = null;
            int highestBlockedBy = 0;
            foreach (User user in _context.User.Include(u=>u.BlockedBy))
            {
                if (user.BlockedBy.Count > highestBlockedBy)
                {
                    highestBlockedBy = user.BlockedBy.Count;
                    mostHated = user;
                }
                
            }
            string mostHatedUser = "-";
            if (mostHated != null) { 
            mostHatedUser = mostHated.Username;
            }
            
            
            ViewData["mostHatedUser"] = mostHatedUser;
            

            //GENDER STATISTIC
            int maleUsers=_context.User.Count(u => u.Gender == Models.User.Genders.Maennlich);
            int femaleUsers = _context.User.Count(u => u.Gender == Models.User.Genders.Weiblich);
            int diverseUsers = _context.User.Count(u => u.Gender == Models.User.Genders.Divers);
           

            double GenderStatisticMale = ((double)maleUsers / totalUsers)*100;
            double GenderStatisticFemale = ((double)femaleUsers / totalUsers)*100;
            double GenderStatisticDiverse = ((double)diverseUsers / totalUsers)*100;

            ViewBag.GenderStatisticMale = Math.Round(GenderStatisticMale, 2);
            ViewBag.GenderStatisticFemale = Math.Round(GenderStatisticFemale, 2);
            ViewBag.GenderStatisticDiverse = Math.Round(GenderStatisticDiverse, 2);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Route("Download/UsersSortedByFollowedBy")]
        public FileResult UsersSortedByFollowedBy()
        {
            string csvresult = "Username,FollowedByCount";
            var users = _context.User.Include(u=>u.FollowedBy).ToList();
            users = users.OrderByDescending(u => u.FollowedBy.Count).ToList();
            
            foreach (var user in users)
            {
                csvresult += System.Environment.NewLine+user.Username + "," + user.FollowedBy.Count;
            }
            
            byte[] bytes = Encoding.UTF8.GetBytes(csvresult);
            return File(bytes, "text/csv", DateTime.Now+"UsersSortedByFollowedBy.csv");
        }
        [HttpGet]
        [Route("Download/UsersSortedByBlockedBy")]
        public FileResult UsersSortedByBlockedBy()
        {
            string csvresult = "Username,BlockedByCount";
            var users = _context.User.Include(u => u.BlockedBy).ToList();
            users = users.OrderByDescending(u => u.BlockedBy.Count).ToList();

            foreach (var user in users)
            {
                csvresult += System.Environment.NewLine + user.Username + "," + user.BlockedBy.Count;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(csvresult);
            return File(bytes, "text/csv", DateTime.Now + "UsersSortedByBlockedBy.csv");
        }
    }
    
}