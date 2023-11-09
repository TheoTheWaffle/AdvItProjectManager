using Microsoft.Extensions.Configuration.Json;
using System.Runtime.InteropServices;

namespace ProjectManager.Api;
public static class Program
{
    private static string ContentRootPath = Directory.GetCurrentDirectory();

    public static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                for (int pos = configurationBuilder.Sources.Count - 1; pos >= 0; --pos)
                {
                    ContentRootPath = hostBuilderContext.HostingEnvironment.ContentRootPath;
                    if (configurationBuilder.Sources[pos] is JsonConfigurationSource)
                    {
                        var source = new JsonConfigurationSource()
                        {
                            Path = Path.Join(ContentRootPath, "appsettings.local.json"),
                            Optional = true,
                            ReloadOnChange = true,
                        };
                        source.ResolveFileProvider();
                        configurationBuilder.Sources.Insert(pos + 1, source);
                    }
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}