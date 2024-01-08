using System;

namespace Subscriptions.Example.Chat;

public sealed record ChatMessageUpdated(Guid MessageId) : IChatMessageEvent;
