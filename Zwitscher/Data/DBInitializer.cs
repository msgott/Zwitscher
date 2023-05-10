using System.Diagnostics;
using Zwitscher.Models;

namespace Zwitscher.Data
{
    public class DBInitializer
    {
        public static void Initialize(ZwitscherContext context)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.User.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
            new User{FirstName="Carson",LastName="Alexander",Username="AlexCarson",Password="test123", Birthday=DateTime.Parse("2005-09-01")},
            new User{FirstName="Meredith",LastName="Alonso",Username="MeredithAlonso",Password="test1234", Birthday=DateTime.Parse("2002-04-01")},
            new User{FirstName="Arturo",LastName="Anand",Username="ArturoAnand",Password="test1235", Birthday=DateTime.Parse("2000-03-01")},
            new User{FirstName="Gytis",LastName="Barzdukas",Username="GytisBarzdukas",Password="test1236", Birthday=DateTime.Parse("2001-10-01")}

            };
            foreach ( User u in users)
            {
                context.User.Add(u);
            }
            context.SaveChanges();

            var roles = new Role[]
            {
            new Role{Name="User"},
            new Role{Name="Moderator"},
            new Role{Name="Administrator"}

            };
            foreach (Role r in roles)
            {
                context.Role.Add(r);
            }
            context.SaveChanges();
        }
    }
}
