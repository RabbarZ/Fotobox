using Fotobox.Models;
using Microsoft.AspNetCore.SignalR;

namespace Fotobox.Hubs
{
    public class FotoboxHub : Hub<IFotoboxClient>
    {
    }
}
