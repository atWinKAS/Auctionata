using System.Collections.Generic;

namespace AuctionDemoWebApp.Models
{
    public interface IAuctionRepository
    {
        IEnumerable<Item> GetAllItems();

        IEnumerable<Item> GetAllItemsWithBids();

        void AddItem(Item newItem);

        bool SaveAll();

        Item GetItemByName(string itemName);

        void AddBid(string itemName, Bid newBid);

        IEnumerable<Item> GetItemsWithBids();
    }
}