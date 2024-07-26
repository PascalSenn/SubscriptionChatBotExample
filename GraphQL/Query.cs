using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types.Relay;

namespace Subscriptions.Example.Chat;

public sealed class Query
{
    [NodeResolver]
    public Task<Chat?> GetChatById(
        [Service] IChatService service,
        Guid id,
        CancellationToken ct)
        => service.GetChatByIdAsync(id, ct);

    public async Task<IMessage> GetChatMessageById(
        [Service] IChatService service,
        [ID] Guid id,
        CancellationToken ct)
    {
        var message = await service.GetMessageByIdAsync(id, ct);
        if (message is null)
        {
            throw new Exception("Message not found");
        }

        return message;
    }

    public async Task<IReadOnlyList<Chat>> GetChatMessagesByChatId(
        [Service] IChatService service,
        CancellationToken ct)
        => await service.GetChatsAsync(ct);
}
