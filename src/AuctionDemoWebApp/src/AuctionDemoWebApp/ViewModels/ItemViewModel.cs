using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionDemoWebApp.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 5)]
        public string Name { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public double CurrentPrice { get; set; }

        public IEnumerable<BidViewModel> Bids { get; set; } 

    }
}
