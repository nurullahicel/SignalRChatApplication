using Microsoft.AspNetCore.SignalR;
using SignalRChatApplication.Data;
using SignalRChatApplication.Models;

namespace SignalRChatApplication.Hubs
{
    public class ChatHub:Hub
    {
        public async Task GetUsername(string username)
        {
            Client client = new Client
            {
                ConnectionId=Context.ConnectionId,
                Username = username
            };
            ClientSource.Clients.Add(client);
            await Clients.Others.SendAsync("clientJoined", username);
            await Clients.All.SendAsync("clients", ClientSource.Clients);
        }
        public async Task SendMessageAsync(string message,string clientName )
        {
            clientName=clientName.Trim();
            Client senderClient = ClientSource.Clients.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (clientName=="All") {
                await Clients.All.SendAsync("receiveMessage", message,senderClient.Username);
            }
            else
            {
                Client client = ClientSource.Clients.FirstOrDefault(c => c.Username == clientName);
                await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, senderClient.Username);

            }
            
          
        }
    }
}
