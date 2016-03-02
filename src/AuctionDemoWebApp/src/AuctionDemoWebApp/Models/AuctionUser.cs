using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AuctionDemoWebApp.Models
{
    public class AuctionUser : IdentityUser
    {
        public string UserData { get; set; }
    }
}
