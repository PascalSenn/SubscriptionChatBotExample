using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

public interface ISessionAccessor
{
    ValueTask<Session?> GetSessionAsync(CancellationToken ct);
}
