using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MsdnSpy.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateWebHostBuilder(args).Build().Run();
            Listener.RunNew();
            Console.WriteLine("Drop the mic");
            Console.ReadKey();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}