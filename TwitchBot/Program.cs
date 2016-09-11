using System;

namespace TwitchBot
{
    class Program
    {
        //Application entry point
        public static void Main(string[] args)
        {
            ScriptLoader loader = ScriptLoader.NewInstance();

            if (!loader.LoadScripts())
            {
                Console.ReadKey();
            }
        }
    }
}
