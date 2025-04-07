namespace Eos.Desktop.Features.Conversation;

using System;

using Microsoft.Extensions.AI;

public sealed class MessageModel(ChatMessage chatMessage)
{
    public Boolean ShowSource { get; set; }
    public ChatMessage ChatMessage { get; } = chatMessage;
}