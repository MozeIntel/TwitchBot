namespace TwitchBotApi.Scripting
{
    /// <summary>
    /// Interface all scripts inherit from.
    /// </summary>
    /// <remarks>
    /// <para>Don't use this directly, inherit from a lower-level interface: this is mainly used for internal code.</para>
    /// <para>All scripts will be executed in a multi-threaded context</para>
    /// </remarks>
    public interface IScript
    {
        //Empty interface is empty.
    }
}
