using System;

namespace Subscriptions.Example.Chat;

public sealed class ChatNotFoundError : Exception
{
    public ChatNotFoundError(Guid chatId)
        : base($"Chat with id '{chatId}' not found.")
    {
    }
}
