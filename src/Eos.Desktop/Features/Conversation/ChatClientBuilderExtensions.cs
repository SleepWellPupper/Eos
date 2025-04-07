namespace Eos.Desktop.Features.Conversation;

using System;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

internal static class ChatClientBuilderExtensions
{
    public static ChatClientBuilder AddModelIdToAdditionalProperties(this ChatClientBuilder builder) =>
        builder.Use(c => new ModelIdAppendingChatClient(c));
    public static ChatClientBuilder Refine(this ChatClientBuilder builder, IServiceProvider sp) =>
        builder.Use(c => new RefinedChatClient(sp.GetRequiredService<RefinementProvider>(), c));
}