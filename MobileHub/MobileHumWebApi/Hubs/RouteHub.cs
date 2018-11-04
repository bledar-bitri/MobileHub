using System;
using Logging;
using Microsoft.AspNet.SignalR;

namespace MobileHumWebApi.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        public void Hello()
        {
            Clients.All.hello();
        }

        /// <summary>
        /// 
        /// </summary>
        public void BroadcastRealTime()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RouteHub>();
            context.Clients.All.setRealTime(DateTime.Now.ToString("h:mm:ss tt"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void BroadcastRouteCalculationProgress(ProgressQueueMessage message)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RouteHub>();
            context.Clients.All.routeCalculationProgress(message);
        }
    }
}