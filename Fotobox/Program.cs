using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Fotobox
{
  public class Program
  {
    public static void Main(string[] args)
    {
      WebHost.CreateDefaultBuilder(args)
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseUrls("http://*:5000", "https://*:5001", "http://*:51327", "https://*:44391")
        .UseStartup<Startup>()
        .Build()
        .Run();
    }
  }
}
