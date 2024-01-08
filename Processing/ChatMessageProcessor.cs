using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Subscriptions.Example.Chat;

public sealed class ChatMessageProcessor
    : IChatMessageProcessor
{
    private const string _exampleMessage = """
        This is a long reply from the assistant. It is so long that it will take a while to process.
        It is chunked into multiple messages and transmitted to the client on each chunk.
        At the end of the processing the complete message is stored in the database.
        The user should receive the notification in 10 character chunks.
        Each chunk is sent with a delay of 500ms.
        """;

    private readonly ChatTopic _chatTopic;
    private readonly IChatRepository _repository;

    // This only works with the in memory provider. You might want to use some sort of messageing
    // system like Azure Service Bus,  RabbitMQ et al. to make this work in a distributed system.
    private readonly Channel<ChatMessage> _channel = Channel.CreateBounded<ChatMessage>(1000);

    public ChatMessageProcessor(ChatTopic chatTopic, IChatRepository repository)
    {
        _chatTopic = chatTopic;
        _repository = repository;
    }

    public void QueueMessageAsync(ChatMessage message)
    {
        _channel.Writer.TryWrite(message);
    }

    public async Task ProcessAsync(CancellationToken ct)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(ct))
        {
            await ProcessMessageAsync(message, ct);
        }
    }

    private async Task ProcessMessageAsync(ChatMessage message, CancellationToken ct)
    {
        var chat = await _repository.GetChatByIdAsync(message.ChatId, ct);

        if (chat is null or not { Status: ChatStatus.Ready })
        {
            return;
        }

        try
        {
            chat.Status = ChatStatus.Processing;
            var assistantMessage =
                await _repository.CreateMessageAsync(chat.Id, "", ChatMessageRole.Assistant, ct);

            await _chatTopic.NotifyMessageCreated(assistantMessage, ct);

            foreach (var chunk in ChunkString(_exampleMessage, 10))
            {
                assistantMessage.Content += chunk;
                await _chatTopic.NotifyMessageUpdated(assistantMessage, ct);
                await Task.Delay(500, ct);
            }

            assistantMessage.Content = _exampleMessage;
        }
        finally
        {
            chat.Status = ChatStatus.Ready;
        }
    }

    private static IEnumerable<string> ChunkString(string str, int chunkSize)
    {
        for (var i = 0; i < str.Length; i += chunkSize)
        {
            yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }
    }
}
