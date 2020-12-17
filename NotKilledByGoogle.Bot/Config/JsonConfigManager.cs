using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Config
{
    public class JsonConfigManager<T> where T : class, new()
    {
        private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
        {
            WriteIndented = true
        }; 

        private static async Task<string> SerializeAsync(object obj, CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, obj, DefaultSerializerOptions, cancellationToken);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        /// <summary>
        /// Read configurations from file.
        /// </summary>
        /// <param name="path">File's full path.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel this task.</param>
        /// <inheritdoc cref="JsonSerializer.DeserializeAsync"/>
        public static async Task<T> ReadConfigAsync(string path, CancellationToken cancellationToken = default)
        {
            await using var fs = File.OpenRead(path);
            var result = await JsonSerializer.DeserializeAsync<T>(fs, DefaultSerializerOptions, cancellationToken);
            if (result is null) throw new JsonException("STJ returned null deserialization result.");
            return result;
        }

        /// <summary>
        /// Save an object to file.
        /// </summary>
        /// <inheritdoc cref="ReadConfigAsync"/>
        public static async Task WriteConfigAsync(object value, string path, CancellationToken cancellationToken = default)
            => await File.WriteAllTextAsync(path, await SerializeAsync(value, cancellationToken), cancellationToken);

        /// <summary>
        /// Current configurations stored in this <see cref="JsonConfigManager{T}"/>.
        /// </summary>
        public T? Config { get; private set; }
        
        /// <summary>
        /// Load configurations from file.
        /// </summary>
        /// <inheritdoc cref="ReadConfigAsync"/>
        public async Task<T> LoadConfigAsync(string path, CancellationToken cancellationToken = default)
        {
            Config = await ReadConfigAsync(path, cancellationToken);
            return Config;
        }

        /// <summary>
        /// Save configurations to file.
        /// </summary>
        /// <inheritdoc cref="ReadConfigAsync"/>
        public Task SaveConfigAsync(string path, CancellationToken cancellationToken = default)
            => WriteConfigAsync(Utils.ThrowIfNull(Config), path, cancellationToken);

        public Task CreateConfigAsync(string path, CancellationToken cancellationToken = default)
            => WriteConfigAsync(new T(), path, cancellationToken);
    }
}