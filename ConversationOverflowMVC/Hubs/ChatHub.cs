using ConversationOverflowMVC.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConversationOverflowMVC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        public async Task SendMessage(string login, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", login, message);
        }
        public async Task SendPrivateMessage(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveMessage", message);
        }

        public async Task SendToUser(List<int> userIds, string receiverConnectionId, string message)
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessageFromUser", userIds, message);
        }

        public async Task SendToUserConnectionIds(List<int> userIds, IEnumerable<string> connectionIds, string message)
        {
            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessageFromUser", userIds, message);
            }
        }
        
        public async Task SendToUserLogins(List<int> userIds, List<string> logins, string message)
        {
            foreach(var login in logins)
            {
                IEnumerable<string> connectionIds = GetConnections(login);

                await SendToUserConnectionIds(userIds, connectionIds, message);
            }
        }

        public async Task SendToGroup(int userId, string groupId, string message)
        {
            await Clients.Groups(groupId).SendAsync("ReceiveMessageFromGroup", userId, groupId, message);
        }

        public async Task SendAttachmentToGroup(int userId, string groupId, string attachment)
        {
            await Clients.Groups(groupId).SendAsync("ReceiveAttachmentFromGroup", userId, groupId, attachment);
        }

        public async Task SendToUserForTyping(int groupId, string login)
        {
            IEnumerable<string> connectionIds = GetConnections(login);

            foreach (var connectionId in connectionIds)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessageFromUserForTyping", groupId);
            }
        }

        public async Task AddToGroup(IEnumerable<string> connectionIds, string groupId) 
        {
            foreach(var connectionId in connectionIds)
            {
                await Groups.AddToGroupAsync(connectionId, groupId);
            }
        }

        public async Task AddToGroupByLogins(List<string> logins, string groupId)
        {
            foreach(var login in logins)
            {
                IEnumerable<string> connectionId = GetConnections(login);

                await AddToGroup(connectionId, groupId);
            }
        }

        public string GetConnectionId() => Context.ConnectionId;

        public string GetUserName() => Context.User.Identity.Name;

        public int GetCount() => _connections.Count;

        public IEnumerable<string> GetConnections(string login) => _connections.GetConnections(login); 

        public override async Task OnConnectedAsync()
        {
            //await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            string name = Context.User.Identity.Name;
            _connections.Add(name, Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
