using System;
using System.IO;

namespace WarThunderLauncherV2.Services;

public sealed class BackgroundMediaService
{
    public string? ResolveBackgroundPath(string? customBackgroundPath)
    {
        if (!string.IsNullOrWhiteSpace(customBackgroundPath) && File.Exists(customBackgroundPath))
            return customBackgroundPath;

        var baseDirectory = AppContext.BaseDirectory;

        string[] possiblePaths =
        {
            Path.Combine(baseDirectory, "background.mp4"),
            Path.Combine(baseDirectory, "background.wmv"),
            Path.Combine(baseDirectory, "background.mov"),

            Path.Combine(baseDirectory, "background.png"),
            Path.Combine(baseDirectory, "background.jpg"),
            Path.Combine(baseDirectory, "background.jpeg"),

            Path.Combine(baseDirectory, "Assets", "background.mp4"),
            Path.Combine(baseDirectory, "Assets", "background.wmv"),
            Path.Combine(baseDirectory, "Assets", "background.mov"),

            Path.Combine(baseDirectory, "Assets", "background.png"),
            Path.Combine(baseDirectory, "Assets", "background.jpg"),
            Path.Combine(baseDirectory, "Assets", "background.jpeg")
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
                return path;
        }

        return null;
    }

    public bool IsVideo(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        return extension is ".mp4" or ".wmv" or ".avi" or ".mov";
    }

    public bool IsImage(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        return extension is ".png" or ".jpg" or ".jpeg" or ".bmp";
    }
}