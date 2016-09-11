using System;
using System.Threading;

namespace TwitchBotApi.Utility
{
    public class TimerHelper
    {
        //Start a timer that destroys itself after one run. The delay is in milliseconds.
        public static void SingleShot(Action method, int delay)
        {
            Timer t = null;
            t = new Timer((args) => { method.Invoke(); t.Dispose(); }, null, delay, Timeout.Infinite);
        }
    }
}
