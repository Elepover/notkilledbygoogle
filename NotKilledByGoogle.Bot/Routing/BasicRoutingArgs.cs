namespace NotKilledByGoogle.Bot.Routing
{
    public class BasicRoutingArgs<TIncoming> : IRoutingArgs<TIncoming>
    {
        public BasicRoutingArgs(TIncoming incomingData)
        {
            IncomingData = incomingData;
        }
        
        private bool _continue = false;
        public bool Continue => _continue;

        /// <summary>
        /// Incoming data.
        /// </summary>
        public TIncoming IncomingData { get; }
    }
}
