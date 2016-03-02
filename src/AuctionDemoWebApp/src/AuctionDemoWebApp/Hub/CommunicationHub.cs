using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace AuctionDemoWebApp.Hub
{
    public class CommunicationHub : Microsoft.AspNet.SignalR.Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        
    }
}
