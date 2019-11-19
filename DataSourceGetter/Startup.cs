using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataSourceGetter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private DataSourceGetterService _DSGservice;
        private ApplicationConfiguration _conf;
        private ILogger _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<ApplicationConfiguration>(Configuration.GetSection("ApplicationConfiguration"));
            services.AddSingleton<DataSourceGetterService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime, ILogger<Startup> logger)
        {
            _DSGservice = app.ApplicationServices.GetRequiredService<DataSourceGetterService>();
            _conf = app.ApplicationServices.GetRequiredService<IOptions<ApplicationConfiguration>>().Value;
            _logger = logger;
            applicationLifetime.ApplicationStopping.Register(OnShutdown);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void OnShutdown()
        {
            //this code is called when the application stops
            try
            {
                _logger.LogTrace("Results savind started, please wait");
                File.AppendAllLines(Path.Combine(_conf.DataSourceFilePath, "result.txt"), _DSGservice.GetCurrentState());//+ DateTime.Now.ToShortTimeString() + "-" + DateTime.Now.ToShortTimeString() + "
                _DSGservice.SaveCurrentStates();
                _logger.LogTrace("Results saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

    }
}
