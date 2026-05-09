namespace WarThunderLauncherV2.Models;

public sealed class AppSettings
{
    public string? WarThunderPath { get; set; }

    public string? LauncherExePath { get; set; }

    public string? AcesExePath { get; set; }

    public bool LaunchViaAces { get; set; } = true;

    public string? BackgroundPath { get; set; }

    public string? SplashGifPath { get; set; }

    public string DonateUrl { get; set; } = "https://www.patreon.com/AGULA";
    
    public string SupportUrl { get; set; } = "https://linktr.ee/AGULA.VFX";

    public bool SoundsEnabled { get; set; } = true;
}