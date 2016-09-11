using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TwitchBotApi.Scripting;
using TwitchBotApi.Utility;

namespace TwitchBot
{
    /// <summary>
    /// Helper class for loading all <see cref="TwitchBotApi.Scripting.IScript"/> objects from files.
    /// </summary>
    /// <remarks>
    /// All scripts will be loaded in a seperate <see cref="System.AppDomain"/>, to allow the to be unloaded.
    /// </remarks>
    public class ScriptLoader : MarshalByRefObject
    {
        public const string SCRIPT_DIR = "Scripts";
        public const string APPDOMAIN_NAME = "TwitchBot Script Engine";

        public AppDomain OldDomain { get; private set; }
        public AppDomain NewDomain { get; private set; }

        /// <summary>
        /// Create a new instance of the <see cref="ScriptLoader"/> in the seperate <see cref="System.AppDomain"/>.
        /// </summary>
        /// <returns>The the proxied <see cref="ScriptLoader"/></returns>
        public static ScriptLoader NewInstance()
        {
            AppDomain oldDomain = AppDomain.CurrentDomain;
            AppDomain newDomain = AppDomain.CreateDomain(APPDOMAIN_NAME, oldDomain.Evidence, oldDomain.SetupInformation);
            Type type = typeof(ScriptLoader);
            return (ScriptLoader)newDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, false, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { oldDomain, newDomain }, null, null);
        }

        private ScriptLoader(AppDomain oldDomain, AppDomain newDomain)
        {
            OldDomain = oldDomain;
            NewDomain = newDomain;
        }

        /// <summary>
        /// Find and load all scripts.
        /// </summary>
        /// <returns>If scripts could be loaded, or some exception occured.</returns>
        public bool LoadScripts()
        {
            Logger.Info("Loading scripts...");

            CompilerResults results;

            using (var compiler = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } }))
            {
                results = CompileScripts(compiler, FindScripts());
            }

            if (results == null)
            {
                return false;
            }

            SearchAssembly(results.CompiledAssembly);

            ScriptEngine.ProcessScripts();
            return ScriptEngine.Start();
        }

        //Add all needed assembly references for the compiler parameters
        private CompilerParameters BuildCompilerParams()
        {
            CompilerParameters compilerParams = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true };

            #if DEBUG
            compilerParams.IncludeDebugInformation = true;
            #endif

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    compilerParams.ReferencedAssemblies.Add(assembly.Location);
                }
                catch (NotSupportedException)
                {
                }
            }

            return compilerParams;
        }

        //Finds all .cs files in the script dir. Returns an emtpy array if an IO exception occurs
        private string[] FindScripts()
        {
            try
            {
                return Directory.GetFileSystemEntries(SCRIPT_DIR, "*.cs", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Logger.Fatal("Failed to load scripts: {0}", e);
                return new string[] { };
            }
        }

        //Compiles all scripts, and prints out any compilation errors
        private CompilerResults CompileScripts(CSharpCodeProvider compiler, string[] scripts)
        {
            CompilerResults result;

            try
            {
                result = compiler.CompileAssemblyFromFile(BuildCompilerParams(), scripts);
            }
            catch (NotImplementedException e)
            {
                Logger.Fatal("Failed to compile scripts: {0}", e);
                return null;
            }

            Logger.Success("Scripts compiled ({0} total, {1} errors)", scripts.Length, result.Errors.Count);

            foreach (var error in result.Errors)
            {
                Logger.Warn("Compilation error: {0}", error);
            }

            return result;
        }

        //Searches for any script objects in the compiled assembly
        private void SearchAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.DefinedTypes)
            {
                if (typeof(IScript).IsAssignableFrom(type))
                {
                    ScriptEngine.RegisterScript((IScript)Activator.CreateInstance(type));
                }
            }
        }

    }
}
