using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Subscriptions.Example.Chat;

public sealed class Subscription
{
    public async IAsyncEnumerable<IChatMessageEvent> OnMessageSubscriber(
        Guid chatId,
        [Service] IChatService service)
    {
        var messages = service.SubscribeToChatMessagesAsync(chatId);
        await foreach (var message in messages)
        {
            yield return message;
        }
    }

    [Subscribe(With = nameof(OnMessageSubscriber))]
    public IChatMessageEvent OnChatMessagesUpdated(
        [EventMessage] IChatMessageEvent @event,
        [ID] Guid chatId)
        => @event;
}
