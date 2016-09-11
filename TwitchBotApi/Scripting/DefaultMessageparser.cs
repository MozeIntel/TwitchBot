using System;
using System.Collections.Generic;
using TwitchBotApi.IRC;
using TwitchBotApi.Utility;

namespace TwitchBotApi.Scripting
{
    /*
     * Default message parser implementation.
     * Used only if the user doesn't provide his own.
     */
    public class DefaultMessageParser : IMessageParser
    {
        public IRCMessage ParseMessage(string message)
        {
            IRCMessage ircMsg = new IRCMessage();
            string str;

            foreach (var split in SplitMessage(message))
            {
                str = split.Item1;

                if (str.StartsWith(":"))
                {
                    if (str.Length == 1)
                    {
                        Logger.Warn("Invalid IRC message: {0}", message);
                        return null;
                    }

                    if (ircMsg.Command == null)
                    {
                        ircMsg.Sender = str.Substring(1);
                    }
                    else
                    {
                        ircMsg.Message = message.Substring(split.Item2 + 1);
                        break;
                    }
                }
                else if (str.StartsWith("@"))
                {
                    if (str.Length == 1)
                    {
                        Logger.Warn("Invalid IRC message: {0}", message);
                        return null;
                    }

                    HandleTags(ircMsg, str.Substring(1));
                }
                else
                {
                    if (ircMsg.Command == null)
                    {
                        ircMsg.Command = str;
                    }
                    else
                    {
                        ircMsg.Params.Add(str);
                    }
                }
            }

            return ircMsg;
        }

        //Split the message, using whitespace as the delimeter
        private IEnumerable<Tuple<string, int>> SplitMessage(string message, char delim = ' ')
        {
            int prev_index = 0;
            int search_index;

            while (true)
            {
                search_index = message.IndexOf(delim, prev_index);

                if (search_index == -1)
                {
                    yield return Tuple.Create(message.Substring(prev_index), prev_index);
                    break;
                }
                else
                {
                    yield return Tuple.Create(message.Substring(prev_index, search_index - prev_index), prev_index);
                    prev_index = search_index + 1;
                }
            }
        }

        private void HandleTags(IRCMessage message, string str)
        {
            string keyVal;
            int index;

            foreach (var split in SplitMessage(str, ';'))
            {
                keyVal = split.Item1;
                index = keyVal.IndexOf('=');

                if (index == -1)
                {
                    Logger.Warn("Invalid tag in IRC message: failed to parse key-value pair for {0}", keyVal);
                    continue;
                }

                try
                {
                    message.Tags.Add(keyVal.Substring(0, index), (index + 1) == keyVal.Length ? "" : keyVal.Substring(index + 1));
                }
                catch (Exception e)
                {
                    Logger.Fatal("Exception handling IRC message tags: {0}", e);
                }
            }
        }

    }
}
