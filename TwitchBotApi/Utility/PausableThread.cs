using System.Threading;

namespace TwitchBotApi.Utility
{
    public enum ThreadState
    {
        PAUSED,
        RUNNING,
        STOPPED
    }


    public abstract class PausableThread
    {
        private Thread thread;
        private ThreadState state;

        //We need a lock, since we will be using Monitor functions
        private object lockObj;

        //The thread starts in the paused state.
        public PausableThread()
        {
            lockObj = new object();
            state = ThreadState.PAUSED;
            thread = new Thread(ThreadMethod);
            thread.Start();
        }

        private void UpdateState(ThreadState newState)
        {
            lock (lockObj)
            {
                state = newState;
                Monitor.Pulse(lockObj);
            }
        }

        protected void SetRunning()
        {
            UpdateState(ThreadState.RUNNING);
        }

        protected void SetPaused()
        {
            UpdateState(ThreadState.PAUSED);
        }

        protected void SetStopped()
        {
            UpdateState(ThreadState.STOPPED);
        }

        //The thread state does NOT reset to Paused after the method is run.
        public abstract void Run();

        private void ThreadMethod()
        {
            while (true)
            {
                lock (lockObj)
                {
                    while (state == ThreadState.PAUSED)
                    {
                        Monitor.Wait(lockObj);
                    }

                    if (state == ThreadState.STOPPED)
                    {
                        return;
                    }
                }

                Run();
            }
        }

    }
}
