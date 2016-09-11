using System;
using System.Threading;
using TwitchBotApi.IRC;
using TwitchBotApi.Network;
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

        private CasFlag connectingFlag = new CasFlag(true);

        void IMainScript.Main()
        {
            Logger.Info("Main script starting");
            IRCEvents.Register("Connect", OnConnect);
            IRCEvents.Register("Disconnect", OnDisconnect);
            IRCEvents.Register("Login", OnLogin);

            Connection.Open();
        }

        private void OnConnect(EventArgs args)
        {
            if (connectingFlag.Clear())
            {
                Connection.Login();

                //NOTICE, HOSTTARGET, CLEARCHAT, USERSTATE, RECONNECT, ROOMSTATE, USERNOTICE
                Connection.RequestCap("commands");

                //Add IRCv3 tags to commands
                Connection.RequestCap("tags");
            }
        }

        private void OnDisconnect(EventArgs args)
        {
            if (connectingFlag.Set())
            {
                Logger.Info("Client was disconnected, attempting connection in {0} seconds", Connection.CONNECT_COOLDOWN_TIME);

                Timer timer = null;
                timer = new Timer((obj) => { Connection.Open(); timer.Dispose(); }, null, Connection.CONNECT_COOLDOWN_TIME * 1000, 0);
            }
        }

        private void OnLogin(EventArgs args)
        {
            Connection.JoinChannel("lobosjr");
        }
    }
}
