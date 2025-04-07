namespace Eos.Desktop.Features.Conversation;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class ConversationModel
{
    public ConversationModel(
        IChatClient client,
        IOptionsMonitor<ConversationSettings> settings,
        ILogger<ConversationModel> logger)
    {
        _client = client;
        _settings = settings;
        _logger = logger;
        Model = Models.FirstOrDefault(String.Empty);
    }


    private CancellationTokenSource? _cts;
    private readonly IChatClient _client;
    private readonly IOptionsMonitor<ConversationSettings> _settings;
    private readonly ILogger<ConversationModel> _logger;

    private String _faultedMessage = String.Empty;

    public List<MessageModel> Messages { get; } = [];
    public String Message { get; set; } = String.Empty;
    public Boolean Loading { get; private set; }
    public String Error { get; private set; } = String.Empty;
    public String Model { get; set; }
    public IReadOnlyList<String> Models => _settings.CurrentValue.Models;

    public void CancelSubmission() => _cts?.Cancel();

    public async Task Submit()
    {
        if(_cts is not null || Interlocked.Exchange(ref _cts, new()) is not null)
            return;

        var message = Message is null or []
            ? _faultedMessage
            : Message;

        Message = String.Empty;

        _faultedMessage = Error = String.Empty;

        try
        {
            Loading = true;

            if(message is not null and not [])
                Messages.Add(new(new ChatMessage() { Contents = [new TextContent(message)] }));

            var options = new ChatOptions() { ModelId = Model };
            var response = await _client.GetResponseAsync(Messages.Select(m => m.ChatMessage), options, _cts.Token);

            for(var i = response.Messages.Count - 1; i >= 0; i--)
                Messages.Add(new(response.Messages[i]));
        } catch(Exception ex)
        {
            _logger.LogError(ex, "Error while getting response.");
            Error = ex.Message;
            _faultedMessage = message;
            Messages.RemoveAt(Messages.Count - 1);
        } finally
        {
            Loading = false;
            _cts = null;
        }
    }
}