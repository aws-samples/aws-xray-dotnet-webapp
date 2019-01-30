using System.IO;
using System.Reflection;
using Amazon;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SAmpleEBASPNETCoreApplication
{
    public class Startup
    {
        public static ILog log;
        public IConfiguration Configuration { get; }

        static Startup() // create log4j instance
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log = LogManager.GetLogger(typeof(Startup));
            AWSXRayRecorder.RegisterLogger(LoggingOptions.Log4Net);
        }


        public Startup(IConfiguration configuration)
        {
            AWSXRayRecorder.InitializeInstance(configuration: Configuration); // Inititalizing Configuration object with X-Ray recorder
            AWSSDKHandler.RegisterXRayForAllServices(); // All AWS SDK requests will be traced
        }
      
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseXRay("ASPNETCoreSampleApp"); // Integrate ASPNETCore app with X-Ray
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
