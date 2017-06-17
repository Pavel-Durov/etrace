using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using System.Diagnostics;
using System.Collections.Concurrent;

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
            _motitoredMap = new ConcurrentDictionary<string, WatchedEvent>();
        }

        private ConcurrentDictionary<string, WatchedEvent> _motitoredMap;

        private object _sync = new object();

        private Stopwatch _stopwatch;

        public Stopwatch Stopwatch
        {
            get
            {
                if (_stopwatch == null)
                {
                    lock (_sync)
                    {
                        if (_stopwatch == null)
                        {
                            _stopwatch = new Stopwatch();
                            _stopwatch.Start();
                        }
                    }
                }
                return _stopwatch;
            }
        }

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
                watchedEvent = _motitoredMap[eventName];

                if (watchedEvent.StartEvent == eventName)
                {
                    watchedEvent.TimeStamp = Stopwatch.Elapsed;
                }
                else
                {
                    WatchedEvent removed; 
                    watchedEvent.TimeStamp = Stopwatch.Elapsed - watchedEvent.TimeStamp;
                    result = true;
                }
            }

            if (!_motitoredMap.Any())
            {
                Stopwatch.Stop();
            }

            return result;
        }

        internal bool IsMonitored(string eventName)
        {
            return _motitoredMap.ContainsKey(eventName);
        }
    }
}
