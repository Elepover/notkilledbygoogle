using System;

namespace NotKilledByGoogle.Bot.GraveKeeper
{
    /// <summary>
    /// Provides data to <see cref="GraveKeeper.FetchError"/> event.
    /// </summary>
    public class FetchErrorEventArgs : EventArgs
    {
        public FetchErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
        
        /// <summary>
        /// Exception that caused JSON fetch to fail.
        /// </summary>
        public Exception Exception { get; }
    }
}