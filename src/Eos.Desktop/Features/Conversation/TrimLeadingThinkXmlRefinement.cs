namespace Eos.Desktop.Features.Conversation;

using System;

using Microsoft.Extensions.Logging;

internal sealed class TrimLeadingThinkXmlRefinement(ILogger<TrimLeadingThinkXmlRefinement> logger)
    : IChatMessageRefinement
{
    public void Refine(ref Span<Char> message)
    {
        var length = 0;

        if(message.StartsWith("<think>"))
        {
            var index = message.IndexOf("</think>");

            if(index is not -1)
            {
                length = index + "</think>".Length;
                message = message[length ..];
            }
        }

        logger.LogInformation("Trimmed {Length} chars.", length);
    }
}