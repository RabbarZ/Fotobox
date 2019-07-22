using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Fotobox.Hubs;
using Fotobox.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

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

    private readonly string copyPathRelative;
    private readonly string savedPathRelative;
    private readonly string deletedPathRelative;
    private readonly string digiCamControlPath;

    public FotoboxController(IHubContext<FotoboxHub, IFotoboxClient> hubContext, IActionSingleton singleton, IHttpClientFactory httpClientFactory, IHostingEnvironment environment, IConfiguration configuration)
    {
      this.hubContext = hubContext;
      this.singleton = singleton;
      this.httpClientFactory = httpClientFactory;

      var pictureFolderPath = "wwwroot\\Hoellefaescht";
      this.copyPathRelative = Path.Combine(pictureFolderPath, "Copy");
      this.savedPathRelative = Path.Combine(pictureFolderPath, "Saved");
      this.deletedPathRelative = Path.Combine(pictureFolderPath, "Deleted");
      this.digiCamControlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "digiCamControl", configuration["Session"]);

      // var asd = Path.GetFullPath("wwwroot/hoellefaesch/copy");
      // var asd1 = Path.GetFullPath("/wwwroot/hoellefaesch/copy");
      // var asd2 = Path.GetFullPath("./wwwroot/hoellefaesch/copy");

      // Test 
      /*var asd = Path.Combine(pictureFolderPath, $"{new Random().Next(1, 4)}.png");
      var sd = asd.Split("wwwroot")[1];
      this.hubContext.Clients.All.ReloadPicture(sd);*/
    }

    [HttpGet]
    public ActionResult<string> TakePicture()
    {
      if (this.singleton.IsLocked)
      {
        return this.BadRequest("Server is locked..");
      }

      this.singleton.IsLocked = true;

      var thread = new Thread(async () =>
      {
        // save cached photo
        this.SavePictureLocal();

        // Take picture with DigiCamControl (name is date & time)
        using (HttpClient client = this.httpClientFactory.CreateClient())
        {
          try
          {
            client.BaseAddress = new Uri("http://localhost:5513/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var countdownThread = new Thread(async () =>
            {
              for (int i = 3; i >= -1; i--)
              {
                if (i == 0)
                {
                  await this.hubContext.Clients.All.ShowText("Foto!!");
                  Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                else if (i == -1)
                {
                  await this.hubContext.Clients.All.ShowText("Foto wird verarbeitet...");
                }
                else
                {
                  await this.hubContext.Clients.All.ShowText(i.ToString());
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
              }
            });
            countdownThread.Start();
            Thread.Sleep(TimeSpan.FromSeconds(2.6));
            
            var date = DateTime.Now.ToString("dd-MM-yyyy");
            var time = DateTime.Now.ToString("HH-mm-ss");

            /*await client.GetAsync($"/?slc=capture&param1={date}_{time}");
            Thread.Sleep(2000);

            var fileName = $"{date}_{time}.jpg";

            this.CopyFile(Path.Combine(this.digiCamControlPath, fileName), this.copyPathRelative);
            this.singleton.PicturePath = Path.Combine(this.copyPathRelative, fileName);*/
            this.singleton.PicturePath = @"C:\Users\huerlik2\source\Workspace\Fotobox\Fotobox\wwwroot\asd.png";
            await this.hubContext.Clients.All.ReloadPicture(this.singleton.PicturePath.Split("wwwroot")[1]);

            this.singleton.IsLocked = false;
          }
          catch (HttpRequestException)
          {
            this.singleton.PicturePath = string.Empty;
            this.singleton.IsLocked = false;
          }
          catch (Exception)
          {
            this.singleton.PicturePath = string.Empty;
            this.singleton.IsLocked = false;
          }
        }
      });

      thread.Start();
      return this.Ok("Take picture successful..");
    }

    [HttpGet]
    public ActionResult<string> SavePicture()
    {
      if (this.singleton.IsLocked)
      {
        return this.BadRequest("Server is locked..");
      }

      this.singleton.IsLocked = true;
      var thread = new Thread(() =>
      {
        this.SavePictureLocal();
        this.singleton.IsLocked = false;
      });

      thread.Start();
      return this.Ok("Saved picture..");
    }

    [HttpGet]
    public ActionResult<string> DeletePicture()
    {
      if (this.singleton.IsLocked)
      {
        return this.BadRequest("Server is locked..");
      }

      this.singleton.IsLocked = true;
      var thread = new Thread(() =>
      {
        this.DeletePictureLocal();
        this.singleton.IsLocked = false;
      });

      thread.Start();
      return this.Ok("Deleted picture..");
    }

    private async void SavePictureLocal()
    {
      // await this.hubContext.Clients.All.Reset("Foto gespeichert.\nBuzzer drücken um Foto zu machen.");
      if (!string.IsNullOrEmpty(this.singleton.PicturePath))
      {
        this.CopyFile(this.singleton.PicturePath, this.savedPathRelative);
        this.singleton.PicturePath = string.Empty;
        await this.hubContext.Clients.All.Reset("Foto gespeichert.\nBuzzer drücken um Foto zu machen.");
      }
    }

    private async void DeletePictureLocal()
    {
      if (!string.IsNullOrEmpty(this.singleton.PicturePath))
      {
        this.CopyFile(this.singleton.PicturePath, this.deletedPathRelative);
        this.DeleteFile(this.singleton.PicturePath);
        this.singleton.PicturePath = string.Empty;
        await this.hubContext.Clients.All.Reset("Foto gelöscht.\nBuzzer drücken um Foto zu machen.");
      }
    }

    private void DeleteFile(string sourceFileName)
    {
      if (System.IO.File.Exists(sourceFileName))
      {
        System.IO.File.Delete(sourceFileName);
      }
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
  }
}