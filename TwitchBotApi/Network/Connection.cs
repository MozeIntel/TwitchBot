using System;
using System.Net.Sockets;
using TwitchBotApi.IRC;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace TwitchBotApi.Network
{
    /*
     * Static class for network I/O handling
     */
    public static class Connection
    {
        public const int CONNECT_COOLDOWN_TIME = 10;

        public static TcpClient Socket { get; private set; }

        private static SocketReader reader;
        private static SocketWriter writer;
        private static CasFlag connectingFlag;
        private static CasFlag disconnectingFlag;

        static Connection()
        {
            reader = new SocketReader(); 
            writer = new SocketWriter();
            connectingFlag = new CasFlag(false);
            disconnectingFlag = new CasFlag(false);

            IRCEvents.Register("Disconnect", OnDisconnect);
        }

        //Coninuosly attemps connection in a 10 second interval. Method is non-blocking
        public static void Open()
        {
            if (connectingFlag.Set())
            {
                if (Socket != null)
                {
                    Socket.Client.Dispose();
                }

                Socket = new TcpClient();
                Socket.ReceiveTimeout = ScriptEngine.MainScript.ReceiveTimeout;
                Socket.SendTimeout = ScriptEngine.MainScript.SendTimeout;

                BeginConnect();
            }
        }

        private static void OnDisconnect(EventArgs args)
        {
            if (disconnectingFlag.Set())
            {
                Logger.Warn("Lost connection to remove server, attempting re-connect in {0} seconds", CONNECT_COOLDOWN_TIME);
                TimerHelper.SingleShot(Open, CONNECT_COOLDOWN_TIME * 1000);
            }
        }

        private static void BeginConnect()
        {
            Logger.Info("Connecting to {0}:{1}...", ScriptEngine.MainScript.Host, ScriptEngine.MainScript.Port);
            Socket.BeginConnect(ScriptEngine.MainScript.Host, ScriptEngine.MainScript.Port, EndConnect, Socket);
        }

        private static void EndConnect(IAsyncResult ar)
        {
            try
            {
                Socket.EndConnect(ar);
            }
            catch (Exception e)
            {
                Logger.Fatal("Connection failed: {0}", e);
                Logger.Warn("Attempting again in {0} seconds", CONNECT_COOLDOWN_TIME);

                TimerHelper.SingleShot(BeginConnect, CONNECT_COOLDOWN_TIME * 1000);
                return;
            }

            Logger.Success("Connected to {0}:{1}", ScriptEngine.MainScript.Host, ScriptEngine.MainScript.Port);

            connectingFlag.Clear();
            disconnectingFlag.Clear();

            //Notify the R/W IO threads before the event listeners
            reader.OnConnect();
            writer.OnConnect();

            IRCEvents.Invoke("Connect");
            Login();
        }

        //Login with the credentials specified in the IMainScript
        private static void Login()
        {
            writer.SendMessage(true, "PASS {0}", ScriptEngine.MainScript.Password);
            writer.SendMessage(true, "NICK {0}", ScriptEngine.MainScript.Username);
        }

        /*
         * Join an IRC chatroom.
         * The channel must NOT start with the trailing '#'
         */
        public static void JoinChannel(string channel)
        {
            writer.SendMessage(true, "JOIN #{0}", channel.ToLower());
        }

        /*
         * Request a Capability.
         * Currently supports membership, commands and tags
         */
        public static void RequestCap(string cap)
        {
            writer.SendMessage(false, "CAP REQ :twitch.tv/{0}", cap.ToLower());
        }

        //Reply to a PING message
        public static void Pong(string pingMessage)
        {
            writer.SendMessage(false, "PONG :{0}", pingMessage);
        }

        /*
         * Writes the message as is to the network buffer.
         */ 
        public static void SendRaw(string message, bool rateLimit = false)
        {
            writer.SendMessage(rateLimit, message);
        }
    }
}
