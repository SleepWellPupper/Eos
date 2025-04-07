namespace Eos.Desktop.Features.Conversation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

internal sealed class ModelIdAppendingChatClient(IChatClient client) : ChatClientBase(client)
{
    protected override ValueTask OnAfterGetResponse(
        ChatResponse response,
        List<ChatMessage> requestMessages,
        ChatOptions? options,
        CancellationToken cancellationToken)
    {
        if(options is not { ModelId: { } modelId })
            return ValueTask.CompletedTask;

        response.AdditionalProperties ??= [];
        response.AdditionalProperties.Add("modelId", modelId);

        return HandleResponseMessages(
            response,
            requestMessages,
            options,
            cancellationToken,
            ( static (message, _, _, _, _, modelId) =>
                {
                    message.AdditionalProperties ??= [];
                    message.AdditionalProperties.TryAdd("modelId", modelId);

                    return ValueTask.CompletedTask;
                },
                modelId ));
    }
}