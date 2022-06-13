using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace WebApi
{
    public class Program
    {
        public static string MethodGet { get; } = "GET";
        public static string MethodPost { get; } = "POST";
        public static string MethodPut { get; } = "PUT";
        public static string MethodPatch { get; } = "PATCH";
        public static string MethodDelete { get; } = "DELETE";

        public static string TempFolder { get; } = "/tmp";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
