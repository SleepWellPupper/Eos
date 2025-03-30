using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace Eos.Desktop
{
    using Features.Conversation;

    using Microsoft.Extensions.AI;

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

            appBuilder.Services
                .AddChatClient(sp=> new OllamaChatClient(
                        new Uri("http://localhost:11434"),
                        "llama3.1")
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build());
            
            appBuilder.Services
                .AddLogging();

            RegisterModels(appBuilder.Services);
            
            // register root component and selector
            appBuilder.RootComponents.Add<App>("app");

            var app = appBuilder.Build();

            // customize window
            app.MainWindow
                .SetIconFile("favicon.ico")
                .SetTitle("Eos");

            AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
            {
                app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
            };

            app.Run();
        }

        private static void RegisterModels(IServiceCollection services)
        {
            services.AddTransient<ConversationModel>();
        }
    }
}
