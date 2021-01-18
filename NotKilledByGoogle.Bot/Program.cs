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
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using static NotKilledByGoogle.Bot.ConsoleHelper;

namespace NotKilledByGoogle.Bot
{
    internal static class Program
    {
        #region Compile-time configurations
        private const string Version = "0.1.17a";
        private const int DeathAnnouncerInterval = 300000;
        private static readonly int[] AnnounceBeforeDays = { 0, 1, 2, 3, 7, 30, 90, 180 };
        #endregion

        #region Runtime "global" variables
        private static readonly Stopwatch AppStopwatch = new();
        private static readonly IConfigManager<BotConfig> ConfigManager = new JsonConfigManager<BotConfig>()
        {
            FilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "config.json")
        };
        private static readonly CancellationTokenSource MainCancellationTokenSource = new();

        private static GraveKeeper _keeper = null!;
        private static AnnouncementScheduler _scheduler = null!;
        private static TelegramBotClient _bot = null!;
        private static int _cancelCounter = 3;
        #endregion

        #region Event handlers
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
            _keeper?.Stop();
            // no need to stop bot receiving: we started receiving with CancellationToken
            MainCancellationTokenSource.Cancel();
        }
        
        private static void OnFetchError(object? sender, FetchErrorEventArgs e)
            => Error($"Failed to fetch graveyard data from {e.FailedUrl} (last successful fetch at {e.LastSuccessfulFetch:R}): {e.Exception}");

        private static void OnFetched(object? sender, FetchedEventArgs e)
            => Info("Fetched graveyard data from " + e.FetchUrl);

        private static async void OnAnnouncement(object? sender, AnnouncementEventArgs e)
        {
            Info($"Incoming announcement for {e.Gravestone.DeceasedType.ToString().ToLowerInvariant()} {e.Gravestone.Name}.");
            try
            {
                await AnnounceAsync((e.Gravestone.DateClose - DateTimeOffset.Now < TimeSpan.FromDays(1)) ? AnnouncementType.Killed : AnnouncementType.Killing, e.Gravestone);
            }
            catch (Exception ex)
            {
                Error("Unable to make announcement: " + ex);   
            }
        }

        private static void OnUpdate(object? sender, UpdateEventArgs e)
        {
            try
            {
                if (e.Update.Type == UpdateType.ChannelPost)
                {
                    Info($"Received channel post from channel {e.Update.ChannelPost.Chat.Title} (ID: {e.Update.ChannelPost.Chat.Id})");
                }
            }
            catch (Exception ex)
            {
                Error("Error processing update: " + ex);
            }
        }
        #endregion

        #region Thread and task logics
        /// <summary>
        /// Death announcer's logic. DON'T USE IT AS AN API.
        /// </summary>
        /// <returns></returns>
        private static async Task DeathAnnouncer()
        {
            // STAGE 1: SETUP
            
            // wait until GraveKeeper tells it that data is ready
            var tcs = new TaskCompletionSource();
            var sw = new Stopwatch();
            GraveKeeper.FetchedEventHandler oneTimeHandler = (_, _) => tcs.SetResult();
            _keeper.Fetched += oneTimeHandler;
            sw.Start();
            _keeper.Start();
            Info("Awaiting graveyard data...");
            await tcs.Task;
            sw.Stop();
            _keeper.Fetched -= oneTimeHandler;
            
            // ok it's ready now, cache it as initial graveyard
            var graveyard = _keeper.Gravestones;
            var skipped = 0;
            Info($"Graveyard data fetched in {sw.Elapsed.TotalSeconds:F2}s, scheduling death announcements...");
            
            // make initial schedules
            foreach (var gravestone in graveyard)
            {
                if (gravestone.DateClose <= DateTimeOffset.Now)
                {
                    skipped++;
                    continue;
                }
                var scheduledAnnouncementCount = await _scheduler.ScheduleAsync(gravestone, new AnnouncementOptions(AnnounceBeforeDays));
                Info($"Scheduled announcements for {gravestone.DeceasedType.ToString().ToLowerInvariant()} {gravestone.Name}:");
                foreach (var announcementDate in _scheduler.GetAnnouncementDates(gravestone))
                {
                    Info($"{new string(' ', 4)} {announcementDate:R}");
                }
            }
            
            // ready, enter next stage
            Info($"Death announcer is ready, {_scheduler.ScheduledCount} scheduled. (RIP for the {skipped} already dead products)");

            // STAGE 2: UPDATE LOOP
            var token = MainCancellationTokenSource.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(DeathAnnouncerInterval, token);
                    // get latest graveyard status
                    var newGraveyard = _keeper.Gravestones;

                    // check if anything is "added"
                    var added = newGraveyard.Except(graveyard, new GravestoneEqualityComparer()).ToList();
                    if (added.Any())
                    {
                        foreach (var gravestone in added)
                        {
                            await _scheduler.ScheduleAsync(gravestone, new AnnouncementOptions(AnnounceBeforeDays));
                            Info($"New product joined the Being Alive Club: {gravestone.DeceasedType.ToString().ToLowerInvariant()} {gravestone.Name}.");
                            await AnnounceAsync(AnnouncementType.NewVictim, gravestone);
                        }
                        goto announcerCycleDone;
                    }

                    // check if anything is "removed"
                    var removed = graveyard.Except(newGraveyard, new GravestoneEqualityComparer()).ToList();
                    if (removed.Any())
                    {
                        foreach (var gravestone in removed)
                        {
                            _scheduler.Cancel(gravestone, true);
                            Info($"HOLY SHIT a project was SAVED by Google! It was {gravestone.DeceasedType.ToString().ToLowerInvariant()} {gravestone.Name}");
                            await AnnounceAsync(AnnouncementType.ProductExempted, gravestone);
                        }
                    }
                    
announcerCycleDone:
                    // finalize: update the cached graveyard
                    graveyard = newGraveyard;
                    Info($"Graveyard updated, {_scheduler.ScheduledCount} announcements pending.");
                }
                catch (TaskCanceledException) {}
                catch (Exception ex)
                {
                    Warning("Death announcer encountered something wrong: " + ex);
                }
            }
        }
        #endregion

        #region Private APIs or "snippets"
        /// <summary>
        /// Send a message through Telegram bot asynchronously.
        /// </summary>
        /// <param name="message">Message body, encoded with <see cref="ParseMode.MarkdownV2"/>.</param>
        /// <returns></returns>
        private static async Task SendMessageAsync(string message)
        {
            foreach (var id in ConfigManager.Config.BroadcastId)
            {
                Info($"Sending message to chat {id}...");
                await _bot.SendTextMessageAsync(new(id), Utils.EscapeIllegalMarkdownV2Chars(message), ParseMode.MarkdownV2, true);
            }
        }

        /// <summary>
        /// Make an announcement through Telegram bot asynchronously.
        /// </summary>
        /// <param name="type">Announcement type.</param>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <returns></returns>
        private static async Task AnnounceAsync(AnnouncementType type, Gravestone gravestone)
        {
            switch (type)
            {
                case AnnouncementType.Killed:
                    await SendMessageAsync(
                        string.Format(MessageFormatter.KilledByGoogle,
                                      MessageFormatter.DeceasedTypeName(gravestone.DeceasedType),
                                      gravestone.Name,
                                      gravestone.Description));
                    break;
                case AnnouncementType.Killing:
                    await SendMessageAsync(
                        string.Format(MessageFormatter.KillingByGoogle,
                                      MessageFormatter.DeceasedTypeName(gravestone.DeceasedType),
                                      gravestone.Name,
                                      MessageFormatter.FormatTimeLeft(gravestone.DateClose - DateTimeOffset.Now)));
                    break;
                case AnnouncementType.NewVictim:
                    await SendMessageAsync(
                        string.Format(MessageFormatter.NewProductMurdered,
                                      MessageFormatter.DeceasedTypeName(gravestone.DeceasedType),
                                      gravestone.Name,
                                      gravestone.Description
                        ));
                    break;
                case AnnouncementType.ProductExempted:
                    await SendMessageAsync(
                        string.Format(MessageFormatter.ProductExempted,
                                      MessageFormatter.DeceasedTypeName(gravestone.DeceasedType),
                                      gravestone.Name,
                                      gravestone.Description
                        ));
                    break;
                default:
                    Warning($"Unknown announcement type passed to bot: {type}, gravestone name: {gravestone.Name}, ditched.");
                    break;
            }
        }
        #endregion
        
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
                if (!Debugger.IsAttached)
                    _bot.TestApiAsync().Wait(MainCancellationTokenSource.Token);
                else
                    Info("Skipped: in debug mode.");
                
                Info("Preparing graveyard keeper...");
                _keeper = new (ConfigManager.Config.GraveyardJsonLocation);
                _keeper.FetchError += OnFetchError;
                _keeper.Fetched += OnFetched;
                // NOT starting keeper just yet, let the announcer do it.
                
                Info("Preparing death announcer...");
                _scheduler = new();
                _scheduler.Announcement += OnAnnouncement;
                _ = Task.Run(DeathAnnouncer);
                
                Info("Starting update receiving...");
                _bot.OnUpdate += OnUpdate;
                _bot.StartReceiving(new [] {UpdateType.ChannelPost}, MainCancellationTokenSource.Token);

                Info($"Startup complete in {AppStopwatch.Elapsed.TotalSeconds:F2}s, main thread entering standby state...");
                var token = MainCancellationTokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(Timeout.Infinite, token);
                    }
                    // ignore that TaskCanceledException: we're terminating everything
                    catch (OperationCanceledException) {}
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
