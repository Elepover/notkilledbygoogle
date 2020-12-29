using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Grave
{
    public class GraveKeeper : IDisposable
    {
        private readonly string _graveyardJsonLocation;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly HttpClient _client = new()
        {
            DefaultRequestVersion = new(2, 0),
            Timeout = TimeSpan.FromSeconds(30)
        };
        private bool _busy = false;

        private async Task GraveyardUpdateLoop()
        {
            _busy = true;
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // fetch JSON from server
                    using var response = await _client.GetAsync(_graveyardJsonLocation, _cancellationTokenSource.Token);
                    response.EnsureSuccessStatusCode();
                    // ensure it's recognizable JSON
                    var deserialized = await JsonSerializer.DeserializeAsync<List<Gravestone>>(await response.Content.ReadAsStreamAsync(), Gravestone.SerializerOptions);
                    Gravestones = Utils.ThrowIfNull(deserialized).ToArray();
                    // set latest fetch time
                    LatestSuccessfulFetch = DateTimeOffset.Now;
                    // notify all subscribers
                    Fetched?.Invoke(this, new FetchedEventArgs(_graveyardJsonLocation));
                }
                catch (Exception ex)
                {
                    FetchError?.Invoke(this, new FetchErrorEventArgs(ex, LatestSuccessfulFetch ?? DateTimeOffset.UnixEpoch, _graveyardJsonLocation));
                }

                try { await Task.Delay(UpdateInterval, _cancellationTokenSource.Token); } finally { } 
            }
            _busy = false;
        }

        /// <summary>
        /// Occurs when the <see cref="GraveKeeper"/> successfully fetched data.
        /// </summary>
        public event FetchedEventHandler? Fetched;
        /// <summary>
        /// Occurs when the <see cref="GraveKeeper"/> fails to fetch JSON data.
        /// </summary>
        public event FetchErrorEventHandler? FetchError;
        public delegate void FetchErrorEventHandler(object? sender, FetchErrorEventArgs e);
        public delegate void FetchedEventHandler(object? sender, FetchedEventArgs e);
        
        /// <summary>
        /// Specifies how frequently should this <see cref="GraveKeeper"/> check for graveyard updates, in milliseconds.
        /// </summary>
        public int UpdateInterval { get; set; } = 300000;

        /// <summary>
        /// Gravestones fetched from specified graveyard.
        /// </summary>
        public Gravestone[] Gravestones { get; private set; } = Array.Empty<Gravestone>();
        /// <summary>
        /// The timestamp of last successful fetch.
        /// </summary>
        public DateTimeOffset? LatestSuccessfulFetch { get; private set; }

        public GraveKeeper(string graveyardJsonLocation)
        {
            _graveyardJsonLocation = graveyardJsonLocation;
        }

        public void Start()
        {
            if (_busy || _cancellationTokenSource.IsCancellationRequested)
                throw new InvalidOperationException("Cannot start an active, stopping or stopped " + nameof(GraveKeeper));

            Task.Run(GraveyardUpdateLoop);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                _client.Dispose();
                _cancellationTokenSource.Dispose();
            }
            // free native resources if there are any.
        }
    }
}
