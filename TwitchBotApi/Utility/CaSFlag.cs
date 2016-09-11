using System.Threading;

namespace TwitchBotApi.Utility
{
    /// <summary>
    /// Compare-and-Swap flag, used for lockless multi-threading sync.
    /// </summary>
    public class CasFlag
    {
        private volatile int flag;

        public CasFlag(bool flagState = true)
        {
            flag = flagState ? 1 : 0;
        }

        /// <summary>
        /// Tries to update the flag to the set state.
        /// </summary>
        /// <returns>Whether the flag was updated.</returns>
        public bool Set()
        {
            return Interlocked.CompareExchange(ref flag, 1, 0) == 0;
        }

        /// <summary>
        /// Tries to update the flag to the clear state.
        /// </summary>
        /// <returns>Whether the flag was updated.</returns>
        public bool Clear()
        {
            return Interlocked.CompareExchange(ref flag, 0, 1) == 1;
        }

        public bool State()
        {
            //Reads on a volatile variable are atomic, and our var is marked volatile
            return flag == 1 ? true : false;
        }
    }
}
