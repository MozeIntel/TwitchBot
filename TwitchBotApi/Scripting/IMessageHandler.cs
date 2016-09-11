using TwitchBotApi.IRC;

namespace TwitchBotApi.Scripting
{
    /// <summary>
    /// Interface for scripts that handle a specific IRC command (PING, JOIN, PRIVMSG etc).
    /// </summary>
    public interface IMessageHandler : IScript
    {
        /// <summary>
        /// The IRC command this script handles. Should be unique among all scripts.
        /// </summary>
        string Command { get; }


        void Handle(IRCMessage msg);
    }
}
