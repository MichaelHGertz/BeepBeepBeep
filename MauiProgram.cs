using Microsoft.Extensions.Logging;

namespace BeepBeepBeep;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if ANDROID || WINDOWS
        builder.Services.AddSingleton<IBeepPlayer, BeepPlayer>();
#else
        builder.Services.AddSingleton<IBeepPlayer, NoOpBeepPlayer>();
#endif
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
