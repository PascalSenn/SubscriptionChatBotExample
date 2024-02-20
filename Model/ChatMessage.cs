using System;
using System.Runtime.InteropServices;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Subscriptions.Example.Chat;

public class ChatMessage(
    Guid id,
    Guid chatId,
    string content,
    ChatMessageRole role,
    DateTime sentAt) : IMessage
{
    [ID]
    public Guid Id { get; set; } = id;

    [GraphQLIgnore]
    public Guid ChatId { get; set; } = chatId;

    public string Content { get; set; } = content;

    public ChatMessageRole Role { get; set; } = role;

    public DateTime SentAt { get; set; } = sentAt;
}

public class DocumentMessage(
    Guid id,
    Guid chatId,
    string documentUrl,
    ChatMessageRole role,
    DateTime sentAt) : IMessage
{
    [ID]
    public Guid Id { get; set; } = id;

    [GraphQLIgnore]
    public Guid ChatId { get; set; } = chatId;

    public string DocumentUrl { get; set; } = documentUrl;

    public ChatMessageRole Role { get; set; } = role;

    public DateTime SentAt { get; set; } = sentAt;
}

[GraphQLName("Message")]
public interface IMessage
{
    [ID]
    public Guid Id { get; set; }

    [GraphQLIgnore]
    public Guid ChatId { get; set; }

    public ChatMessageRole Role { get; set; }

    public DateTime SentAt { get; set; }
}
