using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zwitscher.Models;

namespace Zwitscher.Data
{
    public class ZwitscherContext : DbContext //DBContext for Entity Framework
    {
        public ZwitscherContext (DbContextOptions<ZwitscherContext> options)
            : base(options)
        {
        }
        

        public DbSet<Zwitscher.Models.User> User { get; set; } = default!;

        public DbSet<Zwitscher.Models.Role> Role { get; set; } = default!;

        public DbSet<Zwitscher.Models.Post> Post { get; set; } = default!;

        public DbSet<Zwitscher.Models.Comment> Comment { get; set; } = default!;
        public DbSet<Zwitscher.Models.Media> Media { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.FollowedBy)
                .WithMany(u => u.Following)
                .UsingEntity(j => j.ToTable("UserFollowers"));
        }
        public DbSet<Zwitscher.Models.Vote> Vote { get; set; } = default!;
    }
}
