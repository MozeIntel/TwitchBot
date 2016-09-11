using System;
using System.Collections.Concurrent;

namespace TwitchBotApi.IRC
{
    /// <summary>
    /// Helper class for handling IRC events.
    /// </summary>
    /// <remarks>
    /// <para>This class IS thread safe.</para>
    /// <para>The EventArgs are usually <see cref="IRCMessage"/> objects, but can be user defined.</para>
    /// <para>Multiple callbacks can be registered for the same event.</para>
    /// </remarks>
    public static class IRCEvents
    {
        private static ConcurrentDictionary<string, Action<EventArgs>> eventMap = new ConcurrentDictionary<string, Action<EventArgs>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Register an event.
        /// </summary>
        /// <param name="name">The name of the event. Case is ignored.</param>
        /// <param name="callback">The event callback.</param>
        public static void Register(string name, Action<EventArgs> callback)
        {
            if (!eventMap.TryAdd(name, callback))
            {
                eventMap[name] += callback;
            }
        }

        /// <summary>
        /// Raise an event.
        /// </summary>
        /// <param name="name">The name of the event. Case is ignored.</param>
        /// <param name="args">The argurment for the event.</param>
        public static void Invoke(string name, EventArgs args)
        {
            Action<EventArgs> action;

            if (eventMap.TryGetValue(name, out action))
            {
                action.Invoke(args);
            }
        }

        /// <summary>
        /// Raise an event with no argument.
        /// </summary>
        /// <param name="name">The name of the event. Case is ignored.</param>
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
