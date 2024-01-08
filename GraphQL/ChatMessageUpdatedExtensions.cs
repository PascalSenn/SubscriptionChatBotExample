using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;

namespace Subscriptions.Example.Chat;

[ExtendObjectType<ChatMessageUpdated>]
public sealed class ChatMessageUpdatedExtensions
{
    [BindMember(nameof(ChatMessageUpdated.MessageId))]
    public async Task<ChatMessage?> GetMessageAsync(
        [Parent] ChatMessageUpdated parent,
        [Service] IChatService chatService,
        CancellationToken cancellationToken)
    {
        return await chatService.GetMessageByIdAsync(parent.MessageId, cancellationToken);
    }
}
