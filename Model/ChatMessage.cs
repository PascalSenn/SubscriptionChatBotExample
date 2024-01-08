using System;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Subscriptions.Example.Chat;

public class ChatMessage(
    Guid id,
    Guid chatId,
    string content,
    ChatMessageRole role,
    DateTime sentAt)
{
    [ID]
    public Guid Id { get; set; } = id;

    [GraphQLIgnore]
    public Guid ChatId { get; set; } = chatId;

    public string Content { get; set; } = content;

    public ChatMessageRole Role { get; set; } = role;

    public DateTime SentAt { get; set; } = sentAt;
}
