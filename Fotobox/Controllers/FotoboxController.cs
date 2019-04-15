using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fotobox.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Fotobox.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class FotoboxController : ControllerBase
    {
        private bool running = false;
        private DateTime date;
        private readonly IHubContext<FotoboxHub> hubContext;
        private readonly ISession session;

        public FotoboxController(IHubContext<FotoboxHub> hubContext, ISession session)
        {
            this.hubContext = hubContext;
            this.session = session;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Function(long id)
        {
            //if (this.running)
            //{
            //    return "wrong";
            //}
            //this.running = true;

            //var tasks = new[]
            //{
            //    await Task.Run(this.Waiter)
            //};

            var thread = new Thread(this.Execute);
            thread.Start();

            this.running = true;

            return "wright";
        }

        private async Task<bool> Waiter()
        {
            if(this.date < DateTime.Now.AddSeconds(-100))
            this.date = DateTime.Now;
            Thread.Sleep(1000);
            while (DateTime.Now.AddSeconds(-20) < this.date)
            {
                await this.Waiter();
            }
            this.running = false;
            return true;
        }

        private void Execute()
        {

        }
    }
}