using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool AddBid(string itemName, Bid newBid)
        {
            //// it should not be possible to add new bid if price in bid is less then max price.
            bool canAdd = false;
            var theItem = this.GetItemByName(itemName);

            if (!theItem.Bids.Any())
            {
                canAdd = true;
            }
            else
            {
                double currMax = theItem.Bids.OrderByDescending(b => b.Price).FirstOrDefault().Price;
                if (newBid.Price >= currMax)
                {
                    canAdd = true;
                }
            }

            //// for a  now let's say all is ok...
            canAdd = true;

            if (canAdd)
                {
                    theItem.Bids.Add(newBid);

                    this.context.Bids.Add(newBid);
                    return true;
                }
            return false;
            
        }

        public IEnumerable<Item> GetItemsWithBids()
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
    }
}
