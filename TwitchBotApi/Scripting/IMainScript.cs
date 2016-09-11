namespace TwitchBotApi.Scripting
{
    /// <summary>
    /// Interface for generic information about the bot.
    /// </summary>
    /// <remarks>
    /// Only and at least one script should inherit from this interface.
    /// </remarks>
    public interface IMainScript : IScript
    {
        /// <summary>Twitch IRC server info</summary>
        string Host { get; }

        /// <summary>Twitch IRC server info</summary>
        ushort Port { get; }

        /// <summary>User info</summary>
        /// <remarks>Should be in lower case</remarks>
        string Username { get; }

        /// <summary>User info</summary>
        /// <remarks>The password is the oauth key, not the account password</remarks>
        string Password { get; } 

        /// <summary>Connection timeout</summary>
        /// <remarks>Setting this to 0 will make the timeout infinite</remarks>
        int SendTimeout { get; }

        /// <summary>Connection timeout</summary>
        /// <remarks>
        /// <para>Setting this to 0 will make the timeout infinite</para>
        /// <para>Setting this to a low value may disconnect the bot randomly (chat inactivity)</para>
        /// </remarks>
        int ReceiveTimeout { get; }

        /// <summary>IRC message limiter value</summary>
        int DefaultRateLimit { get; }

        /// <summary>IRC message limiter interval (in seconds)</summary>
        int DefaultRateLimitInterval { get; }


        ///<summary> Entry point for the <seealso cref="TwitchBotApi.Scripting.ScriptEngine"/> </summary>
        void Main();
    }
}
