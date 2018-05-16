using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ball.Gameplay;

namespace Ball.Stats
{
    public enum EventType
    {
        None,
        MatchStart,
        MatchEnd,
        Shoot,
        Tackle,
    }

    public class Event
    {
        public EventType Type;
        public Dictionary<String, Object> Data;

        public Event(EventType type = EventType.None)
        {
            Type = type;
            Data = new Dictionary<string, object>();
        }
    }
}
