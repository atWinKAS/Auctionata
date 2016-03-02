using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace AuctionDemoWebApp.Models
{
    public class AuctionContext : IdentityDbContext<AuctionUser>
    {
        public AuctionContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Bid> Bids { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //// change database connection string for production and developemnt database 
            
            var connString = Startup.Configuration["Data:AuctionContextConnection"];

            optionsBuilder.UseSqlServer(connString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
