using Entities.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EMS_Backend.HubConfig
{
    public class ChatHub : Hub
    {

        private static readonly List<ChatUser> _connectedUsers = new List<ChatUser>();

        public async Task SendMessageToSupport(ChatMessage message)
        {
            await Clients.Client(GetSupportConnectionId()).SendAsync("SendMessageToSupport", message);
        }
        public async Task ReplyToClient(ChatMessage message/*, string connectionId*/)
        {
            var tempClientConnectionId = _connectedUsers.Where(prop => !prop.IsSupport).FirstOrDefault().ConnectionId;
            await Clients.Client(tempClientConnectionId).SendAsync("ReplyToClient", message);
        }
        
        //Notification
        public async Task StartAsync(string userName, bool isSupport)
        {
            var currentConnectionId = Context.ConnectionId;
            
            var user = new ChatUser
            {
                UserName = userName,
                ConnectionId = currentConnectionId,
                IsSupport = isSupport,
            };

            var existingUsers = _connectedUsers.Where(prop => prop.UserName == userName).FirstOrDefault();

            if (existingUsers == null)
            {
                _connectedUsers.Add(user);
                await Clients.Client(GetSupportConnectionId()).SendAsync("Joined", userName);
            }
            else
            {
                //Update connection id
                existingUsers.ConnectionId = currentConnectionId;
            }
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Connected, {0}", Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectedUser = _connectedUsers.Where(prop => prop.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if(connectedUser != null)
                _connectedUsers.Remove(connectedUser);

            return base.OnDisconnectedAsync(exception);
        }

        private string GetSupportConnectionId()
        {
            var supportUser = _connectedUsers.Where(prop => prop.IsSupport).FirstOrDefault();
            return supportUser != null ? supportUser.ConnectionId : null;
        }
    }
}
