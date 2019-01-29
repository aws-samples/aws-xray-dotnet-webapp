using Amazon.XRay.Recorder.Handlers.AspNet;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using System.Web.Http;

namespace SampleEBWebApplication
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            base.Init();
            AWSXRayASPNET.RegisterXRay(this, "ASPNETSampleApp"); // Configuring web app with X-Ray
            AWSSDKHandler.RegisterXRayForAllServices(); // Configure AWS SDK Handler for X-Ray 
        }
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
