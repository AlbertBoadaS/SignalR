using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;
using SignalR;
using SignalR.Infrastructure;
using SignalR.Hosting.AspNet;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace LogViewer.Models
{

    public class LogEndpoint : Hub, IConnected, IDisconnect
    {
        private static readonly ISet<string> LogLevels = new HashSet<string> { "trace", "debug", "info", "warn", "error", "fatal" };
        private static HashSet<string> _clientIds = new HashSet<string>();
        private ISimpleLogger _logger = new SimpleHubLogger<LogEndpoint>();


        public void WatchLevel(string level)
        {
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, MethodBase.GetCurrentMethod().Name));
            if (LogLevels.Contains(level))
            {
                AddToGroup(level);
                _logger.Info(string.Format("Added user {0} to group {1}", Context.ConnectionId, level));
            }
        }

        public void AddToGroups(params string[] groupNames) {
            foreach (var groupName in groupNames) {
                AddToGroup(groupName);
            }
        }

        public void UnwatchLevel(string level)
        {
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, MethodBase.GetCurrentMethod().Name));
            if (LogLevels.Contains(level))
            {
                RemoveFromGroup(level);
                _logger.Info(string.Format("Removed user {0} from group {1}", Context.ConnectionId, level));
            }
        }

        private Task UpdateTotalClients()
        {
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, MethodBase.GetCurrentMethod().Name));
            return Clients.setTotalUsers(_clientIds.Count());
        }

        public Task Connect(IEnumerable<string> groups)
        {
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, MethodBase.GetCurrentMethod().Name));
            var initalLevels = new string[] {/* "trace", */ "info", "debug", "warn", "error", "fatal" };
            AddToGroups(initalLevels);
            _logger.Debug(string.Format("Added client {0} to levels: {1}", Context.ConnectionId, string.Join(",", initalLevels)));
            _logger.Info("New user connected.  Id: " + Context.ConnectionId);
            _clientIds.Add(Context.ConnectionId);
            return UpdateTotalClients();
        }

        public System.Threading.Tasks.Task Reconnect(IEnumerable<string> groups)
        {
            //leaving this out for now since this was firing repeatedly from browsers at the office.
/*
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, MethodBase.GetCurrentMethod().Name));
            _logger.Info("User reconnected.  Id: " + Context.ConnectionId);
            _clientIds.Add(Context.ConnectionId);
            return UpdateTotalClients();
*/
            return null;
        }

        public System.Threading.Tasks.Task Disconnect()
        {
            _logger.Info("User disconnected.  Id: " + Context.ConnectionId);
            _clientIds.Remove(Context.ConnectionId);
            return UpdateTotalClients();
        }
    }
}