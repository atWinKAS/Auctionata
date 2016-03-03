using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace AuctionDemoWebApp.Hub
{
    public class CommunicationHub : Microsoft.AspNet.SignalR.Hub
    {
        public void Send(string message)
        {
            ////Clients.All.broadcastMessage(name, message);
            this.Log($"Client Message: {message}");
        }

        public override Task OnConnected()
        {
            this.Log($"Client Connected {Context.ConnectionId}");
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            this.Log($"Client Disconnected {Context.ConnectionId}");
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Communication hub: {message}");
        }
    }
}
