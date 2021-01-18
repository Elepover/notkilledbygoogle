using System;
using NotKilledByGoogle.Bot.Grave;

namespace NotKilledByGoogle.Bot
{
    /// <summary>
    /// Provides message formatting for bot logics.
    /// </summary>
    public class MessageFormatter
    {
        /*  PLEASE NOTE THAT STRINGS ARE IN MarkdownV2 FORMAT AND THESE CHARACTERS SHOULD BE ESCAPED:
         *  _, *, [, ], (, ), ~, `, >, #, +, -, =, |, {, }, ., !
         *
         * These are NOT automatically escaped: '*', '`', '_', '~', '[', ']', '(', ')',
         *     escape them manually if you're NOT using Markdown.
         */
        // {0}: app/service/hardware, {1}: product name, {2}: time left (in 2 days, tomorrow, etc)
        public const string KillingByGoogle = "🔪 The {0}: *{1}* is going to be killed by Google `{2}`.";
        // {0}: app/service/hardware, {1}: product name, {2}: description of this product
        public const string KilledByGoogle = "⚰️ The {0}: *{1}* is now officially killed by Google.\n\n_{2}_";
        // {0}: app/service/hardware, {1}: product name, {2}: description of this product
        public const string NewProductMurdered = "⚠️ Yet another {0} is going to be killed by Google: *{1}*\n\n_{2}_";
        // {0}: app/service/hardware, {1}: product name, {2}: description of this product
        public const string ProductExempted = "🎉 A {0} was surprisingly exempted by Google: *{1}*\n\n_{2}_";
        
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
