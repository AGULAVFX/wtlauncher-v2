using System;
using System.Diagnostics;
using System.IO;
using WarThunderLauncherV2.Models;

namespace WarThunderLauncherV2.Services;

public sealed class GameLaunchService
{
    public Process? Launch(AppSettings settings)
    {
        var exePath = GetExecutablePath(settings);

        if (string.IsNullOrWhiteSpace(exePath))
            throw new InvalidOperationException("War Thunder aces.exe was not found.");

        if (!File.Exists(exePath))
            throw new FileNotFoundException("Executable file was not found.", exePath);

        var workingDirectory = Path.GetDirectoryName(exePath);

        if (string.IsNullOrWhiteSpace(workingDirectory))
            throw new InvalidOperationException("Working directory was not found.");

        var startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            WorkingDirectory = workingDirectory,
            UseShellExecute = true
        };

        return Process.Start(startInfo);
    }

    private static string? GetExecutablePath(AppSettings settings)
    {
        // V2: всегда предпочитаем прямой запуск игры через aces.exe
        if (!string.IsNullOrWhiteSpace(settings.AcesExePath) && File.Exists(settings.AcesExePath))
            return settings.AcesExePath;

        // fallback, если aces.exe почему-то не найден
        if (!string.IsNullOrWhiteSpace(settings.LauncherExePath) && File.Exists(settings.LauncherExePath))
            return settings.LauncherExePath;

        return null;
    }
}