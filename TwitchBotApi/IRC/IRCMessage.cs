using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchBotApi.IRC
{
    /// <summary>
    /// Object that represents a parsed IRC message.
    /// </summary>
    public class IRCMessage : EventArgs
    {
        /// <summary>The IRC command, identifies this message.</summary>
        public string Command { get; set; }

        /// <summary>Any extra parameters for the command.</summary>
        public List<string> Params { get; set; }

        public string Sender { get; set; }

        /// <summary>The actual message contents.</summary>
        public string Message { get; set; }

        /// <summary>Key-Value mapped tags for the command.</summary>
        public Dictionary<string, string> Tags { get; set; }

        public IRCMessage()
        {
            Params = new List<string>();
            Tags = new Dictionary<string, string>();
        }

        //Debug code
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
