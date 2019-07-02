using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Fotobox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                /*.UseKestrel()
                .UseIISIntegration()*/
                .UseStartup<Startup>()/*.UseUrls("http://10.138.1.181:5000", "http://10.138.1.181:5001", "http://localhost:5000", "http://localhost:5001")*/;
    }
}
