using System;
using Logging;
using Microsoft.AspNet.SignalR;

namespace MobileHumWebApi.Hubs
{
    public class RouteHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void BroadcastRealTime()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RouteHub>();
            context.Clients.All.setRealTime(DateTime.Now.ToString("h:mm:ss tt"));
        }
        public void BroadcastRouteCalculationProgress(ProgressQueueMessage message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RouteHub>();
            context.Clients.All.routeCalculationProgress(message);
        }
    }
}