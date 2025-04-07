namespace Eos.Desktop.Features.Conversation;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using RhoMicro.CodeAnalysis;

[JsonSchema]
public sealed partial class ConversationSettings
{
    [JsonSchemaProperty(Description = "The models available to the used chat client.")]
    public List<String> Models { get; set; } = [];
}