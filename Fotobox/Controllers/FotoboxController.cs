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
    private readonly IHubContext<FotoboxHub, IFotoboxClient> hubContext;
    private readonly IActionSingleton singleton;
    private readonly IHttpClientFactory httpClientFactory;

    private readonly string picturesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Hoellefaescht\\Pictures");
    private readonly string deletedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Hoellefaescht\\Deleted");
    private readonly string digiCamControlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "digiCamControl", "Session1");

    public FotoboxController(IHubContext<FotoboxHub, IFotoboxClient> hubContext, IActionSingleton singleton, IHttpClientFactory httpClientFactory)
    {
      this.hubContext = hubContext;
      this.singleton = singleton;
      this.httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public ActionResult<string> TakePicture()
    {
      if (this.singleton.IsLocked)
      {
        return this.BadRequest("wrong");
      }

      this.singleton.IsLocked = true;

      var thread = new Thread(async () =>
      {
        // save cached photo
        if (!string.IsNullOrEmpty(this.singleton.PicturePath))
        {
          this.CopyFile(this.singleton.PicturePath, this.picturesPath);
          this.singleton.PicturePath = string.Empty;
          await this.hubContext.Clients.All.Reset("Speichern...");
        }

        // Take picture with DigiCamControl (name is date & time)

        using (HttpClient client = this.httpClientFactory.CreateClient())
        {
          try
          {
            client.BaseAddress = new Uri("http://localhost:5513/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var date = DateTime.Now.ToString("dd-MM-yyyy");
            var time = DateTime.Now.ToString("HH-mm-ss");

            var countdownThread = new Thread(async () =>
            {
              for (int i = 3; i >= -1; i--)
              {
                await this.hubContext.Clients.All.ChangeCountdown(i);
                Thread.Sleep(TimeSpan.FromSeconds(1));
              }
            });
            countdownThread.Start();
            Thread.Sleep(TimeSpan.FromSeconds(1));

            HttpResponseMessage response = await client.GetAsync($"/?slc=capture&param1={date}&param2={time}");
            this.singleton.PicturePath = Path.Combine(this.digiCamControlPath, $"{date} {time}.jpg");
            
            // string jsonContent = await response.Content.ReadAsStringAsync();

            Thread.Sleep(2000);
            await this.hubContext.Clients.All.ReloadPicture();

            this.singleton.IsLocked = false;
            //await this.hubContext.Clients.All.SendAsync("SaveDeletePicture", });
          }
          catch (HttpRequestException e)
          {
            this.singleton.PicturePath = string.Empty;
          }
          catch (Exception e)
          {
            this.singleton.PicturePath = string.Empty;
            throw;
          }
        }
      });

      thread.Start();
      return this.Ok("wright");
    }

    [HttpGet]
    public ActionResult<string> SavePicture()
    {
      if (this.singleton.IsLocked)
      {
        return this.BadRequest("wrong");
      }

      this.singleton.IsLocked = true;
      var thread = new Thread(async () =>
      {
        if (!string.IsNullOrEmpty(this.singleton.PicturePath))
        {
          this.CopyFile(this.singleton.PicturePath, this.picturesPath);
          await this.hubContext.Clients.All.Reset("Speichern...");
          this.singleton.PicturePath = string.Empty;
        }
        this.singleton.IsLocked = false;
      });

      thread.Start();
      return this.Ok("wright");
    }

    [HttpGet]
    public ActionResult<string> DeletePicture()
    {
      if (this.singleton.IsLocked)
      {
        return this.BadRequest("wrong");
      }

      this.singleton.IsLocked = true;
      var thread = new Thread(async () =>
      {
        if (!string.IsNullOrEmpty(this.singleton.PicturePath))
        {
          this.CopyFile(this.singleton.PicturePath, this.deletedPath);
          this.DeleteFile(this.singleton.PicturePath);
          this.singleton.PicturePath = string.Empty;
          await this.hubContext.Clients.All.Reset("Löschen...");
        }
        this.singleton.IsLocked = false;
      });

      thread.Start();
      return this.Ok("wright");
    }

    [HttpGet]
    public ActionResult<string> TestDigiCamControl()
    {
      var thread = new Thread(async () =>
      {
        using (HttpClient client = this.httpClientFactory.CreateClient())
        {
          try
          {
            client.BaseAddress = new Uri("http://localhost:5513/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var date = DateTime.Now.ToString("dd-MM-yyyy");
            var time = DateTime.Now.ToString("HH-mm-ss");

            HttpResponseMessage response = await client.GetAsync($"/?slc=capture&param1={date}&param2={time}");

            // string jsonContent = await response.Content.ReadAsStringAsync();
            await this.hubContext.Clients.All.ReloadPicture();
          }
          catch (Exception e)
          {
            Console.WriteLine(e);
            throw;
          }
        }
      });
      thread.Start();

      return "";
    }

    private void CopyFile(string sourceFileName, string destinationDirectory)
    {
      if (System.IO.File.Exists(sourceFileName))
      {
        Directory.CreateDirectory(destinationDirectory);
        var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName));
        System.IO.File.Copy(sourceFileName, destinationFileName, true);
      }
    }

    private void DeleteFile(string sourceFileName)
    {
      if (System.IO.File.Exists(sourceFileName))
      {
        System.IO.File.Delete(sourceFileName);
      }
    }
  }
}