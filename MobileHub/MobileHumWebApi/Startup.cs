
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MobileHumWebApi.Startup))]

namespace MobileHumWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
