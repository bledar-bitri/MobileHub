using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Utilities;
using Newtonsoft.Json;
using Parameters;

namespace RouteGenerationWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public static string QueueName = "routegeneration";

        public override void Run()
        {
            Trace.TraceInformation("RouteGenerationWorkerRole is running");

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

            Trace.TraceInformation("RouteGenerationWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("RouteGenerationWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("RouteGenerationWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var queue = new MobileAppCloudQueue(QueueName);

            while (!cancellationToken.IsCancellationRequested)
            {
                var msg = queue.GetMessage();
                while (msg != null)
                {
                    if (msg == null) break;
                    Trace.TraceInformation(string.Format("message: {0}", msg.AsString));
                    var parameters = JsonConvert.DeserializeObject<RouteRequestParameters>(msg.AsString);
                    Trace.TraceInformation(string.Format("RouteRequestParameters: {0}", parameters.ToString()));
                    queue.DeleteMessage(msg);
                    msg = queue.GetMessage();
                }
                await Task.Delay(1000);
            }
        }
    }
}
