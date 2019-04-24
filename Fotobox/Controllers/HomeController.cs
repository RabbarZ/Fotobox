using System.Threading.Tasks;
using Fotobox.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fotobox.Controllers
{
  public class HomeController : Controller
  {
    private readonly IHubContext<FotoboxHub> hubContext;
    //private HubConnection connection;

    public HomeController(IHubContext<FotoboxHub> hubContext)
    {
      this.hubContext = hubContext;
    }

    public IActionResult Index()
    {
      return this.View();
    }

    public async Task<IActionResult> Admin()
    {
      //var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
      await this.hubContext.Clients.All.SendCoreAsync("SendMessage", new object[] { "hello world" });
      return this.View();
    }
  }
}
