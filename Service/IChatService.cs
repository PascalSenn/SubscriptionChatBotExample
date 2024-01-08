using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

public interface IChatService
{
    Task<Chat> CreateChatAsync(CancellationToken ct);

    Task<Chat?> GetChatByIdAsync(Guid chatId, CancellationToken ct);

    Task<ChatMessage> SendMessageAsync(Guid chatId, string content, CancellationToken ct);

    Task<ChatMessage?> GetMessageByIdAsync(Guid messageId, CancellationToken ct);

    IAsyncEnumerable<IChatMessageEvent> SubscribeToChatMessagesAsync(Guid chatId);

    Task<Chat> CloseChatAsync(Guid chatId, CancellationToken ct);
}
