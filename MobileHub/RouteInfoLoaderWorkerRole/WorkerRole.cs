using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json;
using Ninject;
using Parameters;
using Services;
using Utilities;

namespace RouteInfoLoaderWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public static string QueueName = "routegeneration";
        private static MobileAppCloudQueue queue;

        public override void Run()
        {
            Trace.TraceInformation("RouteInfoLoaderWorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("RouteInfoLoaderWorkerRole has been started");
            queue = new MobileAppCloudQueue(QueueName);

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("RouteInfoLoaderWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("RouteInfoLoaderWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000, cancellationToken);
                var msg = queue.GetMessage();
                if (msg == null) continue;

                var requestParams = JsonConvert.DeserializeObject<RouteRequestParameters>(msg.AsString);
                Trace.TraceInformation($"Params UserID: {requestParams.UserId}");

                
                using (var service = kernel.Get<IRouteService>())
                {
                    var bestTour = service.GetRouteForUserId(1);
                    bestTour.ForEach(c => Trace.TraceInformation(c.DebuggerDisplay));
                }
                
                queue.DeleteMessage(msg);
            }
        }
    }
}
