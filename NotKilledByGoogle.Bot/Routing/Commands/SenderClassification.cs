using System;

namespace NotKilledByGoogle.Bot.Routing.Commands
{
    /// <summary>
    /// Accepted message senders.
    /// </summary>
    [Flags]
    public enum SenderClassification
    {
        /// <summary>
        /// Unclassified sender.
        /// </summary>
        None = 0b000,
        /// <summary>
        /// Sender is an administrator.
        /// </summary>
        Admin = 0b001,
        /// <summary>
        /// Sender is a real person.
        /// </summary>
        User = 0b010,
        /// <summary>
        /// Sender is a bot.
        /// </summary>
        Bot = 0b100,
        Any = 0b111
    }
}