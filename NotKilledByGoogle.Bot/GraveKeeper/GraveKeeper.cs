using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.GraveKeeper
{
    public class GraveKeeper
    {
        private readonly string _graveyardJsonLocation;
        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _busy = false;

        private async Task GraveyardUpdateLoop()
        {
            _busy = true;
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // fetch JSON from server
                    var req = WebRequest.CreateHttp(_graveyardJsonLocation);
                    using var res = await req.GetResponseAsync();
                    await using var rs = res.GetResponseStream();
                    // ensure it's recognizable JSON
                    var deserialized = await JsonSerializer.DeserializeAsync<List<Gravestone>>(rs, Gravestone.SerializerOptions);
                    Gravestones = Utils.ThrowIfNull(deserialized).ToArray();
                    // set latest fetch time
                    LatestSuccessfulFetch = DateTimeOffset.Now;
                    // notify all subscribers
                    Fetched?.Invoke(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    FetchError?.Invoke(this, new FetchErrorEventArgs(ex));
                }

                await Task.Delay(UpdateInterval);
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
        public delegate void FetchedEventHandler(object? sender, EventArgs e);
        
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
    }
}
