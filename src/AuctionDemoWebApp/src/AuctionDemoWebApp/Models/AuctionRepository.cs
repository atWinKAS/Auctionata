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
            var theItem = this.GetItemByName(itemName);
            if(theItem != null)
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
        
        public bool ClearBids(string itemName)
        {
            var item = this.context.Items.Include(i => i.Bids).FirstOrDefault(i => i.Name.Equals(itemName));
            if (item != null && item.Bids != null && item.Bids.Any())
            {
                foreach(var bid in item.Bids)
                {
                    this.context.Bids.Remove(bid);
                }
                
                return true;
            }
            
            return false;
        }
    }
}
