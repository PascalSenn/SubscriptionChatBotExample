using System;

namespace Subscriptions.Example.Chat;

public sealed class ChatClosedError : Exception
{
    public ChatClosedError(Guid chatId)
        : base($"Chat with id '{chatId}' is closed.")
    {
    }
}
