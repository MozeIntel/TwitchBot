using System;
using System.IO;
using System.Text;
using System.Threading;
using TwitchBotApi.IRC;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace TwitchBotApi.Network
{
    /*
     * Class for thread safe network input
     */
    internal class SocketReader : PausableThread
    {
        private StreamReader reader;
        private Encoding utf8Encoder;

        public SocketReader()
        {
            reader = null;
            utf8Encoder = new UTF8Encoding(false); //No BOM char
            IRCEvents.Register("Disconnect", OnDisconnect);   
        }

        public void OnConnect()
        {
            reader = new StreamReader(Connection.Socket.GetStream(), utf8Encoder);
            SetRunning();
        }

        private void OnDisconnect(EventArgs args)
        {
            reader.Dispose();
            SetPaused();
        }

        public override void Run()
        {
            string line;

            try
            {
                line = reader.ReadLine();
            }
            catch (Exception e)
            {
                Logger.Fatal("Failed to read network data: {0}", e);
                IRCEvents.Invoke("Disconnect");
                return;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                Logger.Fatal("Failed to read network data: end of stream");
                IRCEvents.Invoke("Disconnect");
                return;
            }

            ThreadPool.QueueUserWorkItem(WorkItemCallback, line);
        }

        /*
         * Callback for the thread pool work job. The state is the line to process
         */ 
        private void WorkItemCallback(object state)
        {
            IRCMessage message = ScriptEngine.ProcessIrcMessage(state as string);

            if (message != null)
            {
                try
                {
                    if (!ScriptEngine.HandleIrcMessage(message))
                    {
                        Logger.Warn("Unhandled IRC message: {0}", state);
                    }
                }
                catch (Exception e)
                {
                    Logger.Fatal("Message handler script exception: {0}", e);
                    Logger.Fatal("For message: {0}", state);
                    Logger.Fatal("StackTrace: {0}", e.StackTrace);
                }
            }
            else
            {
                Logger.Warn("Failed to parse IRC message: {0}", state);
            }
        }

    }
}
