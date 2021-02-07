using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Routing
{
    public interface IRoutable<in TIncoming>
    {
        bool IsEligible(TIncoming incoming);
        Task ProcessAsync(IRoutingArgs<TIncoming> args);
    }
}
