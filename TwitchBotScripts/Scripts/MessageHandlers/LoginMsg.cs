using TwitchBotApi.IRC;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace Scripts.MessageHandlers
{ 
    //Msg format: :<sender> 001 <user> :<Message>
    public class LoginMsg : IMessageHandler
    {
        public string Command { get { return "001"; } }

        public void Handle(IRCMessage msg)
        {
            Logger.Info("Logged in! Server message: {0}", msg.Message);
            IRCEvents.Invoke("Login");
        }
    }
}
