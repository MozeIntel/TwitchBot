using System;
using TwitchBotApi.IRC;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace Scripts
{
    public class Main : IMainScript
    {
        public string Host { get { return "irc.twitch.tv"; } }
        public ushort Port { get { return 6667; } }

        public string Username { get { return "justinfan12345"; } }
        public string Password { get { return "Kappa"; } }

        public int SendTimeout { get { return 5000; } }
        public int ReceiveTimeout { get { return 0; } }

        public int DefaultRateLimit { get { return 20; } }
        public int DefaultRateLimitInterval { get { return 30; } }

        void IMainScript.Main()
        {
            Logger.Info("Main script starting");
            IRCEvents.Register("Login", OnLogin);
        }

        private void OnLogin(EventArgs args)
        {
            //Join channel
        }
    }
}
