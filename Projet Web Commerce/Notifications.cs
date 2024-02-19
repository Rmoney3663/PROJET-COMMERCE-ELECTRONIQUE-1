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
        private readonly Dictionary<string, string> _userConnections = new Dictionary<string, string>(); // Mapping of user ID to connection ID

        public Notifications(UserManager<Utilisateur> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                _userConnections[user.Id] = Context.ConnectionId; // Store the mapping
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null && _userConnections.ContainsKey(user.Id))
            {
                _userConnections.Remove(user.Id); // Remove the mapping
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task NotificationMessage(string userId)
        {
            if (_userConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("NouveauMessage");
            }
            else
            {
                // Handle the case where the user is not connected
            }
        }
    }
}
