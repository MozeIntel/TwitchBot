using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TwitchBotApi.IRC;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace TwitchBotApi.Network
{
    /// <summary>
    /// Handles network ouptut asynchronously.
    /// </summary>
    internal class SocketWriter : PausableThread
    {
        private StreamWriter writer;
        private BlockingCollection<string> queue;
        private Encoding utf8Encoder;
        private RateLimiter rateLimiter;

        public SocketWriter()
        {
            writer = null;
            queue = null;
            utf8Encoder = new UTF8Encoding(false); //Make sure the encoder doesn't output the BOM char
            rateLimiter = new RateLimiter(ScriptEngine.MainScript.DefaultRateLimit, ScriptEngine.MainScript.DefaultRateLimitInterval);

            IRCEvents.Register("Disconnect", OnDisconect);
        }

        public void OnConnect()
        {
            writer = new StreamWriter(Connection.Socket.GetStream(), utf8Encoder) { AutoFlush = true, NewLine = "\r\n" };
            queue = new BlockingCollection<string>();
            SetRunning();
        }

        private void OnDisconect(EventArgs args)
        {
            if (queue != null)
            {
                queue.CompleteAdding();
                queue.Dispose();
            }

            if (writer != null)
            {
                writer.Dispose();
            }

            SetPaused();
        }

        public void SendMessage(bool rateLimit, string message, params string[] fmt)
        {
            if (rateLimit)
            {
                try
                {
                    queue.Add(string.Format(message, fmt));
                }
                catch (Exception) { }
            }
            else
            {
                BeginSend(message, fmt);
            }
        }

        private void BeginSend(string message, params string[] fmt)
        {
            byte[] data = utf8Encoder.GetBytes(string.Format(message, fmt) + "\r\n");

            try
            {
                Connection.Socket.Client.BeginSend(data, 0, data.Length, SocketFlags.None, EndSend, null);
            }
            catch (Exception e)
            {
                Logger.Fatal("Failed to send network message: {0}", e);
            }
        }

        private void EndSend(IAsyncResult ar)
        {
            try
            {
                Connection.Socket.Client.EndSend(ar);
            }
            catch (Exception e)
            {
                Logger.Fatal("Failed to send network message: {0}", e);
                IRCEvents.Invoke("Disconnect");
            }
        }

        public override void Run()
        {
            string line;

            try
            {
                line = queue.Take();
            }
            catch (Exception e)
            {
                //Disconnect event was raised: thread should be pausing.
                return;
            }

            try
            {
                writer.WriteLine(line);
            }
            catch (Exception e)
            {
                Logger.Fatal("Failed to send network data: {0}", e);
                IRCEvents.Invoke("Disconnect");
            }
        }

    }

}
