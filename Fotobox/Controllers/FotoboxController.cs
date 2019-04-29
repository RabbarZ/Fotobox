using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Fotobox.Hubs;
using Fotobox.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fotobox.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class FotoboxController : ControllerBase
  {
    //private DateTime date;
    private readonly IHubContext<FotoboxHub> hubContext;
    private readonly IActionSingleton instance;
    private readonly IHttpClientFactory clientFactory;

    public FotoboxController(IHubContext<FotoboxHub> hubContext, IActionSingleton instance, IHttpClientFactory client)
    {
      this.hubContext = hubContext;
      this.instance = instance;
      this.clientFactory = client;
    }

    [HttpGet]
    public ActionResult<string> TakePicture()
    {
      if (this.instance.IsLocked)
      {
        return "wrong";
      }

      this.instance.IsLocked = true;

      var thread = new Thread(async () =>
      {
        // save cached photo
        if (!string.IsNullOrEmpty(this.instance.Picture))
        {
          var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Hoellefaescht\\Pictures");
          if (!Directory.Exists(path))
          {
            Directory.CreateDirectory(path);
          }
          System.IO.File.Copy(this.instance.Picture, path);
          await this.hubContext.Clients.All.SendAsync("Reset", "Speichern...");
          this.instance.Picture = string.Empty;
        }

        await this.hubContext.Clients.All.SendAsync("Countdown");
        Thread.Sleep(4000);

        // Take picture with DigiCamControl (name is date & time)

        var client = clientFactory.CreateClient();

        client.BaseAddress = new Uri("http://localhost:5513/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
          new MediaTypeWithQualityHeaderValue("application/json"));

        var date = DateTime.Now.ToString("dd-MM-yyyy");
        var time = DateTime.Now.ToString("HH-mm-ss");

        var response = await client.GetAsync($"/?slc=capture&param1={date}&param2={time}");
        this.instance.Picture = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"{date}_{time}");

        if (!response.IsSuccessStatusCode)
        {
          await this.hubContext.Clients.All.SendAsync("Reset", string.Empty);
        }

        var content = response.Content;

        Thread.Sleep(2000);
        await this.hubContext.Clients.All.SendAsync("ReloadPicture");

        this.instance.IsLocked = false;
        //await this.hubContext.Clients.All.SendAsync("SaveDeletePicture", });
      });

      thread.Start();
      return "Wright";
    }

    [HttpGet]
    public ActionResult<string> SavePicture()
    {
      if (this.instance.IsLocked)
      {
        return "wrong";
      }

      this.instance.IsLocked = true;
      var thread = new Thread(async () =>
      {
        if (!string.IsNullOrEmpty(this.instance.Picture))
        {
          var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Hoellefaescht\\Pictures");
          if (!Directory.Exists(path))
          {
            Directory.CreateDirectory(path);
          }
          System.IO.File.Copy(this.instance.Picture, path);
          await this.hubContext.Clients.All.SendAsync("Reset", "Speichern...");
          this.instance.Picture = string.Empty;
        }
        this.instance.IsLocked = false;
      });

      thread.Start();
      return "wright";
    }

    [HttpGet]
    public ActionResult<string> DeletePicture()
    {
      if (this.instance.IsLocked)
      {
        return "wrong";
      }

      this.instance.IsLocked = true;
      var thread = new Thread(async () =>
      {
        if (!string.IsNullOrEmpty(this.instance.Picture))
        {
          var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Hoellefaescht\\Deleted");
          if (!Directory.Exists(path))
          {
            Directory.CreateDirectory(path);
          }
          System.IO.File.Copy(this.instance.Picture, path);
          await this.hubContext.Clients.All.SendAsync("Reset", "Löschen...");
          this.instance.Picture = string.Empty;
        }
          this.instance.IsLocked = false;
      });

      thread.Start();
      return "wright";
    }
  }
}