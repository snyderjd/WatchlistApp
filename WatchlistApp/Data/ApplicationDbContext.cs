using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WatchlistApp.Models;

namespace WatchlistApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUser");
            modelBuilder.Entity<Stock>().ToTable("Stock");
            modelBuilder.Entity<Watchlist>().ToTable("Watchlist");

            base.OnModelCreating(modelBuilder);

            // Create a new user for Identity Framework
            ApplicationUser user = new ApplicationUser
            {
                FirstName = "admin",
                LastName = "admin",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = "7f434309-a4d9-48e9-9ebb-8803db794577",
                Id = "00000000-ffff-ffff-ffff-ffffffffffff"
            };
            var passwordHash = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHash.HashPassword(user, "Admin8*");
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            Stock stock1 = new Stock
            {
                Id = 1,
                Name = "S&P 500",
                Ticker = "SPY"
            };
            modelBuilder.Entity<Stock>().HasData(stock1);

            Stock stock2 = new Stock
            {
                Id = 2,
                Name = "John Deere",
                Ticker = "DE"
            };
            modelBuilder.Entity<Stock>().HasData(stock2);

            Stock stock3 = new Stock
            {
                Id = 3,
                Name = "Apple",
                Ticker = "AAPL"
            };
            modelBuilder.Entity<Stock>().HasData(stock3);

            Stock stock4 = new Stock
            {
                Id = 4,
                Name = "Delta Airlines",
                Ticker = "DAL"
            };
            modelBuilder.Entity<Stock>().HasData(stock4);

            Stock stock5 = new Stock
            {
                Id = 5,
                Name = "Microsoft",
                Ticker = "MSFT"
            };
            modelBuilder.Entity<Stock>().HasData(stock5);


        }
    }
}
