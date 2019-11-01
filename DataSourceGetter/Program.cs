using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DataSourceGetter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("http://*:80/");
                    webBuilder.UseUrls(); //по умолчанию возьмет из appsetings
                });
    }
}
