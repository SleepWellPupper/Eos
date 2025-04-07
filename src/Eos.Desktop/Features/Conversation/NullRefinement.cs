namespace Eos.Desktop.Features.Conversation;

using System;

internal sealed class NullRefinement : IChatMessageRefinement
{
    public void Refine(ref Span<Char> message) { }
}