using System;
using System.Runtime.CompilerServices;

namespace NotKilledByGoogle.Bot
{
    public static class ConsoleHelper
    {
        private static string GetPrefix(string severity, string caller) => $"[{DateTimeOffset.Now:G}][{severity}@{caller}] ";
        
        public static void Info(string log, [CallerMemberName] string caller = "?")
            => Console.WriteLine(GetPrefix("INFO", caller) + log);

        public static void Warning(string log, [CallerMemberName] string caller = "?")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(GetPrefix("WARN", caller) + log);
            Console.ResetColor();
        }

        public static void Error(string log, [CallerMemberName] string caller = "?")
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(GetPrefix("FAIL", caller) + log);
            Console.ResetColor();
        }
    }
}
