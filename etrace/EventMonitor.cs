using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using System.Diagnostics;

namespace etrace
{
    class WatchedEvent
    {
        public string StartEvent { get; set; }
        public string EndEvent { get; set; }
        public TimeSpan TimeStamp { get; set; }
    }

    class EventMonitor
    {
        public EventMonitor()
        {
            _motitoredMap = new Dictionary<string, WatchedEvent>();
        }

        private Dictionary<string, WatchedEvent> _motitoredMap;

        private Stopwatch _stopwatch;
        private Stopwatch Stopwatch
        {
            get
            {
                if (_stopwatch == null)
                    _stopwatch = new Stopwatch();
                return _stopwatch;
            }
        }

        internal void Monitor(string startEvent, string endEvent)
        {
            Stopwatch.Start();

            var ev = new WatchedEvent()
            {
                EndEvent = endEvent,
                StartEvent = startEvent
            };

            _motitoredMap[startEvent] = ev;
            _motitoredMap[endEvent] = ev;
        }

        internal void Process(string eventName)
        {
            if (IsMonitored(eventName))
            {
                var e  =_motitoredMap[eventName];

                if(e.StartEvent == eventName)
                {
                    e.TimeStamp = Stopwatch.Elapsed;
                }
                else
                {
                    e.TimeStamp = _stopwatch.Elapsed - e.TimeStamp;
                }
            }
        }

        internal bool IsMonitored(string eventName)
        {
            return _motitoredMap.ContainsKey(eventName);
        }
    }
}
