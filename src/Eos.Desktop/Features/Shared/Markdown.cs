namespace Eos.Desktop.Features.Shared;

using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

public sealed class Markdown(MarkdownToHtmlConverter converter) : ComponentBase
{
    [Parameter] public String Text { get; set; } = String.Empty;

    [Parameter(CaptureUnmatchedValues = true)]
    public IEnumerable<KeyValuePair<String, Object>> AdditionalAttributes { get; set; } = [];

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var html = converter.Convert(Text);

        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddMarkupContent(2, html);
        builder.CloseElement();
    }
}