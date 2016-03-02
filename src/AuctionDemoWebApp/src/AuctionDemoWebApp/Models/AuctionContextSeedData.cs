using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace AuctionDemoWebApp.Models
{
    public class AuctionContextSeedData
    {
        private AuctionContext context;
        private UserManager<AuctionUser> userManager;

        public AuctionContextSeedData(AuctionContext context, UserManager<AuctionUser> userManager)   
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task EnsureSeedData()
        {
            if (await this.userManager.FindByEmailAsync("john@mail.com") == null)
            {
                //// Adding new user

                var user = new AuctionUser()
                {
                    UserName = "jd",
                    Email = "john@mail.com",

                };

                await this.userManager.CreateAsync(user, "P@ssw0rd");

            }

            if (!this.context.Items.Any())
            {
                // Add new Data
                var item = new Item
                {
                    Name = "Car",
                    Created = DateTime.UtcNow,
                    UserName = "jd",
                    CurrentPrice = 100.0,
                    Bids = new List<Bid>
                    {
                        new Bid
                        {
                            Created = DateTime.UtcNow,
                            UserName = "jd",
                            Price = 100.0
                        }
                    }
                };

                context.Items.Add(item);
                context.Bids.AddRange(item.Bids);

                context.SaveChanges();
            }
        }
    }
}
