using System.Threading;

namespace TwitchBotApi.Utility
{
    //Compare-and-Swap flag. Useful for threading sync, without needing a lock.
    public class CasFlag
    {
        private volatile int flag;

        public CasFlag(bool flagState = true)
        {
            flag = flagState ? 1 : 0;
        }

        //Checks if the flag is clear: if it is, updates it to set.
        public bool Set()
        {
            return Interlocked.CompareExchange(ref flag, 0, 1) == 0;
        }

        //Checks if the flag is set: if it is, updates it to clear.
        public bool Clear()
        {
            return Interlocked.CompareExchange(ref flag, 1, 0) == 1;
        }
    }
}
