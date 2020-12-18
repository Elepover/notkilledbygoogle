using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Config;
using static NotKilledByGoogle.Bot.ConsoleHelper;

namespace NotKilledByGoogle.Bot
{
    internal class Program
    {
        private static readonly string Version = "0.1.4a";
        private static readonly JsonConfigManager<BotConfig> ConfigManager = new() { FilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "config.json") };
        
        public static async Task Main(string[] args)
        {
            Info("Starting ᴺᴼᵀKilled by Google bot, version " + Version);
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                Info("Loading configurations from: " + ConfigManager.FilePath);
                if (!await ConfigManager.ValidateConfigAsync())
                {
                    Warning("Unable to load configurations, creating new file...");
                    if (ConfigManager.Backup()) Warning($"Backed up to: {ConfigManager.FilePath}.bak");
                    await ConfigManager.CreateConfigAsync();
                    Warning("You need to modify configurations before you can get the bot up & running.");
                    Environment.Exit(1);
                }
                await ConfigManager.LoadConfigAsync();
                Info("Configurations loaded.");
                
            }
            catch (Exception ex)
            {
                Error("Startup failed: " + ex);
            }
            
        }
    }
}