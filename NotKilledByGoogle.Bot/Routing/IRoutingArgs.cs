namespace NotKilledByGoogle.Bot.Routing
{
    public interface IRoutingArgs<out TIncoming>
    {
        bool Continue { get; }
        TIncoming IncomingData { get; }
    }
}