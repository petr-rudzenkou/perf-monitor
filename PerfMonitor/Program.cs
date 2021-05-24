using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace PerfMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = AppStartup();
            var runner = ActivatorUtilities.CreateInstance<PerfCounterRunner>(host.Services);
            runner.Run();
        }

        static void ConfigSetup(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            ConfigSetup(builder);

            var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
                {

                    services.AddSingleton<IPerfCounterProvider, PerfCounterProvider>();
                    services.AddSingleton<IPerfCounterOutput, PerfCounterConsoleOutput>();
                    services.AddSingleton<IPerfCounterRunner, PerfCounterRunner>();
                })
                .Build();

            return host;
        }
    }
}
