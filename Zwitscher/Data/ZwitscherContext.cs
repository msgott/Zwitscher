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
        public DbSet<Zwitscher.Models.Vote> Vote { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Users
            modelBuilder.Entity<User>()
                .HasMany(u => u.FollowedBy)
                .WithMany(u => u.Following)
                .UsingEntity(j => j.ToTable("UserFollowers"));
                
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.BlockedBy)
                .WithMany(u => u.Blocking)
                .UsingEntity(j => j.ToTable("UserBlockers"));

            modelBuilder.Entity<Post>()
                .HasOne(u => u.User)
                .WithMany(u => u.Posts)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(u => u.User)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vote>()
                .HasOne(u => u.User)
                .WithMany(u => u.Votes)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Media>()
                .HasOne(u => u.User)
                .WithOne(u => u.ProfilePicture)
                .OnDelete(DeleteBehavior.ClientSetNull);


            //Comments
            modelBuilder.Entity<Comment>()
                .HasOne(u => u.commentsComment)
                .WithMany(u => u.commentedBy)
                .OnDelete(DeleteBehavior.ClientCascade);

            //Posts
            modelBuilder.Entity<Media>()
                .HasOne(u => u.Post)
                .WithMany(u => u.Media)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Vote>()
                .HasOne(u => u.Post)
                .WithMany(u => u.Votes)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Comment>()
                .HasOne(u => u.Post)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            //Votes
            modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.UserId, v.PostId })
            .IsUnique();
        }
    }
}
