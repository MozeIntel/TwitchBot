using System;
using System.Threading;

namespace TwitchBotApi.Utility
{
    public class TimerHelper
    {
        /// <summary>
        /// Starts a <see cref="System.Threading.Timer"/> that destroys itself after execution.
        /// </summary>
        /// <param name="method">The timer callback</param>
        /// <param name="delay">The time (in milliseconds to wait) before starting execution.</param>
        public static void SingleShot(Action method, int delay)
        {
            Timer t = null;
            t = new Timer((args) => { method.Invoke(); t.Dispose(); }, null, delay, Timeout.Infinite);
        }
    }
}
