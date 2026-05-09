using System;
using System.IO;
using System.Threading.Tasks;
using WarThunderLauncherV2.Windows;

namespace WarThunderLauncherV2.Services;

public sealed class SplashOverlayService
{
    public SplashOverlayWindow Show(string? customSplashPath)
    {
        var splashPath = ResolveSplashPath(customSplashPath);

        var window = new SplashOverlayWindow();
        window.SetSplashPath(splashPath);
        window.Show();

        return window;
    }

    public async void CloseAfter(SplashOverlayWindow window, TimeSpan delay)
    {
        await Task.Delay(delay);

        if (!window.IsVisible)
            return;

        window.Dispatcher.Invoke(window.CloseWithFade);
    }

    public void CloseNow(SplashOverlayWindow? window)
    {
        if (window == null)
            return;

        if (!window.IsVisible)
            return;

        window.CloseWithFade();
    }

    private static string? ResolveSplashPath(string? customSplashPath)
    {
        if (!string.IsNullOrWhiteSpace(customSplashPath) && File.Exists(customSplashPath))
            return customSplashPath;

        var baseDirectory = AppContext.BaseDirectory;

        string[] possiblePaths =
        {
            Path.Combine(baseDirectory, "splash.gif"),
            Path.Combine(baseDirectory, "splash.png"),
            Path.Combine(baseDirectory, "Assets", "splash.gif"),
            Path.Combine(baseDirectory, "Assets", "splash.png")
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
                return path;
        }

        return null;
    }
}