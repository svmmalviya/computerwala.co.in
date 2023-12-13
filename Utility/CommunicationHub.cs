using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;

namespace computerwala.Utility
{
    public class CommunicationHub:Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
   
}
