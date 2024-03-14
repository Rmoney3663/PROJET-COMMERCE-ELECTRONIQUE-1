using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Projet_Web_Commerce.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projet_Web_Commerce
{
    public class Notifications : Hub
    {
        private readonly UserManager<Utilisateur> _userManager;
        private readonly Dictionary<string, string> _userConnections = new Dictionary<string, string>();

        public Notifications(UserManager<Utilisateur> userManager)
        {
            _userManager = userManager;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.Identity?.Name;
            if (userId != null)
            {
                var connectionId = Context.ConnectionId;
                _userConnections[userId] = connectionId;
            }

            return base.OnConnectedAsync();
        }


        public async Task NotificationMessage(string userId)
        {
            if (_userConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage");
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.Identity?.Name;
            if (userId != null && _userConnections.ContainsKey(userId))
            {
                _userConnections.Remove(userId);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
