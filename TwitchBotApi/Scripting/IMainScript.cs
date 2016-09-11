namespace TwitchBotApi.Scripting
{
    /*
     * Interface for generic information about the bot.
     * Only one script must inherit from this.
     */
    public interface IMainScript : IScript
    {
        //twitch irc server info
        string Host { get; }
        ushort Port { get; }

        //User info
        string Username { get; }
        //Password is the oath key
        string Password { get; } 

        //Connection timeout values. Use 0 for infinite.
        int SendTimeout { get; }
        //WARNING: setting a small ReceiveTimeout value may disconnect the bot if the chat is not active
        int ReceiveTimeout { get; }

        //Maximum number of network messages sent in the time interval
        int DefaultRateLimit { get; }
        //Interval time in seconds
        int DefaultRateLimitInterval { get; }

        //The entry point for the application.
        void Main();
    }
}
