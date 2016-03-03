using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionDemoWebApp.ViewModels
{
    public class TryBidViewModel
    {
        public string ItemName { get; set; }

        public double ClientPrice { get; set; }
    }
}
