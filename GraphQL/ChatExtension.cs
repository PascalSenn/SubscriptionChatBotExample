using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;

namespace Subscriptions.Example.Chat;

[ExtendObjectType<Chat>]
public sealed class ChatExtensions
{
    [UsePaging]
    public async Task<IEnumerable<IMessage>> GetMessages(
        [Parent] Chat chat,
        [Service] IChatService service,
        CancellationToken ct)
        => await service.GetMessagesByChatIdAsync(chat.Id, ct);
}
