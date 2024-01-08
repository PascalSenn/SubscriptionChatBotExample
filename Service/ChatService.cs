using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// IMPORTANT: This methods only be used directly from the GraphQL Layer as they us data loaders. 
/// </remarks>
public sealed class ChatService : IChatService
{
    private readonly ChatTopic _chatTopic;
    private readonly ISessionAccessor _sessionAccessor;
    private readonly IChatRepository _repository;
    private readonly IChatByIdDataLoader _chatById;
    private readonly IMessagesByIdDataLoader _messagesById;
    private readonly IChatMessageProcessor _processor;

    public ChatService(
        IChatRepository repository,
        ChatTopic chatTopic,
        ISessionAccessor sessionAccessor,
        IChatByIdDataLoader chatById,
        IMessagesByIdDataLoader messagesById,
        IChatMessageProcessor processor)
    {
        _repository = repository;
        _chatTopic = chatTopic;
        _sessionAccessor = sessionAccessor;
        _chatById = chatById;
        _messagesById = messagesById;
        _processor = processor;
    }

    public async Task<Chat> CreateChatAsync(CancellationToken ct)
    {
        var session = await _sessionAccessor.GetSessionAsync(CancellationToken.None);
        if (session is null)
        {
            throw UnauthorizedError.Create();
        }

        var chat = await _repository.CreateChatAsync(session.UserId, ct);

        return chat;
    }

    public async Task<Chat?> GetChatByIdAsync(Guid chatId, CancellationToken ct)
    {
        var session = await _sessionAccessor.GetSessionAsync(CancellationToken.None);
        if (session is null)
        {
            return null;
        }

        var chat = await _chatById.LoadAsync(chatId, ct);

        if (chat is null)
        {
            return null;
        }

        if (chat.UserId != session.UserId)
        {
            // to prevent leaking information about the existence of a chat
            return null;
        }

        return chat;
    }

    public async Task<IReadOnlyList<ChatMessage>> GetChatMessagesByChatIdAsync(
        Guid chatId,
        CancellationToken ct)
    {
        var session = await _sessionAccessor.GetSessionAsync(CancellationToken.None);
        if (session is null)
        {
            return Array.Empty<ChatMessage>();
        }

        var chat = await _chatById.LoadAsync(chatId, ct);

        if (chat is null)
        {
            return Array.Empty<ChatMessage>();
        }

        if (chat.UserId != session.UserId)
        {
            // to prevent leaking information about the existence of a chat
            return Array.Empty<ChatMessage>();
        }

        var messages = await _repository.GetMessagesByChatIdAsync(chatId, ct);

        return messages
            .Where(x => x.Role is ChatMessageRole.User or ChatMessageRole.Assistant)
            .ToArray();
    }

    public async Task<ChatMessage?> GetMessageByIdAsync(Guid messageId, CancellationToken ct)
    {
        var session = await _sessionAccessor.GetSessionAsync(CancellationToken.None);
        if (session is null)
        {
            return null;
        }

        var message = await _messagesById.LoadAsync(messageId, ct);
        if (message is null)
        {
            return null;
        }

        var chat = await _chatById.LoadAsync(message.ChatId, ct);
        if (chat is null)
        {
            return null;
        }

        if (chat.UserId != session.UserId)
        {
            return null;
        }

        return message;
    }

    public async Task<ChatMessage> SendMessageAsync(
        Guid chatId,
        string content,
        CancellationToken ct)
    {
        var session = await _sessionAccessor.GetSessionAsync(CancellationToken.None);
        if (session is null)
        {
            throw UnauthorizedError.Create();
        }

        var chat = await _chatById.LoadAsync(chatId, ct);

        if (chat is null)
        {
            throw new ChatNotFoundError(chatId);
        }

        if (chat.UserId != session.UserId)
        {
            // to prevent leaking information about the existence of a chat
            throw new ChatNotFoundError(chatId);
        }

        if (chat.Status == ChatStatus.Closed)
        {
            throw new ChatClosedError(chatId);
        }

        if (chat.Status == ChatStatus.Processing)
        {
            throw new ChatNotReadyForMessageError(chatId);
        }

        var message =
            await _repository.CreateMessageAsync(chatId, content, ChatMessageRole.User, ct);

        await _chatTopic.NotifyMessageCreated(message, ct);
        _processor.QueueMessageAsync(message);

        return message;
    }

    /// <inheritdoc />
    public async Task<Chat> CloseChatAsync(Guid chatId, CancellationToken ct)
    {
        var session = await _sessionAccessor.GetSessionAsync(CancellationToken.None);
        if (session is null)
        {
            throw UnauthorizedError.Create();
        }

        var chat = await _chatById.LoadAsync(chatId, ct);

        if (chat is null)
        {
            throw new ChatNotFoundError(chatId);
        }

        if (chat.UserId != session.UserId)
        {
            // to prevent leaking information about the existence of a chat
            throw new ChatNotFoundError(chatId);
        }

        chat.Status = ChatStatus.Closed;

        await _chatTopic.CompleteChat(chatId);

        return chat;
    }

    public async IAsyncEnumerable<IChatMessageEvent> SubscribeToChatMessagesAsync(Guid chatId)
    {
        var session = await _sessionAccessor.GetSessionAsync(CancellationToken.None);
        if (session is null)
        {
            yield break;
        }

        // IMPORTANT! You cannot use dataloaders in SubscribeWith methods!!!
        var chat = await _repository.GetChatByIdAsync(chatId, CancellationToken.None);
        if (chat is null)
        {
            yield break;
        }

        if (chat.UserId != session.UserId)
        {
            yield break;
        }

        await foreach (var message in _chatTopic.SubscribeAsync(chatId))
        {
            switch (message)
            {
                case { Role: ChatMessageRole.System }:
                    // We dont want to send system messages to the client.
                    break;

                case { Kind: ChatMessageKind.Created }:
                    yield return new ChatMessageCreated(message.MessageId);
                    break;

                case { Kind: ChatMessageKind.Updated }:
                    yield return new ChatMessageUpdated(message.MessageId);
                    break;
            }
        }
    }
}
