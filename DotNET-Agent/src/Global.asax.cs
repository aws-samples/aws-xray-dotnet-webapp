using System.Web.Http;

namespace SampleWebApplication
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            base.Init();
        }

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
