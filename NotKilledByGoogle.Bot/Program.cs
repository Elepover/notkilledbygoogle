using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Config;
using NotKilledByGoogle.Bot.Grave;
using Telegram.Bot;
using static NotKilledByGoogle.Bot.ConsoleHelper;

namespace NotKilledByGoogle.Bot
{
    internal class Program
    {
        private static readonly string Version = "0.1.4a";
        private static readonly Stopwatch AppStopwatch = new();
        private static readonly JsonConfigManager<BotConfig> ConfigManager = new()
        {
            FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "config.json")
        };
        private static readonly CancellationTokenSource TokenSource = new();
        
        private static GraveKeeper _keeper = null!;
        private static TelegramBotClient _bot = null!;
        private static int _cancelCounter = 3;

        private static void OnKeyboardInterrupt(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cancelCounter--;
            if (_cancelCounter <= 0) Environment.Exit(1);
            Info($"Stopping... Press {_cancelCounter} more times to force exit.");
            TokenSource.Cancel();
        }
        
        private static void OnFetchError(object? sender, FetchErrorEventArgs e)
            => Error($"Failed to fetch graveyard data from {e.FailedUrl} (last successful fetch at {e.LastSuccessfulFetch:R}): {e.Exception}");

        private static void OnFetched(object? sender, FetchedEventArgs e)
            => Info("Fetched graveyard data from " + e.FetchUrl);
        
        public static async Task Main(string[] args)
        {
            AppStopwatch.Start();
            Info("Starting ᴺᴼᵀKilled by Google bot, version " + Version);
            try
            {
                Info("Arming event handler...");
                Console.CancelKeyPress += OnKeyboardInterrupt;
                
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
                _ = Utils.ThrowIfNull(ConfigManager.Config);
                Info("Configurations loaded.");
                
                Info("Preparing Telegram bot...");
                _bot = new(ConfigManager.Config.ApiKey);
                await _bot.TestApiAsync();
                
                Info("Preparing Graveyard keeper...");
                _keeper = new GraveKeeper(ConfigManager.Config.GraveyardJsonLocation) { UpdateInterval = 5000 };
                _keeper.FetchError += OnFetchError;
                _keeper.Fetched += OnFetched;
                _keeper.Start();

                Info($"Startup complete ({AppStopwatch.Elapsed:g}), main thread entering standby state...");
                while (!TokenSource.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(5000, TokenSource.Token);
                    }
                    catch {}
                }

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Error($"Startup failed after {AppStopwatch.Elapsed:c}: " + ex);
            }
        }
    }
}
