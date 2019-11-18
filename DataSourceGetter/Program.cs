using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace DataSourceGetter
{
    public class Program
    {
        private static string certificateFileName { get; set; }
        private static string certificatePassword { get; set; }
        private static string ipaddress { get; set; }
        private static int port { get; set; }

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
            .Build();
            var certificateSettings = config.GetSection("certificateSettings");
            certificateFileName = certificateSettings.GetValue<string>("filename");
            certificatePassword = certificateSettings.GetValue<string>("password");

            var httpsBindingsSettings = config.GetSection("httpsBindings");
            ipaddress = httpsBindingsSettings.GetValue<string>("ipaddress");
            port = httpsBindingsSettings.GetValue<int>("port");

            //var certificate = new X509Certificate2(certificateFileName, certificatePassword);
            //var host = new WebHostBuilder()
            //.UseKestrel(
            //    options =>
            //    {
            //        options.AddServerHeader = false;
            //        options.Listen(IPAddress.Parse(ipaddress), port, listenOptions =>
            //        {
            //            listenOptions.UseHttps(certificate);
            //        });
            //    }
            //)
            //.UseConfiguration(config)
            //.UseContentRoot(Directory.GetCurrentDirectory())
            //.UseStartup<Startup>()
            //.UseUrls()
            //.Build();

            //host.Run();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                   {
                       options.Listen(ipaddress == "*" ? IPAddress.Any : IPAddress.Parse(ipaddress), port, listenOptions =>
                         {
                             listenOptions.UseHttps(certificateFileName, certificatePassword);
                         });
                       options.ConfigureHttpsDefaults(listenOptions =>
                       {
                           listenOptions.SslProtocols = SslProtocols.Tls;
                       });
                   });
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("http://*:80/");
                    webBuilder.UseUrls(); //по умолчанию возьмет из appsetings
                });
    }
}
