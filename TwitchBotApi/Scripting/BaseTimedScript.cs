using System;
using System.Timers;

namespace TwitchBotApi.Scripting
{
    public abstract class BaseTimedScript : IScript, IDisposable
    {
        //Interval of time (in millisecs) between callback invocations
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

        //Logic implementation goes in here.
        protected abstract void Run();

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
