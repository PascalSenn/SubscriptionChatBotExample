using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Subscriptions;

namespace Subscriptions.Example.Chat;

public sealed class ChatTopic
{
    private const string _topicName = "Chat";
    private readonly ITopicEventReceiver _eventReceiver;
    private readonly ITopicEventSender _eventSender;

    public ChatTopic(ITopicEventReceiver eventReceiver, ITopicEventSender eventSender)
    {
        _eventReceiver = eventReceiver;
        _eventSender = eventSender;
    }

    private static string CreateTopicName(Guid chatId) => $"{_topicName}_{chatId}";

    public async Task NotifyMessageCreated(ChatMessage message, CancellationToken ct)
    {
        await _eventSender.SendAsync(
            CreateTopicName(message.ChatId),
            ChatMessageEventMessage.Created(message.Id, message.Role),
            ct);
    }

    public async Task NotifyMessageUpdated(ChatMessage message, CancellationToken ct)
    {
        await _eventSender.SendAsync(
            CreateTopicName(message.ChatId),
            ChatMessageEventMessage.Updated(message.Id, message.Role),
            ct);
    }

    public async Task CompleteChat(Guid chatId)
    {
        await _eventSender.CompleteAsync(CreateTopicName(chatId));
    }

    public async IAsyncEnumerable<ChatMessageEventMessage> SubscribeAsync(Guid chatId)
    {
        var stream = await _eventReceiver
            .SubscribeAsync<ChatMessageEventMessage>(CreateTopicName(chatId));

        await foreach (var message in stream.ReadEventsAsync())
        {
            yield return message;
        }
    }
}
