using CommunityToolkit.Maui;
using Indexr.Core.Services;
using Indexr.MAUI;
using Indexr.MAUI.ViewModels;
using Indexr.MAUI.Views;
using Microsoft.Extensions.Logging;

namespace Indexr.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<UncPathService>();
            builder.Services.AddSingleton<FolderScannerService>();
            builder.Services.AddSingleton<ProjectFileService>();
            builder.Services.AddSingleton<RecentProjectsService>();
            builder.Services.AddSingleton<VersionIncrementService>();
            builder.Services.AddSingleton<DocumentGeneratorService>();

            // ViewModels
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<ProjectViewModel>();

            // Pages
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<ProjectPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}