using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Models;

namespace Zwitscher.Data
{
    public class ZwitscherContext : DbContext
    {
        public ZwitscherContext (DbContextOptions<ZwitscherContext> options)
            : base(options)
        {
        }
        

        public DbSet<Zwitscher.Models.User> User { get; set; } = default!;

        public DbSet<Zwitscher.Models.Role> Role { get; set; } = default!;

        public DbSet<Zwitscher.Models.Post> Post { get; set; } = default!;

        public DbSet<Zwitscher.Models.Comment> Comment { get; set; } = default!;
    }
}
