using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using WarThunderLauncherV2.Models;

namespace WarThunderLauncherV2.Services;

public sealed class FolderService
{
    public string LauncherFolder => AppContext.BaseDirectory;

    public string? TryFindWarThunderFolder()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string[] possiblePaths =
        {
            @"C:\Program Files (x86)\Steam\steamapps\common\War Thunder",
            @"C:\Program Files\Steam\steamapps\common\War Thunder",
            @"D:\SteamLibrary\steamapps\common\War Thunder",
            @"E:\SteamLibrary\steamapps\common\War Thunder",

            @"C:\Games\WarThunder",
            @"C:\Games\War Thunder",
            @"D:\Games\WarThunder",
            @"D:\Games\War Thunder",
            @"E:\Games\WarThunder",
            @"E:\Games\War Thunder",

            @"C:\WarThunder",
            @"C:\War Thunder",
            @"D:\WarThunder",
            @"D:\War Thunder",
            @"E:\WarThunder",
            @"E:\War Thunder",

            @"C:\Program Files\WarThunder",
            @"C:\Program Files\War Thunder",
            @"C:\Program Files (x86)\WarThunder",
            @"C:\Program Files (x86)\War Thunder",

            Path.Combine(localAppData, "WarThunder"),
            Path.Combine(localAppData, "War Thunder"),
            Path.Combine(localAppData, "Gaijin", "WarThunder"),
            Path.Combine(localAppData, "Gaijin", "War Thunder")
        };

        foreach (var path in possiblePaths)
        {
            if (IsWarThunderFolder(path))
                return path;
        }

        return null;
    }

    public bool IsWarThunderFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (!Directory.Exists(path))
            return false;

        var launcherPath = Path.Combine(path, "launcher.exe");
        var acesPath = Path.Combine(path, "win64", "aces.exe");

        return File.Exists(launcherPath) || File.Exists(acesPath);
    }

    public string? AskUserForWarThunderFolder()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select War Thunder folder"
        };

        var result = dialog.ShowDialog();

        if (result != true)
            return null;

        return dialog.FolderName;
    }

    public void UpdateGamePaths(AppSettings settings, string warThunderPath)
    {
        settings.WarThunderPath = warThunderPath;

        var launcherPath = Path.Combine(warThunderPath, "launcher.exe");
        var acesPath = Path.Combine(warThunderPath, "win64", "aces.exe");

        settings.LauncherExePath = File.Exists(launcherPath) ? launcherPath : null;
        settings.AcesExePath = File.Exists(acesPath) ? acesPath : null;

        // V2 по умолчанию запускает именно игру, а не Gaijin launcher
        settings.LaunchViaAces = true;
    }
    
    public string GetUserSkinsFolder(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.WarThunderPath))
            throw new InvalidOperationException("War Thunder path is not selected.");

        return Path.Combine(settings.WarThunderPath, "UserSkins");
    }

    public string GetUserSightsFolder(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.WarThunderPath))
            throw new InvalidOperationException("War Thunder path is not selected.");

        return Path.Combine(settings.WarThunderPath, "UserSights");
    }

    public void OpenFolder(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Process.Start(new ProcessStartInfo
        {
            FileName = "explorer.exe",
            Arguments = $"\"{path}\"",
            UseShellExecute = true
        });
    }
}