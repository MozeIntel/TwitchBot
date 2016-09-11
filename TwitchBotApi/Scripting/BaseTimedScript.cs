using System;
using System.Timers;

namespace TwitchBotApi.Scripting
{
    /// <summary>
    /// Abstract class representing a script to be executed at a specific time interval.
    /// </summary>
    public abstract class BaseTimedScript : IScript, IDisposable
    {
        /// <summary>The time interval, in seconds</summary>
        protected abstract int Interval { get; }

        private Timer timer;

        public BaseTimedScript(bool startState = false)
        {
            timer = new Timer(Interval);
            timer.AutoReset = true;
            timer.Elapsed += (sender, args) => { Run(); };
            timer.Enabled = startState;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        /// <summary>Logic implementation goes here.</summary>
        protected abstract void Run();

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
