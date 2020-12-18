using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Config;
using NotKilledByGoogle.Bot.Grave;
using NotKilledByGoogle.Bot.Grave.Helpers;
using Telegram.Bot;
using static NotKilledByGoogle.Bot.ConsoleHelper;

namespace NotKilledByGoogle.Bot
{
    internal class Program
    {
        private const string Version = "0.1.4a";
        private const int DeathAnnouncerInterval = 300000;
        private static readonly int[] AnnounceBeforeDays = { 0, 1, 2, 3, 7, 30, 90, 180 };
        private static readonly Stopwatch AppStopwatch = new();
        private static readonly JsonConfigManager<BotConfig> ConfigManager = new()
        {
            FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "config.json")
        };
        private static readonly CancellationTokenSource TokenSource = new();
        
        private static GraveKeeper _keeper = null!;
        private static TelegramBotClient _bot = null!;
        private static int _cancelCounter = 3;

        private static async void OnKeyboardInterrupt(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _cancelCounter--;
            if (_cancelCounter <= 0) Environment.Exit(1);
            Info($"Press {_cancelCounter} more times to force exit.");
            if (_cancelCounter != 2) return;
            // first cancel
            Info("Attempting to save configurations...");
            await ConfigManager.SaveConfigAsync();
            Info("Cancelling running tasks...");
            TokenSource.Cancel();
        }

        private static async Task DeathAnnouncer()
        {
            // wait until GraveKeeper tells it that data is ready
            var tcs = new TaskCompletionSource();
            GraveKeeper.FetchedEventHandler oneTimeHandler = (_, _) => tcs.SetResult();
            _keeper.Fetched += oneTimeHandler;
            await tcs.Task;
            _keeper.Fetched -= oneTimeHandler;
            // ok it's ready now, cache it
            var graveyard = _keeper.Gravestones;
            Info("Scheduling death announcements...");
            // TODO: add death announcement scheduling
            Info("Death announcer is ready.");
            
            while (!TokenSource.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(DeathAnnouncerInterval, TokenSource.Token);
                    // get latest graveyard status
                    var newGraveyard = _keeper.Gravestones;

                    // check if anything is "added"
                    if (newGraveyard.Except(graveyard, new GravestoneEqualityComparer()).Any())
                    {
                        // TODO: add "new project (to be) killed by Google" broadcast
                        goto announcerCycleDone;
                    }

                    // check if anything is "removed"
                    if (graveyard.Except(newGraveyard, new GravestoneEqualityComparer()).Any())
                    {
                        // TODO: add "project revived by Google" broadcast
                    }
                    
announcerCycleDone:
                    // update the cached graveyard
                    graveyard = newGraveyard;
                }
                catch (TaskCanceledException) {}
                catch (Exception ex)
                {
                    Warning("Death announcer encountered something wrong: " + ex);
                }
            }
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
                
                Info("Preparing graveyard keeper...");
                _keeper = new GraveKeeper(ConfigManager.Config.GraveyardJsonLocation);
                _keeper.FetchError += OnFetchError;
                _keeper.Fetched += OnFetched;
                _keeper.Start();
                
                Info("Preparing death announcer...");
                _ = Task.Run(DeathAnnouncer);

                Info($"Startup complete ({AppStopwatch.Elapsed:g}), main thread entering standby state...");
                while (!TokenSource.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(5000, TokenSource.Token);
                    }
                    // ignore that TaskCanceledException: we're terminating everything
                    catch (TaskCanceledException) {}
                    catch (Exception ex)
                    {
                        Error("Something happened in main thread: " + ex);
                    }
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
