using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Grave
{
    public sealed class GraveAnnouncementCollection : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public CancellationToken GetCancellationToken() => _cancellationTokenSource.Token;
        public List<(DateTimeOffset, Task)> AnnouncementTasks { get; } = new();

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}