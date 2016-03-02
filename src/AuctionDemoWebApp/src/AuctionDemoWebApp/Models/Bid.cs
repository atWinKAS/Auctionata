using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionDemoWebApp.Models
{
    public class Bid
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public double Price { get; set; }

        public DateTime Created { get; set; }

    }
}
