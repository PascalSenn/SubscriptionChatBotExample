using System;

namespace Subscriptions.Example.Chat;

public sealed record ChatMessageCreated(Guid MessageId) : IChatMessageEvent;
