using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Fotobox.Hubs
{
    public class FotoboxHub : Hub
    {
        public async Task SendMessage(string parameter)
        {
            await this.Clients.All.SendAsync("ReceiveMessage", parameter);
        }
    }
}
