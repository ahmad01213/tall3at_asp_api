using System;
using Donia.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Donia.Dtos
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Trib>()
            .ToTable("trips");
        }
        public DbSet<Ad> ads { get; set; }
        public DbSet<Booking> bookings { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<Notif> notifs { get; set; }
        public DbSet<Photo> photos { get; set; }
        public DbSet<Service> services { get; set; }
        public DbSet<Story> stories { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Trib> trips { get; set; }
        public DbSet<Triporg> triporgs { get; set; }
    }

}

