using System;

namespace Logging.Interfaces
{
    public interface ILogger
    {
        
        void LogMessage(string clientId, int processId, string message, int progressProcent, bool isDone = false );
        void LogError(string clientId, Exception e, int processId, string message);
        void LogWarning(string clientId, int processId, string message);

    }
}
