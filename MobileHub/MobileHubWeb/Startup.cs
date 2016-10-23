using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MobileHubWeb.Startup))]

namespace MobileHubWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
