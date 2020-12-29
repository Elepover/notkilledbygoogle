using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Config
{
    public interface IConfigManager<T>
    {
        T Config { get; }
        string FilePath { get; set; }
        Task<T> LoadConfigAsync(CancellationToken cancellationToken = default);
        Task SaveConfigAsync(CancellationToken cancellationToken = default);
        Task CreateConfigAsync(CancellationToken cancellationToken = default);
        Task<bool> ValidateConfigAsync(CancellationToken cancellationToken = default);
        bool Backup();
    }
}