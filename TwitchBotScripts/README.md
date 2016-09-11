# TwitchBotScripts

Contains all the scripts that will be compiled at runtime by the application.

To get the scripts to load, they must be placed in a "Script" directory in the app's .exe folder. 
An easy way to do this is to create a symlink for the folder, so you don't have to manually copy it.

## Notes:
- All scripts well be dynamically loaded in a seperate appdomain, that has a reference to all standard .net assemblies 
  and the TwitchBotApi
- Scripts can't access code from the main TwitchBot module, only the API
- For a script to load, it must inherit from IScript. Check [the API](https://github.com/MozeIntel/TwitchBot/tree/master/TwitchBotApi/Scripting) for available interfaces
- Scripts can be placed inside any number of sub-directories: just make sure the top directory is called "Scripts"
- Scripts will be executed in a muti-threaded enviroment: thread safety is a concern
- Scripts can be debugged in Visual Studio: make sure you have all the modules in the solution
