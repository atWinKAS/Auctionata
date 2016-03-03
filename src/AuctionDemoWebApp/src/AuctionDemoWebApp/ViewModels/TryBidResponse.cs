using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionDemoWebApp.ViewModels
{
    public class TryBidResponse
    {
        public string ItemName { get; set; }

        public DateTime Created { get; set; }

        public string UserName { get; set; }

        public double Price { get; set; }

        public bool IsHighest { get; set; }
    }
}
