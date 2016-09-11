using System;
using System.Collections.Generic;
using TwitchBotApi.IRC;
using TwitchBotApi.Network;
using TwitchBotApi.Utility;

namespace TwitchBotApi.Scripting
{
    /// <summary>
    /// Static class for everything script related.
    /// </summary>
    public static class ScriptEngine
    {
        private static List<IScript> loadedScripts = new List<IScript>(); //No more ConcurrentBag :( Gone, but not forgotten.

        private static IMainScript mainScript;
        public static IMainScript MainScript { get { return mainScript; } }

        private static IMessageParser messageParserScript;

        //Mapping is IRC command to object
        private static Dictionary<string, IMessageHandler> messageHandlerMap = new Dictionary<string, IMessageHandler>();


        /// <summary>
        /// Register a loaded script for processing
        /// </summary>
        /// <param name="script"> The script to register </param>
        public static void RegisterScript(IScript script)
        {
            loadedScripts.Add(script);
        }

        /// <summary>
        /// Process all registered scripts.
        /// </summary>
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

            if (messageParserScript == null)
            {
                Logger.Info("User didn't provide custom IMessageParser, using default");
                messageParserScript = new DefaultMessageParser();
            }

            Logger.Success("Processed {0} scripts ({1} succedeed, {2} failed)", loadedScripts.Count, success, fail);
        }

        /// <summary>
        /// Some scripts are necessary for everything to work properly
        /// </summary>
        /// <returns>If the engine can start properly</returns>
        public static bool HasNecessaryScripts()
        {
            return mainScript != null;
        }

        /// <summary>
        /// Perform some last checks, and start the main script
        /// </summary>
        public static bool Start()
        {
            if (!HasNecessaryScripts())
            {
                Logger.Fatal("Scripting engine cannot start: required scripts are missing!");
                Logger.Fatal("Required interfaces: IMainScript, IMessageParser");
                return false;
            }

            //TODO: Move somewhere else?
            Connection.Open();

            Logger.Info("ScriptEngine started");
            mainScript.Main();
            return true;
        }

        /// <summary>
        /// Parses a raw network IRC message into an <see cref="TwitchBotApi.IRC.IRCMessage"/> object, 
        /// using the registered <see cref="TwitchBotApi.Scripting.IMessageParser"/>
        /// </summary>
        /// <param name="message">The message to parse</param>
        /// <returns>The <see cref="TwitchBotApi.IRC.IRCMessage"/> object, or null if the <see cref="TwitchBotApi.Scripting.IMessageParser"/> failed</returns>
        public static IRCMessage ParseIrcMessage(string message)
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

        /// <summary>
        /// <para> Handles an <see cref="TwitchBotApi.IRC.IRCMessage"/>, with the appropriate <see cref="TwitchBotApi.Scripting.IMessageHandler"/> </para>
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns> Whether a matching <see cref="TwitchBotApi.Scripting.IMessageHandler"/> was found </returns>
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
