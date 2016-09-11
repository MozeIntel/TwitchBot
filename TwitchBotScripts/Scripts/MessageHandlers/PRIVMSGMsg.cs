using TwitchBotApi.IRC;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace Scripts.MessageHandlers
{
    /*
     * A PRIVMSG is a use message in a specific channel.
     * Format: @<tags>; :<sender> PRIVMSG #channel :<msg>
     */
    class PRIVMSGMsg : IMessageHandler
    {
        public string Command { get { return "PRIVMSG"; } }

        public void Handle(IRCMessage msg)
        {
            string sender = msg.Sender.Substring(0, msg.Sender.IndexOf('!'));
            string channel = msg.Params[0].Substring(1);

            IRCUser user = new IRCUser(sender, channel, msg.Tags);
            Logger.Info("{0}(Sub:{1}, Mod:{2}): {3}", user.DisplayName, user.Subscriber, msg.Tags["mod"] == "1", msg.Message);
        }
    }
}
