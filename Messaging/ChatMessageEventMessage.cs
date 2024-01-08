using System;

namespace Subscriptions.Example.Chat;

public sealed record ChatMessageEventMessage(
    Guid MessageId,
    ChatMessageKind Kind,
    ChatMessageRole Role)
{
    public static ChatMessageEventMessage Created(Guid messageId, ChatMessageRole role) =>
        new(messageId, ChatMessageKind.Created, role);

    public static ChatMessageEventMessage Updated(Guid messageId, ChatMessageRole role) =>
        new(messageId, ChatMessageKind.Updated, role);
}
