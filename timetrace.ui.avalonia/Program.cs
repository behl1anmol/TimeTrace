using Avalonia;
using System;

namespace timetrace.ui.avalonia;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .WithDeveloperTools()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
