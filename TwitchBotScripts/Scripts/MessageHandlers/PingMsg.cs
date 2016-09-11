using TwitchBotApi.IRC;
using TwitchBotApi.Network;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace Scripts.MessageHandlers
{
    /*
     * Ping messages are sent every x minutes from the IRC server to disconnect inactive clients.
     * The format is PING :<msg>, and the reply format should be PONG :<msg>
     */ 
    public class PingMsg : IMessageHandler
    {
        public string Command { get { return "PING"; } }

        public void Handle(IRCMessage msg)
        {
            Connection.Pong(msg.Message);
            Logger.Info("Received PING from server (message: {0})", msg.Message);
            IRCEvents.Invoke("Ping", msg);
        }
    }
}
