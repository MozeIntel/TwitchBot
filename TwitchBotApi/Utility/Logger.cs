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

    //Static class for "fancy" console logging
    public static class Logger
    {
        private static Thread thread;
        private static BlockingCollection<Tuple<object, object[], LogLevel>> queue;

        static Logger()
        {
            Console.OutputEncoding = Encoding.Unicode;

            thread = new Thread(ThreadMethod);
            queue = new BlockingCollection<Tuple<object, object[], LogLevel>>();

            thread.Start();
        }

        private static void ThreadMethod()
        {
            Tuple<object, object[], LogLevel> tuple;
            DateTime time;
            ConsoleColor prevCol;

            while (true)
            {
                tuple = queue.Take();

                prevCol = Console.ForegroundColor;
                Console.ForegroundColor = tuple.Item3.FontColor;

                time = DateTime.Now;
                Console.WriteLine(string.Format("[{0:D2}:{1:D2}:{2:D2}] {3}", time.Hour, time.Minute, time.Second, tuple.Item1), tuple.Item2);

                Console.ForegroundColor = prevCol;
            }
        }


        private static void Log(object obj, LogLevel level, params object[] args)
        {
            queue.Add(Tuple.Create(obj, args, level));
        }

        public static void Info(object obj, params object[] args)
        {
            Log(obj, LogLevel.INFO, args);
        }

        public static void Warn(object obj, params object[] args)
        {
            Log(obj, LogLevel.WARN, args);
        }

        public static void Fatal(object obj, params object[] args)
        {
            Log(obj, LogLevel.FATAL, args);
        }

        public static void Success(object obj, params object[] args)
        {
            Log(obj, LogLevel.SUCCESS, args);
        }
    }
}
