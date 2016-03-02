using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionDemoWebApp.Models
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public DateTime Created { get; set; }

        public string UserName { get; set; }

        public ICollection<Bid> Bids { get; set; } 
    }
}
