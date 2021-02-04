using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Routing
{
    public interface IRoutable<TIncoming>
    {
        bool IsEligible(TIncoming incoming);
        Task ProcessAsync(IRoutingArgs<TIncoming> args);
    }
}
