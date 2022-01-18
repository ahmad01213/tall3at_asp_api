using System;
using Donia.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Donia.Dtos
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Category> categories { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<Notif> notifs { get; set; }
        public DbSet<Photo> photos { get; set; }
        public DbSet<CreditCard> credit_cards { get; set; }

        public DbSet<User> AspNetUsers { get; set; }

        public DbSet<Market> markets { get; set; }
        public DbSet<Food> foods { get; set; }
        public DbSet<Driver> drivers { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<Slider> sliders { get; set; }
        public DbSet<Field> fields { get; set; }
        public DbSet<Option> options { get; set; }
        public DbSet<OptionGroup> OptionGroups { get; set; }
        public DbSet<CartGroupOption> CartGroupoptions { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Address> addresses { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<DriverOrder> driverOrders { get; set; }
        public DbSet<FieldMarket> fieldMarkets { get; set; }

    }

}

