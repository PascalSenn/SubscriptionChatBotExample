using System;

namespace Subscriptions.Example.Chat;

public sealed class ChatNotReadyForMessageError : Exception
{
    public ChatNotReadyForMessageError(Guid chatId)
        : base($"Chat with id '{chatId}' is not ready for messages.")
    {
    }
}
