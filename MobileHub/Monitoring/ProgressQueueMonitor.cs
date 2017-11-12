
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Logging;
using Monitoring.Interfaces;
using Newtonsoft.Json;
using Utilities;

namespace Monitoring
{
    public class ProgressQueueMonitor : IMonitor
    {
        private readonly MobileAppCloudQueue _queue = new MobileAppCloudQueue(CommonConfigValues.ProgressLogQueueName);

        public void LogError(Exception e, int processId, string message)
        {
            Trace.TraceError(message);
            _queue.AddMessage(JsonConvert.SerializeObject(CreateMessage(processId, message, 0, false, true)));

        }

        // Processes any messages on the queue.
        public async Task<ProgressQueueMessage> GetMessageAsync()
        {
            
            var message = await _queue.CloudQueue.GetMessageAsync(TimeSpan.FromSeconds(30), null, null);
            if (message == null)
                return null;
            
            await _queue.CloudQueue.DeleteMessageAsync(message);
            return JsonConvert.DeserializeObject<ProgressQueueMessage>(message.AsString);
            
        }

        public ProgressQueueMessage CreateMessage(int processId, string message, int progressPercent = 0, bool isDone = false, bool isError = false)
        {
            return new ProgressQueueMessage
            {
                ProcessId = processId,
                Text = message,
                IsError = isError,
                IsDone = isDone,
                ProgressPercent = progressPercent
            };
        }
    }
}
