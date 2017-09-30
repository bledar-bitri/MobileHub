using Microsoft.AspNet.SignalR;

namespace MobileHumWebApi.Hubs
{
    public class RouteHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}