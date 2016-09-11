using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TwitchBotApi.IRC;
using TwitchBotApi.Utility;

namespace TwitchBotApi.Scripting
{
    public static class ScriptEngine
    {
        //Script loading is parallel, so we need a thread-safe collection
        private static ConcurrentBag<IScript> loadedScripts = new ConcurrentBag<IScript>(); //ConcurrentBag makes me laugh every time. 

        private static IMainScript mainScript;
        public static IMainScript MainScript { get { return mainScript; } }

        private static IMessageParser messageParserScript;

        //Mapping is IRC command to object
        private static Dictionary<string, IMessageHandler> messageHandlerMap = new Dictionary<string, IMessageHandler>();


        //Register a loaded script in the engine
        public static void RegisterScript(IScript script)
        {
            loadedScripts.Add(script);
        }

        //Sort all loaded script based on type
        public static void ProcessScripts()
        {
            Logger.Info("Processing scripts...");

            uint success = 0;
            uint fail = 0;

            //TODO: implement a Register() function in the IScripts, this isn't very efficient.
            foreach (var script in loadedScripts)
            {
                if (CheckSingleInstanceScript(ref mainScript, script))
                {
                    ++success;
                }
                else if (CheckSingleInstanceScript(ref messageParserScript, script))
                {
                    ++success;
                }
                else if (script is IMessageHandler)
                {
                    IMessageHandler messageHandler = (IMessageHandler)script;

                    try
                    {
                        messageHandlerMap.Add(messageHandler.Command.ToUpper(), messageHandler);
                        ++success;
                    }
                    catch (ArgumentException)
                    {
                        Logger.Warn("Duplicate message handler for {0} found in {1}", messageHandler.Command.ToUpper(), typeof(IMessageHandler));
                    }
                }
                else
                {
                    Logger.Warn("Unrecognized IScript found: {0}", script);
                    ++fail;
                }
            }

            Logger.Success("Processed {0} scripts ({1} succedeed, {2} failed)", loadedScripts.Count, success, fail);
        }

        //Some scripts are required for the engine to run properly
        public static bool HasNecessaryScripts()
        {
            return mainScript != null && messageParserScript != null;
        }

        //Start the engine!
        public static bool Start()
        {
            if (!HasNecessaryScripts())
            {
                Logger.Fatal("Scripting engine cannot start: required scripts are missing!");
                Logger.Fatal("Required interfaces: IMainScript, IMessageParser");
                return false;
            }

            Logger.Info("ScriptEngine started");
            mainScript.Main();
            return true;
        }

        //Parse a network IRC message. May return null if the script fails.
        public static IRCMessage ProcessIrcMessage(string message)
        {
            try
            {
                return messageParserScript.ParseMessage(message);
            }
            catch (Exception e)
            {
                Logger.Warn("Failed to parse IRC Message ({0}): {1}", message, e);
                return null;
            }
        }

        /*
         * Check if there is a IMessageHandler for the IRC message, and handle it. 
         * Returns  false if no IMessageHandler was registered for that command.
         */
        public static bool HandleIrcMessage(IRCMessage message)
        {
            IMessageHandler handler;

            if (messageHandlerMap.TryGetValue(message.Command, out handler))
            {
                handler.Handle(message);
                return true;
            }

            return false;
        }

        private static bool CheckSingleInstanceScript<T>(ref T scriptRef, IScript newScript)
        {
            if (!(newScript is T))
            {
                return false;
            }

            if (scriptRef != null)
            {
                Logger.Warn("Warning: duplicate {0} found in {1}", typeof(T), newScript);
                return true;
            }

            scriptRef = (T)newScript;
            return true;
        }
    }
}
