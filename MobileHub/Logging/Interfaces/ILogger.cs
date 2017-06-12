﻿using System;

namespace Logging.Interfaces
{
    public interface ILogger
    {
        
        void LogMessage(int processId, string message, bool isDone = false );
        void LogError(Exception e, int processId, string message);
        void LogWarning(int processId, string message);

    }
}
