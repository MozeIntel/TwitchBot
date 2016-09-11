using System;

namespace TwitchBotApi.Network
{
    /// <summary>
    /// Used to limit network output.
    /// </summary>
    public class RateLimiter
    {
        private int currentRate;
        private DateTime? lastSendTime;

        private volatile int limit;
        private volatile int interval;

        public RateLimiter(int startLimit, int startInterval)
        {
            currentRate = 0;
            lastSendTime = null;

            limit = startLimit;
            interval = startInterval;
        }

        public void UpdateLimit(int newLimit)
        {
            limit = newLimit;
        }

        public void UpdateInterval(int intervalSecs)
        {
            interval = intervalSecs;
        }

        /// <summary>
        /// Try to increase the current rate.
        /// </summary>
        /// <returns>The time (in seconds) the caller should wait (0 if below the rate limit).</returns>
        public int Increase()
        {
            if (limit <= 0)
            {
                return 0;
            }

            DateTime currentTime = DateTime.Now;

            if (lastSendTime == null)
            {
                lastSendTime = currentTime;
                currentRate = 1;
                return 0;
            }

            int timeDiff = (int)currentTime.Subtract(lastSendTime.Value).TotalSeconds;

            if (timeDiff > interval)
            {
                lastSendTime = currentTime;
                currentRate = 1;
                return 0;
            }

            if (++currentRate > limit)
            {
                return (interval + 1) - timeDiff;
            }

            return 0;
        }
    }
}
