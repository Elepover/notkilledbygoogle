using System;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Provides data for <see cref="GraveKeeper.Fetched"/> event.
    /// </summary>
    public class FetchedEventArgs : EventArgs
    {
        public FetchedEventArgs(string fetchUrl)
        {
            FetchUrl = fetchUrl;
        }
        
        /// <summary>
        /// The source of the failed fetch.
        /// </summary>
        public string FetchUrl { get; }
    }
}