using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Facebook_Mvc.Models;
using Facebook_Mvc.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Facebook_Mvc.Data
{
    public class Facebook_MvcContext :DbContext
    {
        public Facebook_MvcContext(DbContextOptions<Facebook_MvcContext> options)
         : base(options)
        { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(b => b.CreateTime)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Post>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Comment>().Property(b => b.CommentedDate).HasDefaultValueSql("getdate()");
            modelBuilder.Entity<Friend>()
            .HasKey(e => new { e.RequestedById, e.RequestedToId });
            modelBuilder.Entity<Messages>().Property(b => b.SendDate).HasDefaultValueSql("getdate()");
        }
        public DbSet<User> User { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Friend> Friend { get; set; }
        public DbSet<Messages> Messages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=FacebookDb;Trusted_Connection=True;");
        }
    }
}
