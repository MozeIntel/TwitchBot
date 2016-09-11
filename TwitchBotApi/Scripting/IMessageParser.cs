using TwitchBotApi.IRC;

namespace TwitchBotApi.Scripting
{
    /// <summary>
    /// Optional interface for the IRC message parser.
    /// </summary>
    /// <remarks>
    /// The <see cref="TwitchBotApi.Scripting.ScriptEngine"/> will provide a default implementation, if needed.
    /// </remarks>
    public interface IMessageParser : IScript
    {
        IRCMessage ParseMessage(string message);
    }
}
