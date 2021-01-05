using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Grave
{
    public class GraveAnnouncementCollection : IDisposable
    {
        public CancellationTokenSource CancellationTokenSource { get; } = new();
        public List<(DateTimeOffset, Task)> AnnouncementTasks { get; } = new();
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancellationTokenSource.Dispose();
            }
            // free native resources if there are any.
        }
    }
}