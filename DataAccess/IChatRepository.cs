using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

public interface IChatRepository
{
    ValueTask<Chat?> GetChatByIdAsync(Guid id, CancellationToken ct);

    ValueTask<IReadOnlyList<Chat>> GetChatsByIdsAsync(IReadOnlyList<Guid> id, CancellationToken ct);

    ValueTask<Chat> CreateChatAsync(string userId, CancellationToken ct);

    ValueTask<ChatMessage> CreateMessageAsync(
        Guid chatId,
        string content,
        ChatMessageRole role,
        CancellationToken ct);

    ValueTask<IReadOnlyList<ChatMessage>> GetMessagesByIdsAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken ct);

    ValueTask<IReadOnlyList<ChatMessage>> GetMessagesByChatIdAsync(
        Guid chatId,
        CancellationToken ct);
}
