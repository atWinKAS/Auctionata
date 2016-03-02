using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Server.Kestrel.Networking;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;

namespace AuctionDemoWebApp.Models
{
    public class AuctionRepository : IAuctionRepository
    {
        private AuctionContext context;
        private ILogger<AuctionContext> logger;

        public AuctionRepository(AuctionContext context, ILogger<AuctionContext> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public IEnumerable<Item> GetAllItems()
        {
            try
            {
                return this.context.Items.OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Unable to get items", ex);
                return null;
            }
        }

        public IEnumerable<Item> GetAllItemsWithBids()
        {
            try
            {
                return this.context.Items.Include(t => t.Bids).OrderBy(t => t.Name).ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Unable to get items with bids", ex);
                return null;
            }
        }

        public void AddItem(Item newItem)
        {
            this.context.Items.Add(newItem);
        }

        public bool SaveAll()
        {
            return this.context.SaveChanges() > 0;
        }

        public Item GetItemByName(string itemName)
        {
            return this.context.Items
                .Include(t => t.Bids)
                .FirstOrDefault(t => t.Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase));
        }

        public void AddBid(string itemName, Bid newBid)
        {
            var theItem = this.GetItemByName(itemName);
            newBid.Price = theItem.CurrentPrice;

            theItem.Bids.Add(newBid);

            this.context.Bids.Add(newBid);
        }

        public IEnumerable<Item> GetUserItemsWithBids(string userName)
        {
            try
            {
                return this.context.Items.Include(t => t.Bids).OrderBy(t => t.Name).Where(t => t.UserName.Equals(userName)).ToList();
            }
            catch (Exception ex)
            {
                this.logger.LogError("Unable to get items with bids", ex);
                return null;
            }

        }

        public void UpdateItem(Item item)
        {
            var currentItem = this.context.Items.FirstOrDefault(f => f.Id == item.Id);
            if (currentItem != null)
            {
                currentItem.CurrentPrice = item.CurrentPrice;
            }
        }
    }
}
