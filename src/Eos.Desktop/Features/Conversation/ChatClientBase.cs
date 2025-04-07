namespace Eos.Desktop.Features.Conversation;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

internal class ChatClientBase(IChatClient innerClient) : DelegatingChatClient(innerClient)
{
    public override IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public override async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(messages);

        var requestMessages = messages switch
        {
            List<ChatMessage> l => l,
            ChatMessage[] a => [..a],
            _ => messages.ToList()
        };

        await OnBeforeGetResponse(requestMessages, options, cancellationToken);

        var response = await InnerClient.GetResponseAsync(requestMessages, options, cancellationToken);

        await OnAfterGetResponse(response, requestMessages, options, cancellationToken);

        return response;
    }

    protected virtual ValueTask OnBeforeGetResponse(
        List<ChatMessage> requestMessages,
        ChatOptions? options,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ValueTask.CompletedTask;
    }

    protected virtual async ValueTask OnAfterGetResponse(
        ChatResponse response,
        List<ChatMessage> requestMessages,
        ChatOptions? options,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await HandleResponseMessages<Object>(response, requestMessages, options, cancellationToken);
    }

    protected virtual async ValueTask HandleResponseMessages<TState>(
        ChatResponse response,
        List<ChatMessage> requestMessages,
        ChatOptions? options,
        CancellationToken cancellationToken,
        (
            Func<ChatMessage, ChatResponse, List<ChatMessage>, ChatOptions?, CancellationToken, TState, ValueTask>?
            handler,
            TState state) handlerData = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        for(var index = 0; index < response.Messages.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var message = response.Messages[index];

            if(handlerData is ({ } handler, var state))
                await handler.Invoke(message, response, requestMessages, options, cancellationToken, state!);

            await OnResponseMessage(message, response, requestMessages, options, cancellationToken);
        }
    }

    protected virtual ValueTask OnResponseMessage(
        ChatMessage message,
        ChatResponse response,
        List<ChatMessage> requestMessages,
        ChatOptions? options,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ValueTask.CompletedTask;
    }
}