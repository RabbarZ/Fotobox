using System;
using System.Threading;
using System.Threading.Tasks;
using Fotobox.Hubs;
using Fotobox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fotobox.Controllers
{
  [Route("api/[action]")]
  [ApiController]
  public class FotoboxController : ControllerBase
  {
    //private DateTime date;
    private readonly IHubContext<FotoboxHub> hubContext;
    private readonly IActionSingleton instance;

    public FotoboxController(IHubContext<FotoboxHub> hubContext, IActionSingleton instance)
    {
      this.hubContext = hubContext;
      this.instance = instance;
    }

    [HttpGet]
    public ActionResult<string> Function(long id)
    {
      if (!this.instance.IsLocked)
      {
        this.instance.IsLocked = true;
        var thread = new Thread(this.Execute);
        thread.Start();
        return "wright";
      }

      return "wrong";
    }

    //[HttpGet]
    //public ActionResult<string> SavePicture()
    //{
    //  if (!this.instance.IsLocked)
    //  {
    //    this.instance.IsLocked = true;
    //    var thread = new Thread(this.Execute);
    //    thread.Start();
    //    return "asd";
    //  }

    //  return "wrong";
    //}

    private async void Execute()
    {
      await this.hubContext.Clients.All.SendCoreAsync("Countdown", new object[] { });
      Thread.Sleep(4000);
      await this.hubContext.Clients.All.SendCoreAsync("ReloadPicture", new object[] { });
      this.instance.IsLocked = false;
    }
  }
}