using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using Zwitscher.Models;

namespace Zwitscher.Data
{
    public class DBInitializer //Class for Seeding the Database using hardcoded Dummy Data
    {
        private static ZwitscherContext context;
        public static void Initialize(ZwitscherContext _context) //Defines Dummy Data and uses DBcontext to write to Database
        {
            context = _context;
            context.Database.EnsureCreated();
            seedRole();
            seedUser();
            seedPost();
            seedComment();    

        }
        private static void seedRole()
        {
            //Seed the Role Table 
            if (context.Role.Any()) // Look for any roles.
            {
                return;   // DB has been seeded
            }

            var roles = new Role[]
            {
                new Role{
                    Name="User"
                },
                new Role{
                    Name="Moderator"
                },
                new Role{
                    Name="Administrator"
                }
            };


            foreach (Role r in roles)
            {
                context.Role.Add(r);
            }

            context.SaveChanges();
        }
        private static void seedUser()
        {
            //Seed the User Table 

            if (context.User.Any()) // Look for any users.
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User{
                    FirstName="Carson",
                    LastName="Alexander",
                    Username="AlexCarson",
                    Password="test123",
                    Birthday=DateTime.Parse("2005-09-01"),
                    Role = context.Role.Single(r => r.Name == "User")
                },
                new User{
                    FirstName="Meredith",
                    LastName="Alonso",
                    Username="MeredithAlonso",
                    Password="test1234",
                    Birthday=DateTime.Parse("2002-04-01"),
                    Role = context.Role.Single(r => r.Name == "Moderator")
                },
                new User{
                    FirstName="Arturo",
                    LastName="Anand",
                    Username="ArturoAnand",
                    Password="test1235",
                    Birthday=DateTime.Parse("2000-03-01"),
                    Role = context.Role.Single(r => r.Name == "Administrator")},
                new User{
                    FirstName="Gytis",
                    LastName="Barzdukas",
                    Username="GytisBarzdukas",
                    Password="test1236",
                    Birthday=DateTime.Parse("2001-10-01"),
                    Role = context.Role.Single(r => r.Name == "User")
                }
            };
            foreach (User u in users)
            {
                context.User.Add(u);
            }
            context.SaveChanges();
        }
        private static void seedPost()
        {
            //Seed the Post Table 
            if (context.Post.Any()) // Look for any Posts.
            {
                return;   // DB has been seeded
            }
            var Posts = new Post[]
            {
                new Post{
                    UserID = context.User.Single(u => u.FirstName == "Carson").Id,
                    CreatedDate = DateTime.Parse("2022-03-01"),
                    TextContent = "Das hier ist mein erster Post"
                },
                new Post{
                    UserID = context.User.Single(u => u.FirstName == "Meredith").Id,
                    CreatedDate = DateTime.Parse("2022-04-04"),
                    TextContent = "Das hier ist ein Post"
                },
                new Post{
                    UserID = context.User.Single(u => u.FirstName == "Arturo").Id,
                    CreatedDate = DateTime.Parse("2021-03-01"),
                    TextContent = "Das hier ist mein letzter Post"
                }
            };
            foreach (Post p in Posts)
            {
                context.Post.Add(p);
            }
            context.SaveChanges();
        }
        private static void seedComment()
        {
            //Seed the Comment Table 
            if (context.Comment.Any()) // Look for any Posts.
            {
                return;   // DB has been seeded
            }
            var Comments = new Comment[]
            {
            new Comment{
                UserId = context.User.Single(u => u.FirstName == "Carson").Id,
                PostId = context.Post.Single(p => p.User.FirstName == "Carson").Id,
                CreatedDate = DateTime.Parse("2022-04-01"),
                CommentText = "Das hier ist mein erster Kommentar"
            },
            new Comment{
                UserId = context.User.Single(u => u.FirstName == "Arturo").Id,
                PostId = context.Post.Single(p => p.User.FirstName == "Meredith").Id,
                CreatedDate = DateTime.Parse("2022-04-01"),
                CommentText = "Das hier ist auch mein erster Kommentar"
            }

            };
            foreach (Comment c in Comments)
            {
                context.Comment.Add(c);
            }
            context.SaveChanges();
        }
    }
}
