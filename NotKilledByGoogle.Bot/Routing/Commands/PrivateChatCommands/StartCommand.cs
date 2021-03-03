using System;
using System.Text;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing.Commands.PrivateChatCommands
{
    public class StartCommand : CommandRouteBase
    {
        public override string Prefix => "start";

        public override SenderClassification[] AcceptedFrom => new[]
        {
            SenderClassification.Admin,
            SenderClassification.User
        };

        public override async Task ProcessAsync(BotRoutingContext context)
        {
            if (context.IsFromAdmin())
            {
                var sb = new StringBuilder();
                sb.Append("Hello! You're seeing this message because you're recognized as an *administrator*.\n");
                sb.Append($"Bot *version* is `{Program.Version} ({Program.InternalVersion})`\n");
                sb.Append($"*Uptime*: `{DateTime.UtcNow:hh:mm}, up {Program.AppStopwatch.Elapsed:d' day(s) 'hh':'mm}`\n\n");
                sb.Append("*Stats*:\n");
                foreach (var tag in context.Stats.RegisteredSafeInts)
                {
                    sb.Append($"- *{tag.DisplayName}* (`{tag.Id}`): {tag.SafeInt}\n");
                }
                context.RecordGenerationSegmentTime();
                await context.ReplyTextMessageAsync(sb.ToString(), ParseMode.Markdown);
            }
            else
            {
                await context.ReplyTextMessageAsync("Hello! Command features are not developed yet but stay tuned there!");
            }
        }
    }
}
