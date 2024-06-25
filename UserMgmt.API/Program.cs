using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace UserMgmt.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the host builder using Host instead of WebHost
            var host = CreateHostBuilder(args).Build();

            // Run the web host
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
