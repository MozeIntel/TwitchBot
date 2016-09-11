using System.Threading;

namespace TwitchBotApi.Utility
{
    /// <summary>
    /// Represents the execution state of a <see cref="PausableThread"/>.
    /// </summary>
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

        /// <summary>
        /// The method that's called once the <see cref="ThreadState"/> is set to <see cref="ThreadState.RUNNING"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="ThreadState"/> well not be reset to <see cref="ThreadState.PAUSED"/> after execution terminates.
        /// </remarks>
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
