using TwitchBotApi.IRC;

namespace TwitchBotApi.Scripting
{
    /*
     * Interface for IRC message handling scripts.
     * All scripts inheriting from this are executed asynchronously.
     */
    public interface IMessageHandler : IScript
    {
        //The IRC command (JOIN, PING etc) this script handles. Should be unique for each script.
        string Command { get; }

        //The IRCMessage.Command will always equal IMessageHandler.Command.
        void Handle(IRCMessage msg);
    }
}
