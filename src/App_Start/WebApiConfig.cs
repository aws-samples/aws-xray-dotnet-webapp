using System.Web.Http;
using Amazon.XRay.Recorder.Handler.Http;
using SampleEBWebApplication.Controllers;

namespace SampleEBWebApplication
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Add the message handler to HttpCofiguration
            config.MessageHandlers.Add(new TracingMessageHandler());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
           
        }
    }
}
