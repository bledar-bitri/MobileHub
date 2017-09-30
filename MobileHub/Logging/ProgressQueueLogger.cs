using System;
using System.Diagnostics;
using Common;
using Logging.Interfaces;
using Newtonsoft.Json;
using Utilities;

namespace Logging
{
    public class ProgressQueueLogger : ILogger
    {
        private readonly MobileAppCloudQueue _queue = new MobileAppCloudQueue(CommonConfigValues.ProgressLogQueueName);
        
        public void LogError(Exception e, int processId, string message)
        {
            Trace.TraceError(message);
            _queue.AddMessage(JsonConvert.SerializeObject(CreateMessage(processId, message, 0, false, true)));
            
        }

        public void LogMessage(int processId, string message, int progressProcent, bool isDone = false)
        {
            Trace.TraceInformation(message);
            _queue.AddMessage(JsonConvert.SerializeObject(CreateMessage(processId, message, progressProcent, isDone)));
        }

        public void LogWarning(int processId, string message)
        {
            Trace.TraceWarning(message);
            _queue.AddMessage(JsonConvert.SerializeObject(CreateMessage(processId, message)));
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
