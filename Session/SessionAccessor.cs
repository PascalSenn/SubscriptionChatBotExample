using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

public sealed class SessionAccessor : ISessionAccessor
{
    /// <inheritdoc />
    public ValueTask<Session?> GetSessionAsync(CancellationToken ct)
    {
        // This method is async as you might have some permissions that you have to load from
        // somewhere. Before you make this method sync, make sure that you really never need
        // to fetch some data to populate the session.
        // If you change this method to sync, you have to change it everywhere in your code aswell.

        return new ValueTask<Session?>(new Session("123"));
    }
}
