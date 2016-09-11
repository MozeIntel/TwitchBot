using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBotApi.IRC
{
    public class IRCMessage : EventArgs
    {
        //The IRC command, identifies this message
        public string Command { get; set; }

        //Any extra parameters for the command
        public List<string> Params { get; set; }

        public string Sender { get; set; }

        //The actual message contents
        public string Message { get; set; }

        //Key-Value tags for the message
        public Dictionary<string, string> Tags { get; set; }

        public IRCMessage()
        {
            Params = new List<string>();
            Tags = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Sender: |{0}|", Sender).AppendLine();
            builder.AppendFormat("Command: |{0}|", Command).AppendLine();
            builder.Append("Params:");

            foreach (string par in Params)
            {
                builder.AppendFormat(" |{0}|", par);
            }

            builder.AppendLine();
            builder.Append("Tags:");

            foreach (var keyVal in Tags)
            {
                builder.AppendFormat(" |{0}|=|{1}|", keyVal.Key, keyVal.Value);
            }

            builder.AppendLine();
            builder.AppendFormat("Message: |{0}|", Message).AppendLine();

            return builder.ToString();
        }
    }
}
