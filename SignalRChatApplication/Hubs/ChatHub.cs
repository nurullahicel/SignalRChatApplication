using Microsoft.AspNetCore.SignalR;
using SignalRChatApplication.Data;
using SignalRChatApplication.Models;

namespace SignalRChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        public async Task GetUsername(string username)
        {
            Client client = new Client
            {
                ConnectionId = Context.ConnectionId,
                Username = username
            };
            ClientSource.Clients.Add(client);
            await Clients.Others.SendAsync("clientJoined", username);
            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }
        public async Task SendMessageAsync(string message, string clientName)
        {
            clientName = clientName.Trim();
            Client senderClient = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (clientName == "All")
            {
                await Clients.Others.SendAsync("receiveMessage", message, senderClient.Username);
            }
            else
            {
                Client client = ClientSource.Clients.FirstOrDefault(c => c.Username == clientName);
                await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, senderClient.Username);

            }


        }
        public async Task AddGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Group group = new Group { GroupName = groupName };
            group.Clients.Add(ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId));
            GroupSource.Groups.Add(group);
            await Clients.All.SendAsync("groups", GroupSource.Groups);
        }
        public async Task AddClientToGroup(IEnumerable<string> groupNames)
        {
            Client client = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            foreach (string group in groupNames)
            {
                Group _group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == group);
                var result = _group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);
                if (!result)
                {
                    _group.Clients.Add(client);
                    await Groups.AddToGroupAsync(Context.ConnectionId, group);
                }
            }

        }
        public async Task GetClientToGroup(string roomName)
        {
       
            Group group = GroupSource.Groups.FirstOrDefault(g => g.GroupName == roomName);
            await Clients.Caller.SendAsync("clients",roomName=="-1"?ClientSource.Clients: group.Clients);
        }
        public async Task SendMessageToGroupAsync(string roomName,string message)
        {
            await Clients.Group(roomName).SendAsync("receiveMessage",message,
                ClientSource.Clients.FirstOrDefault(c=>c.ConnectionId==Context.ConnectionId).Username);
        }
    }
}
