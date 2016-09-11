using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace TwitchBotApi.Utility
{
    public class LogLevel
    {
        public static readonly LogLevel INFO = new LogLevel { FontColor = ConsoleColor.White };
        public static readonly LogLevel WARN = new LogLevel { FontColor = ConsoleColor.Yellow };
        public static readonly LogLevel FATAL = new LogLevel { FontColor = ConsoleColor.Red };
        public static readonly LogLevel SUCCESS = new LogLevel { FontColor = ConsoleColor.Green };

        public ConsoleColor FontColor { get; private set; }

        private LogLevel() { }
    }

    /// <summary>
    /// Console logging in 2k16 LUL
    /// </summary>
    public static class Logger
    {
        private static Thread thread;
        private static BlockingCollection<Tuple<string, LogLevel>> queue;

        static Logger()
        {
            Console.OutputEncoding = Encoding.Unicode;

            thread = new Thread(ThreadMethod);
            queue = new BlockingCollection<Tuple<string, LogLevel>>();

            thread.Start();
        }

        private static void ThreadMethod()
        {
            Tuple<string, LogLevel> tuple;
            DateTime time;
            ConsoleColor prevCol;

            while (true)
            {
                tuple = queue.Take();

                prevCol = Console.ForegroundColor;
                Console.ForegroundColor = tuple.Item2.FontColor;

                time = DateTime.Now;
                Console.WriteLine("[{0:D2}:{1:D2}:{2:D2}] {3}", time.Hour, time.Minute, time.Second, tuple.Item1);

                Console.ForegroundColor = prevCol;
            }
        }


        private static void Log(LogLevel level, object obj, params object[] args)
        {
            queue.Add(Tuple.Create(string.Format(obj.ToString(), args), level));
        }

        public static void Info(object obj, params object[] args)
        {
            Log(LogLevel.INFO, obj, args);
        }

        public static void Warn(object obj, params object[] args)
        {
            Log(LogLevel.WARN, obj, args);
        }

        public static void Fatal(object obj, params object[] args)
        {
            Log(LogLevel.FATAL, obj, args);
        }

        public static void Success(object obj, params object[] args)
        {
            Log(LogLevel.SUCCESS, obj, args);
        }
    }
}
