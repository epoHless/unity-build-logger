using UnityEngine;

namespace epoHless.Logger
{
    [System.Serializable]
    public class LogEntry
    {
        public string Message;
        public string StackTrace;
        public LogType Type;

        public LogEntry()
        {
        }

        public LogEntry(string message, string stackTrace, LogType type)
        {
            Message = message;
            StackTrace = stackTrace;
            Type = type;
        }
    }
}