using System;
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

using Photino.Blazor;

namespace Eos.Desktop
{
    using Features.Conversation;
    using Features.Shared;

    using Markdig;

    using Microsoft.Extensions.AI;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    class Program
    {
        [STAThread]
        static void Main(String[] args)
        {
            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT")}.json", optional: true)
                .Build();

            appBuilder.Services
                .AddLogging(l => l.AddDebug())
                .AddSingleton(configuration)
                .AddSingleton<IConfiguration>(sp => sp.GetRequiredService<IConfigurationRoot>())
                .AddOptions<ConversationSettings>()
                .BindConfiguration("Conversation")
                .Services
                .AddSingleton(_ => new MarkdownPipelineBuilder().UseAdvancedExtensions().Build())
                .AddSingleton<MarkdownToHtmlConverter>()
                .AddSingleton<RefinementProvider>()
                .AddSingleton<TrimLeadingThinkXmlRefinement>()
                .AddSingleton<NullRefinement>()
                .AddChatClient(sp =>
                    new OllamaChatClient(new Uri("http://localhost:11434"))
                        .AsBuilder()
                        .AddModelIdToAdditionalProperties()
                        .Refine(sp)
                        .UseFunctionInvocation()
                        .Build())
                ;

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