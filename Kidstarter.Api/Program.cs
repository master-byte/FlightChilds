using System.IO;
using System.Threading.Tasks;

using Kidstarter.Api.Extensions;

using Lamar.Microsoft.DependencyInjection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Kidstarter.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLamar()
                .UseSerilog((hc, l) => l.ConfigureSerilog(hc.Configuration))
                .ConfigureAppConfiguration(
                    (_, config) => { config.AddEnvironmentVariables(); })
                .ConfigureWebHostDefaults(
                    webHostBuilder =>
                    {
                        webHostBuilder
                            .ConfigureKestrel(
                                options =>
                                {
                                    options.Limits.MaxRequestBodySize = 200000000; // Max upload size is 200 Mb
                                    options.AddServerHeader = false;
                                })
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseIISIntegration()
                            .UseStartup<Startup>();

                        webHostBuilder.ConfigureServices(services => { services.AddControllers(); });
                    });
    }
}