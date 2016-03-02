using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionDemoWebApp.ViewModels
{
    public class BidViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public double Price { get; set; }

        public DateTime Created { get; set; }
    }
}
