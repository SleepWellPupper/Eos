namespace Eos.Desktop.Features.Conversation;

using System;

public interface IChatMessageRefinement
{
    void Refine(ref Span<Char> message);
}