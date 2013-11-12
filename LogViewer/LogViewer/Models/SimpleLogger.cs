using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using SignalR.Hubs;
using SignalR;
using SignalR.Infrastructure;
using SignalR.Hosting.AspNet;

namespace LogViewer.Models
{
    public interface ISimpleLogger {
        void Trace(string message);
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
        void Log(string level, string message);
    }
    public class SimpleHubLogger<T> : ISimpleLogger where T: Hub
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public class LogItem
        {
            public string Level { get; set; }
            public DateTime Timestamp { get; set; }
            public string Message { get; set; }

            public LogItem()
            {
                Timestamp = DateTime.UtcNow;
            }
        }
        
        public void Info(string message)
        {
            log.Info(message);
            Log("info", message);
        }

        public void Trace(string message)
        {
            log.Info(message);
            Log("trace", message);
        }

        public void Debug(string message)
        {
            log.Debug(message);
            Log("debug", message);
        }
        public void Warn(string message)
        {
            log.Warn(message);
            Log("warn", message);
        }
        public void Error(string message)
        {
            log.Error(message);
            Log("error", message);
        }
        public void Fatal(string message)
        {
            log.Fatal(message);
            Log("fatal", message);
        }
        public void Log(string loglevel, string message)
        {
            Log(new LogItem() { Level = loglevel, Message = message });
        }
        private void Log(LogItem item)
        {
            
            IConnectionManager connectionManager = AspNetHost.DependencyResolver.Resolve<IConnectionManager>();
            dynamic clients = connectionManager.GetClients<LogEndpoint>();
            clients[item.Level].receiveLogEntry(item);
        }
    }
}