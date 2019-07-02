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

        private readonly string copyPath;
        private readonly string savedPath;
        private readonly string deletedPath;
        private readonly string digiCamControlPath;

        public FotoboxController(IHubContext<FotoboxHub, IFotoboxClient> hubContext, IActionSingleton singleton, IHttpClientFactory httpClientFactory, IHostingEnvironment environment, IConfiguration configuration)
        {
            this.hubContext = hubContext;
            this.singleton = singleton;
            this.httpClientFactory = httpClientFactory;
            var pictureFolderPath = Path.Combine(environment.ContentRootPath, "wwwroot\\Hoellefaescht");
            this.copyPath = Path.Combine(pictureFolderPath, "Copy");
            this.savedPath = Path.Combine(pictureFolderPath, "Saved");
            this.deletedPath = Path.Combine(pictureFolderPath, "Deleted");
            this.digiCamControlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "digiCamControl", configuration["Session"]);
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
                    this.CopyFile(this.singleton.PicturePath, this.savedPath);
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
                        Thread.Sleep(TimeSpan.FromSeconds(2.3));

                        await client.GetAsync($"/?slc=capture&param1={date}_{time}");
                        Thread.Sleep(2000);

                        this.singleton.PicturePath = this.CopyFile(Path.Combine(this.digiCamControlPath, $"{date}_{time}.jpg"), this.copyPath);
                        // string jsonContent = await response.Content.ReadAsStringAsync();

                        this.ReloadPicture();

                        this.singleton.IsLocked = false;
                        //await this.hubContext.Clients.All.SendAsync("SaveDeletePicture", });
                    }
                    catch (HttpRequestException)
                    {
                        this.singleton.PicturePath = string.Empty;
                    }
                    catch (Exception)
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
                    this.CopyFile(this.singleton.PicturePath, this.savedPath);
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

                        await client.GetAsync($"/?slc=capture&param1={date}&param2={time}");

                        // string jsonContent = await response.Content.ReadAsStringAsync();
                        this.ReloadPicture();
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

        private string CopyFile(string sourceFileName, string destinationDirectory)
        {
            if (System.IO.File.Exists(sourceFileName))
            {
                Directory.CreateDirectory(destinationDirectory);
                var destinationFileName = Path.Combine(destinationDirectory, Path.GetFileName(sourceFileName));
                System.IO.File.Copy(sourceFileName, destinationFileName, true);
                return destinationFileName;
            }

            return string.Empty;
        }

        private void DeleteFile(string sourceFileName)
        {
            if (System.IO.File.Exists(sourceFileName))
            {
                System.IO.File.Delete(sourceFileName);
            }
        }

        private async void ReloadPicture()
        {
            await this.hubContext.Clients.All.ReloadPicture(this.singleton.PicturePath.Split("wwwroot")[1]);
        }
    }
}