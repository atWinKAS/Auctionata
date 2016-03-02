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

                var user1 = new AuctionUser()
                {
                    UserName = "jd",
                    Email = "john@mail.com",

                };

                await this.userManager.CreateAsync(user1, "P@ssw0rd");

                var user2 = new AuctionUser()
                {
                    UserName = "dj",
                    Email = "jane@mail.com",

                };

                await this.userManager.CreateAsync(user2, "P@ssw0rd");
            }

            if (!this.context.Items.Any())
            {
                // Add new Data
                var item = new Item
                {
                    Name = "Car",
                    Description = "AUDI PNG car image",
                    Picture = "http://pngimg.com/upload/audi_PNG1739.png",
                    Created = DateTime.UtcNow,
                    UserName = "jd",
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
