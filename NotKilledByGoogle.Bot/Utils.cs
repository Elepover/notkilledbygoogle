using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot
{
    public static class Utils
    {
        public static readonly char[] IllegalMarkdownV2Chars =
        {
            // '*', '`', '_', '~', '[', ']', '(', ')' are ignored Markdown stuff because caller
            // should escape them manually for clarity
            '>', '#',
            '+', '-', '=', '|', '{',
            '}', '.', '!'
        };

        public static readonly char[] AdditionalChars =
        {
            '*', '`', '_', '~', '[', ']', '(', ')'
        };
        
        /// <summary>
        /// Throw an exception if the passed value is <see langword="null"/>.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="exceptionToThrow">Exception to throw if <see cref="value"/> is <see langword="null"/>.</param>
        /// <param name="valueMemberName"><see cref="value"/>'s member name in caller.</param>
        /// <typeparam name="T">Value's corresponding type. Can be automatically inferred.</typeparam>
        /// <returns>The <see cref="value"/> itself if not <see langword="null"/>.</returns>
        /// <exception cref="Exception">Passed <see cref="value"/> is <see langword="null"/>.</exception>
        public static T ThrowIfNull<T>(T? value, Exception? exceptionToThrow = null, string valueMemberName = "")
        {
            if (value is null) throw exceptionToThrow ?? new ArgumentNullException(valueMemberName + " cannot be null."); 
            return value;
        }
        
        /// <summary>
        /// A better delay mechanism that accepts longer time than <see cref="int.MaxValue"/>.<br />
        /// <br />
        /// From: https://stackoverflow.com/questions/27995221/task-delay-for-more-than-int-maxvalue-milliseconds
        /// </summary>
        /// <param name="delay">Time to delay, in milliseconds.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel this task.</param>
        /// <returns></returns>
        public static async Task Delay(long delay, CancellationToken cancellationToken = default)
        {
            while (delay > 0)
            {
                var currentDelay = delay > int.MaxValue ? int.MaxValue : (int) delay;
                await Task.Delay(currentDelay, cancellationToken);
                delay -= currentDelay;
            }
        }

        /// <inheritdoc cref="Delay(long,System.Threading.CancellationToken)"/>
        public static Task Delay(TimeSpan delay, CancellationToken cancellationToken = default)
            => Delay(Convert.ToInt64(delay.TotalMilliseconds), cancellationToken);

        /// <summary>
        /// Escape illegal characters. <br />
        /// <code>'*', '`', '_', '~', '[', ']', '(', ')'</code> are ignored Markdown stuff. Escape them manually.
        /// </summary>
        /// <param name="str"><see langword="string"/> to escape.</param>
        /// <param name="additional">Additional characters to escape.</param>
        /// <returns></returns>
        public static string Escape(string str, params char[] additional)
        {
            var escapeMode = false;
            var sb = new StringBuilder();
            var chars = new List<char>(IllegalMarkdownV2Chars);
            chars.AddRange(additional);

            foreach (var ch in str)
            {
                // in escape mode, just append what it wanted to escape.
                // (it's already escaped)
                if (escapeMode)
                {
                    escapeMode = false;
                    goto NextChar;
                }
                
                // enter escape mode on encountering backslash.
                // (it's already escaped so don't escape next character)
                if (ch == '\\')
                {
                    escapeMode = true;
                    goto NextChar;
                }

                // not in escape mode, check if it needs to be escaped.
                if (chars.Contains(ch))
                {
                    // escape it
                    sb.Append('\\');
                }
                
                NextChar:
                sb.Append(ch);
            }

            return sb.ToString();
        }

        public static string Age(TimeSpan duration)
        {
            string S(int num)
                => num == 1 ? "" : "s";
            var days = Convert.ToInt32(duration.TotalDays);
            if (days < 365)
                return $"{days} day{S(days)}";
            var years = days / 365;
            var remainder = days % 365;
            return $"{years} year{S(years)}{(remainder == 0 ? "" : $" and {remainder} day{S(remainder)}")}";
        }

        public static string SimpleAge(TimeSpan duration)
        {
            var years = duration.TotalDays / 365;
            var sb = new StringBuilder();
            sb.Append($"{years:F2}");
            sb.Append(Convert.ToInt32(years) == 1 ? " year" : " years");
            return sb.ToString();
        }

        public static string CapitalizeFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("Input cannot be empty or null.");
            return str.First().ToString().ToUpper() + str.Substring(1);
        }
    }
}
