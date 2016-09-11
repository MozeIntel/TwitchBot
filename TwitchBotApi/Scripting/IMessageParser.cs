using TwitchBotApi.IRC;

namespace TwitchBotApi.Scripting
{
    /*
     * Interface for the IRC message parser.
     * This interface is optional, the ScriptEngine will load a default implementation if none is provided.
     * If you do choose to implement your own, only one script should inherit this.
     */ 
    public interface IMessageParser : IScript
    {
        IRCMessage ParseMessage(string message);
    }
}
