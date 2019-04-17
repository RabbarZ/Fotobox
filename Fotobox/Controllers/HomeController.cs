using System;
using System.Threading.Tasks;
using Fotobox.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fotobox.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<FotoboxHub> hubContext;
        private HubConnection connection;
        public HomeController(IHubContext<FotoboxHub> hubContext)
        {
            // this.hubContext = hubContext;
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/ChatHub")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";
                    messagesList.Items.Add(newMessage);
                });
            });

            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
                connectButton.IsEnabled = false;
                sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
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
