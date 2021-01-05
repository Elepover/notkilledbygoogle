using System;
using NotKilledByGoogle.Bot.Grave;

namespace NotKilledByGoogle.Bot
{
    /// <summary>
    /// Provides message formatting for bot logics.
    /// </summary>
    public class MessageFormatter
    {
        // {0}: app/service/hardware, {1}: product name, {2}: time left (in 2 days, tomorrow, etc)
        public const string KillingByGoogle = "🔪 {0}: *{1}* is going to be killed by Google `{2}`.";
        // {0}: app/service/hardware, {1}: product name, {2}: description of this product
        public const string KilledByGoogle = "⚰️ {0}: *{1}* is now officially killed by Google.\n\nIt was: {2}";
        // {0}: app/service/hardware, {1}: product name, {2}: description of this product
        public const string NewProductMurdered = "⚠️ yet another {0} is going to be killed by Google: *{1}*\n\n_{2}_";
        // {0}: app/service/hardware, {1}: product name, {2}: description of this product
        public const string ProductSaved = "🎉 a {0} was surprisingly saved from Google: *{1}*\n\n_{2}_";
        
        public static string DeceasedTypeName(DeceasedType deceasedType) => deceasedType switch
        {
            DeceasedType.App => "app",
            DeceasedType.Hardware => "hardware",
            DeceasedType.Service => "service",
            _ => "product"
        };

        public static string FormatTimeLeft(TimeSpan left)
        {
            if (left < TimeSpan.FromHours(2)) return "very soon";
            if (left <= TimeSpan.FromDays(1)) return "tomorrow";
            var leftDays = left.TotalDays;
            double result;
            if (Math.Abs(Math.Truncate(leftDays) - leftDays) < double.Epsilon)
                result = leftDays;
            else
                result = Math.Truncate(leftDays) + 1;
            return $"in {result} days";
        }
    }
}
