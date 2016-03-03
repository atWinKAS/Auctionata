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
            ////Clients.All.broadcastMessage(name, message);
        }

        public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}
