using TwitchBotApi.IRC;

namespace TwitchBotApi.Scripting
{
    /*
     * Interface for the IRC message parser.
     * Only one script must inherit this interface.
     */ 
    public interface IMessageParser : IScript
    {
        IRCMessage ParseMessage(string message);
    }
}
