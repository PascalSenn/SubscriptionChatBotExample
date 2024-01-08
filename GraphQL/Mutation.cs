using System;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Subscriptions.Example.Chat;

public sealed class Mutation
{
    [Error<UnauthorizedError>]
    public Task<Chat> CreateChatAsync([Service] IChatService service, CancellationToken ct)
        => service.CreateChatAsync(ct);

    [Error<UnauthorizedError>]
    [Error<ChatClosedError>]
    [Error<ChatNotFoundError>]
    [Error<ChatNotReadyForMessageError>]
    [UseMutationConvention(PayloadFieldName = "message")]
    public Task<ChatMessage> SendMessageAsync(
        [Service] IChatService service,
        [ID] Guid chatId,
        string content,
        CancellationToken ct)
        => service.SendMessageAsync(chatId, content, ct);

    [Error<UnauthorizedError>]
    [Error<ChatNotFoundError>]
    public Task<Chat> CloseChatAsync(
        [Service] IChatService service,
        [ID<Chat>] Guid chatId,
        CancellationToken ct)
        => service.CloseChatAsync(chatId, ct);
}
