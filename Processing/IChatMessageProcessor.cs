using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

public interface IChatMessageProcessor
{
    void QueueMessageAsync(ChatMessage message);

    Task ProcessAsync(CancellationToken ct);
}
