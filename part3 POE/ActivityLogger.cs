using System;
using System.Collections.Generic;
using System.Text;

namespace MyPOE
{
    public class ActivityLogger
    {
        private readonly List<string> _entries = new List<string>();
        private readonly object _lock = new object();

public ActivityLogger()
{
}

public void LogAction(string message)
{
            if (message == null) return;
            var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            lock (_lock)
            {
                _entries.Add(entry);
            }
        }

        public string GetLogSummary()
        {
            lock (_lock)
            {
                if (_entries.Count == 0) return "No activity recorded.";
                var sb = new StringBuilder();
                foreach (var e in _entries)
                {
                    sb.AppendLine(e);
                }
                return sb.ToString().TrimEnd();
            }
        }
    }
}
