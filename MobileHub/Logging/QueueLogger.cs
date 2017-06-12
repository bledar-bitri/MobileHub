using System;
using System.Diagnostics;
using Common;
using Logging.Interfaces;
using Newtonsoft.Json;
using Utilities;

namespace Logging
{
    public class QueueLogger : ILogger
    {
        private readonly MobileAppCloudQueue _queue = new MobileAppCloudQueue(CommonConfigValues.ProgressLogQueueName);
        
        public void LogError(Exception e, int processId, string message)
        {
            Trace.TraceError(message);
            _queue.AddMessage(JsonConvert.SerializeObject(CreateMessage(processId, message, false, true)));
            
        }

        public void LogMessage(int processId, string message, bool isDone = false)
        {
            Trace.TraceInformation(message);
            _queue.AddMessage(JsonConvert.SerializeObject(CreateMessage(processId, message, isDone)));
        }

        public void LogWarning(int processId, string message)
        {
            Trace.TraceWarning(message);
            _queue.AddMessage(JsonConvert.SerializeObject(CreateMessage(processId, message)));
        }

        public QueueMessage CreateMessage(int processId, string message, bool isDone = false, bool isError = false)
        {
            return new QueueMessage
            {
                ProcessId = processId,
                Text = message,
                IsError = isError,
                IsDone = isDone
            };
        }
    }
}
