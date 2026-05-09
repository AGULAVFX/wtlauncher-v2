using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace WarThunderLauncherV2.Services;

public sealed class SoundService
{
    private readonly List<MediaPlayer> _activePlayers = new();

    public void Play(string soundName, double volume = 0.25)
    {
        try
        {
            var path = ResolveSoundPath(soundName);

            if (path == null)
                return;

            var player = new MediaPlayer();

            player.MediaEnded += (_, _) =>
            {
                player.Close();
                _activePlayers.Remove(player);
            };

            player.MediaFailed += (_, _) =>
            {
                player.Close();
                _activePlayers.Remove(player);
            };

            _activePlayers.Add(player);

            player.Open(new Uri(path, UriKind.Absolute));
            player.Volume = Math.Clamp(volume, 0, 1);
            player.Play();
        }
        catch
        {
            // Звуки не должны ломать лаунчер.
        }
    }

    private static string? ResolveSoundPath(string soundName)
    {
        var baseDirectory = AppContext.BaseDirectory;

        string[] possiblePaths =
        {
            Path.Combine(baseDirectory, "Sounds", soundName + ".wav"),
            Path.Combine(baseDirectory, "Sounds", soundName + ".mp3"),
            Path.Combine(baseDirectory, soundName + ".wav"),
            Path.Combine(baseDirectory, soundName + ".mp3")
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
                return path;
        }

        return null;
    }
}