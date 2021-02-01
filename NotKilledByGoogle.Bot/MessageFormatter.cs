using System;
using NotKilledByGoogle.Bot.Grave;

namespace NotKilledByGoogle.Bot
{
    /// <summary>
    /// Provides message formatting for bot logics.
    /// </summary>
    public static class MessageFormatter
    {
        public static readonly string[] MonthNames =
        {
            "January", "February", "March",
            "April", "May", "June",
            "July", "August", "September",
            "October", "November", "December"
        };
        /*  PLEASE NOTE THAT STRINGS ARE IN MarkdownV2 FORMAT AND THESE CHARACTERS SHOULD BE ESCAPED:
         *  _, *, [, ], (, ), ~, `, >, #, +, -, =, |, {, }, ., !
         *
         * These are NOT automatically escaped: '*', '`', '_', '~', '[', ']', '(', ')',
         *     escape them manually if you're NOT using Markdown.
         */
        // {0}: app/service/hardware, {1}: product name, {2}: time left (in 2 days, tomorrow, etc)
        public const string KillingByGoogle = "🔪 The {0}: *{1}* is going to be killed by Google `{2}`.";
        // {0}: app/service/hardware, {1}: product name, {2}: description of this product, {3}: reference link, {4}: age
        public const string KilledByGoogle = "⚰️ The {0}: *{1}* is now officially killed by Google. It's {4} old upon death.\n\n_{2}_\n\n[reference]({3})";
        // {0}: app/service/hardware, {1}: product name, {2}: death date, {3}: description of this product, {4}: reference link
        public const string NewProductMurdered = "⚠️ Yet another {0} is going to be killed by Google: *{1}*\n\n*Would be dead on:* {2}\n\n_{3}_\n\n[reference]({4})";
        // {0}: app/service/hardware, {1}: days before death, {2}: product name, {3}: description of this product
        public const string ProductExempted = "🎉 A {0} was surprisingly exempted by Google {1} days before its death: *{2}*\n\n_{3}_";
        // {0}: month, {1}: products left to be killed
        public const string NewMonthWithProductsToBeKilled = "🗓 Welcome to {0}, there are still {1} products on their way to the heaven.\n\nThe nearest death is the {2} *{3}*, which's gonna be killed on {4}.";
        // {0}: month
        public const string NewMonthButNoProductsGonnaBeKilled = "🗓 Welcome to {0}. _No upcoming product funerals, yet._";
        
        /// <summary>
        /// Get formatted message for corresponding announcement.
        /// </summary>
        /// <param name="type">Announcement type.</param>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> is not a valid type.</exception>
        public static string FormatMessage(AnnouncementType type, Gravestone gravestone)
            => type switch
            {
                AnnouncementType.Killed => string.Format(KilledByGoogle, 
                                                         DeceasedTypeName(gravestone.DeceasedType),
                                                         gravestone.Name,
                                                         gravestone.Description,
                                                         gravestone.ReferenceLink,
                                                         FormatAge(gravestone.DateClose - gravestone.DateOpen)),
                AnnouncementType.Killing => string.Format(KillingByGoogle,
                                                          DeceasedTypeName(gravestone.DeceasedType),
                                                          gravestone.Name,
                                                          FormatTimeLeft(gravestone.DateClose - DateTimeOffset.Now)),
                AnnouncementType.NewVictim => string.Format(NewProductMurdered,
                                                            DeceasedTypeName(gravestone.DeceasedType),
                                                            gravestone.Name,
                                                            gravestone.DateClose.ToString("R"),
                                                            gravestone.Description,
                                                            gravestone.ReferenceLink),
                AnnouncementType.ProductExempted => string.Format(ProductExempted,
                                                                  DeceasedTypeName(gravestone.DeceasedType),
                                                                  (gravestone.DateClose - DateTimeOffset.UtcNow).TotalDays.ToString("F1"),
                                                                  gravestone.Name,
                                                                  gravestone.Description),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        
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

        public static string FormatAge(TimeSpan age)
        {
            // 1 English word
            // 0 English words / 2 English words
            static string S(int num) => num == 1 ? "" : "s";
            static string Ds(double num) => Math.Abs(num - 1.0) < double.Epsilon ? "" : "s";
            
            if (age < TimeSpan.FromDays(31))
                return $"{age.Days} day{S(age.Days)}";

            if (age < TimeSpan.FromDays(365))
                return $"about {age.TotalDays / 31:F1} month{Ds(age.TotalDays / 31)}";

            return $"about {age.TotalDays / 365:F1} year{Ds(age.TotalDays / 365)}";
        }
    }
}
