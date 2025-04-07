namespace Eos.Desktop.Features.Conversation;

using System;

internal sealed class RefinementProvider(
    TrimLeadingThinkXmlRefinement trimLeadingThinkXmlRefinement,
    NullRefinement nullRefinement)
{
    public IChatMessageRefinement GetRefinement(String modelId) => trimLeadingThinkXmlRefinement;
}