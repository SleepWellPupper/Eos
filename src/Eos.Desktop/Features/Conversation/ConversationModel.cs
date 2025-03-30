namespace Eos.Desktop.Features.Conversation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

public sealed class ConversationModel(IChatClient client)
{
    public List<ChatMessage> Messages { get; } = [];
    public String Title { get; } = "Conversation Page";
    // public async Task On
}