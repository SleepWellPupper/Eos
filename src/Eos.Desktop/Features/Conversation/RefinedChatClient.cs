namespace Eos.Desktop.Features.Conversation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

sealed class RefinedChatClient(RefinementProvider refinements, IChatClient client) : ChatClientBase(client)
{
    protected override ValueTask OnAfterGetResponse(
        ChatResponse response,
        List<ChatMessage> requestMessages,
        ChatOptions? options,
        CancellationToken cancellationToken)
    {
        if(options is not { ModelId: { } modelId })
            return ValueTask.CompletedTask;

        var refinement = refinements.GetRefinement(modelId);

        return HandleResponseMessages(
            response,
            requestMessages,
            options,
            cancellationToken,
            ( static (message, _, _, _, _, refinement) =>
                {
                    if(message.Role != ChatRole.Assistant || !message.Contents.All(c => c is TextContent))
                        return ValueTask.CompletedTask;

                    var refinedText = (Span<Char>)message.Text.ToCharArray();
                    refinement.Refine(ref refinedText);

                    if(refinedText.Length == message.Text.Length)
                        return ValueTask.CompletedTask;

                    message.Contents = [new TextContent(refinedText.ToString())];
                    return ValueTask.CompletedTask;
                },
                refinement ));
    }
}