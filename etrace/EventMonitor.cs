using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using System.Diagnostics;

namespace etrace
{
    internal class WatchedEvent
    {
        public string StartEvent { get; set; }
        public string EndEvent { get; set; }
        public TimeSpan TimeStamp { get; set; }
    }

    internal class EventMonitor
    {
        public EventMonitor()
        {
            _motitoredMap = new Dictionary<string, WatchedEvent>();
        }

        private Dictionary<string, WatchedEvent> _motitoredMap;

        private Stopwatch _stopwatch;

        internal void Monitor(string startEvent, string endEvent)
        {
            var ev = new WatchedEvent()
            {
                EndEvent = endEvent,
                StartEvent = startEvent
            };

            _motitoredMap[startEvent] = ev;
            _motitoredMap[endEvent] = ev;
        }

        internal bool Process(string eventName, out WatchedEvent watchedEvent)
        {
            bool result = false;
            watchedEvent = null;

            if (IsMonitored(eventName))
            {
                var e = _motitoredMap[eventName];

                if (e.StartEvent == eventName)
                {
                    if (_stopwatch == null)
                    {
                        _stopwatch = new Stopwatch();
                        _stopwatch.Start();
                    }

                    e.TimeStamp = _stopwatch.Elapsed;
                }
                else
                {
                    watchedEvent.TimeStamp = _stopwatch.Elapsed - e.TimeStamp;
                    result = true;
                }
            }

            if(!_motitoredMap.Any())
            {
                _stopwatch.Stop();
            }

            return result;
        }

        internal bool IsMonitored(string eventName)
        {
            return _motitoredMap.ContainsKey(eventName);
        }
    }
}
