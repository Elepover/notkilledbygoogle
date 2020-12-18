using System;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Provides data to <see cref="GraveKeeper.FetchError"/> event.
    /// </summary>
    public class FetchErrorEventArgs : EventArgs
    {
        public FetchErrorEventArgs(Exception exception, DateTimeOffset lastSuccessfulFetch, string failedUrl)
        {
            Exception = exception;
            LastSuccessfulFetch = lastSuccessfulFetch;
            FailedUrl = failedUrl;
        }
        
        /// <summary>
        /// Exception that caused JSON fetch to fail.
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// Timestamp of last successful fetch.
        /// </summary>
        public DateTimeOffset LastSuccessfulFetch { get; }
        /// <summary>
        /// The source of the failed fetch.
        /// </summary>
        public string FailedUrl { get; }
    }
}
