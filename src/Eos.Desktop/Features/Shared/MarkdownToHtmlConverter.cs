namespace Eos.Desktop.Features.Shared;

using System;

using Markdig;

public sealed class MarkdownToHtmlConverter(MarkdownPipeline pipeline)
{
    public String Convert(String markdown)
    {
        var result = Markdig.Markdown.ToHtml(markdown, pipeline);

        return result;
    }
}