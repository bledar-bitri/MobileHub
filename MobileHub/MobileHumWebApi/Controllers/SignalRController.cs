using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using MobileHumWebApi.Hubs;
using Monitoring;

namespace MobileHumWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SignalRController : ApiController
    {
        private CancellationToken _token = new CancellationToken();

        // GET api/values
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Get()
        {

            Task.Run(async () => await StartAsync(), _token);

            return new List<string>{"Signal R Monitoring Task Started!"};
        }

        private async Task StartAsync()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    await ProcessMessagesAsync();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex, "Exception in worker role Run loop.");
                }
                await Task.Delay(1000, _token);
            }
            
        }

        private async Task ProcessMessagesAsync()
        {
            var monitor = new ProgressQueueMonitor();
            var hub = new RouteHub();
            while (true)
            {
                var message = await monitor.GetMessageAsync();
                if (message == null)
                    break;

                hub.BroadcastRouteCalculationProgress(message);
            }
        }
    }
}
