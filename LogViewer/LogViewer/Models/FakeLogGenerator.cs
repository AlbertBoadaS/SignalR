using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace LogViewer.Models
{
    public class FakeLogGenerator
    {
        private readonly string[] _levels = new string[] { "trace", "debug", "info", "warn", "error", "fatal" };
        private ISimpleLogger _logger = new SimpleHubLogger<LogEndpoint>();

        private int _levelNdx;
        private void GenerateFakeLog() {
            var level = _levels[_levelNdx++];
            _levelNdx %= _levels.Length;
            _logger.Log(level, string.Format("Sample {0} log message", level));
        }
        private void Loop() {
            while (true) {
                GenerateFakeLog();
                Thread.Sleep(10000);
            }
        }

        public static void SpawnThread() {
            new Thread(new FakeLogGenerator().Loop).Start();
        }

    }
}