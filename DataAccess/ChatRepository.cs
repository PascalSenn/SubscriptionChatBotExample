using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

public class ChatRepository
    : IChatRepository
{
    private readonly IDictionary<Guid, Chat> _chats = new Dictionary<Guid, Chat>();
    private readonly IDictionary<Guid, ChatMessage> _messages = new Dictionary<Guid, ChatMessage>();

    public ValueTask<Chat?> GetChatByIdAsync(Guid id, CancellationToken ct)
        => new(_chats.TryGetValue(id, out var chat) ? chat : null);

    public ValueTask<IReadOnlyList<Chat>> GetChatsByIdsAsync(
        IReadOnlyList<Guid> id,
        CancellationToken ct)
        => new(_chats.Where(x => id.Contains(x.Key)).Select(x => x.Value).ToList());

    public ValueTask<Chat> CreateChatAsync(string userId, CancellationToken ct)
    {
        var chat = new Chat(Guid.NewGuid(), ChatStatus.Ready, userId);
        _chats.Add(chat.Id, chat);
        return new(chat);
    }

    public ValueTask<ChatMessage> CreateMessageAsync(
        Guid chatId,
        string content,
        ChatMessageRole role,
        CancellationToken ct)
    {
        var message = new ChatMessage(Guid.NewGuid(), chatId, content, role, DateTime.UtcNow);
        _messages.Add(message.Id, message);
        return new(message);
    }

    public ValueTask<IReadOnlyList<ChatMessage>> GetMessagesByIdsAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken ct)
        => new(_messages.Values
            .Where(x => ids.Contains(x.Id))
            .ToList());

    public ValueTask<IReadOnlyList<ChatMessage>> GetMessagesByChatIdAsync(
        Guid chatId,
        CancellationToken ct)
        => new(_messages.Values
            .Where(x => x.ChatId == chatId)
            .ToList());
}
