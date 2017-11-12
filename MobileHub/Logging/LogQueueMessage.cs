﻿namespace Logging
{
    public class LogQueueMessage : QueueMessage
    {
        public int ProcessId { get; set; }
        
        public string Text { get; set; }

        public bool IsError { get; set; }

        public bool IsDone { get; set; }

    }
}
