using System;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Subscriptions.Example.Chat;

public sealed class Chat(Guid id, ChatStatus status, string userId)
{
    [ID]
    public Guid Id { get; set; } = id;

    public ChatStatus Status { get; set; } = status;

    [GraphQLIgnore]
    public string UserId { get; set; } = userId;
}
