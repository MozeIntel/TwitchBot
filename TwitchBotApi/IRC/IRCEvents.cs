using System;
using System.Collections.Concurrent;

namespace TwitchBotApi.IRC
{
    /*
     * Helper class for handling all IRC events.
     * This class is thread safe.
     * The EventArgs are usually IRCMessage objects, but can be user defined.
     * Multiple callbacks can be registered for a single event
     */ 
    public static class IRCEvents
    {
        private static ConcurrentDictionary<string, Action<EventArgs>> eventMap = new ConcurrentDictionary<string, Action<EventArgs>>();

        public static void Register(string name, Action<EventArgs> callback)
        {
            if (!eventMap.TryAdd(name, callback))
            {
                eventMap[name] += callback;
            }
        }

        public static void Invoke(string name, EventArgs args)
        {
            Action<EventArgs> action;

            if (eventMap.TryGetValue(name, out action))
            {
                action.Invoke(args);
            }
        }

        public static void Invoke(string name)
        {
            Action<EventArgs> action;

            if (eventMap.TryGetValue(name, out action))
            {
                action.Invoke(null);
            }
        }

        public static bool HasEvent(string name)
        {
            return eventMap.ContainsKey(name);
        }

        public static void Clear()
        {
            eventMap.Clear();
        }
    }
}
