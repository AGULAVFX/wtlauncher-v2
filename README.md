# War Thunder Launcher V2
 custom launcher for War Thunder with OLED-style UI, custom video background, custom splash overlay, direct `aces.exe` launch, folder shortcuts, support/donate buttons, UI sounds, and a small system log.

> This is an unofficial custom launcher. It is not affiliated with Gaijin Entertainment or War Thunder.

> How to Use
Download or build the launcher.
Start WarThunderLauncherV2.exe.
On first launch, select your War Thunder folder if it is not detected automatically.
Click Launch.
> The launcher expects the War Thunder folder to contain one of these:
launcher.exe
win64/aces.exe
The recommended launch target is: win64/aces.exe


---

## Features

- Minimal OLED-style interface
- Direct launch through `aces.exe`
- Custom background support:
  - `.mp4`
  - `.mov`
  - `.wmv`
  - `.avi`
  - `.png`
  - `.jpg`
  - `.jpeg`
  - `.bmp`
- Custom splash overlay support:
  - `.mov`
  - `.mp4`
  - `.wmv`
  - `.avi`
  - `.gif`
  - `.png`
  - `.jpg`
  - `.jpeg`
  - `.bmp`
- Animated 800×450 splash overlay
- Automatic startup monitoring
- Auto-clicks `No / Нет / Ні` on War Thunder update/error dialog
- Quick folder access:
  - Launcher folder
  - UserSkins folder
  - UserSights folder
- Donate button
- Support button
- Clickable About panel
- Optional UI sounds
- Small status log panel

---

Requirements

For development:

Windows 10 / Windows 11
.NET 8 SDK or newer
JetBrains Rider / Visual Studio / any C# IDE

For users:

Windows 10 / Windows 11
War Thunder installed
No separate .NET installation required if using the self-contained published build

Project Structure:

WarThunderLauncherV2
│
├─ Assets
│  └─ WarThunderLauncherV2.ico
│
├─ Models
│  └─ AppSettings.cs
│
├─ Services
│  ├─ AppSettingsService.cs
│  ├─ BackgroundMediaService.cs
│  ├─ FolderService.cs
│  ├─ GameLaunchService.cs
│  ├─ GameStartupMonitorService.cs
│  ├─ SoundService.cs
│  └─ SplashOverlayService.cs
│
├─ Windows
│  ├─ SplashOverlayWindow.xaml
│  └─ SplashOverlayWindow.xaml.cs
│
├─ MainWindow.xaml
├─ MainWindow.xaml.cs
├─ App.xaml
├─ App.xaml.cs
└─ WarThunderLauncherV2.csproj

Buttons
Launch

Starts War Thunder directly through aces.exe.

Open folder

Opens a context menu with:

Launcher folder
User skins folder
User sights folder
Donate

Opens the donate link from the launcher settings.

Support

The wrench button opens the support link from the launcher settings.

Log button

The ≡ button shows or hides the system log.

Logo / About

Click the WT / LAUNCHER V2 text to open the About panel.

Customization

Place custom files next to the final .exe file.

Example final folder:

publish
│
├─ WarThunderLauncherV2.exe
├─ background.mp4
├─ splash.mov
│
└─ Sounds
   ├─ hover.wav
   ├─ click.wav
   ├─ launch.wav
   ├─ support.wav
   ├─ error.wav
   └─ success.wav
Custom Background

The launcher automatically searches for these files near the .exe:

background.mp4
background.wmv
background.mov
background.avi
background.png
background.jpg
background.jpeg
background.bmp

It also checks inside:

Assets/

Example:

WarThunderLauncherV2.exe
background.mp4

Recommended video format:

MP4
H.264
1920x1080 or 1280x720
No audio or muted audio

.mov is supported, but Windows/WPF may require proper codecs. For best compatibility, use .mp4.

Custom Splash

The launcher automatically searches for these files near the .exe:

splash.mov
splash.mp4
splash.wmv
splash.avi
splash.gif
splash.png
splash.jpg
splash.jpeg
splash.bmp

It also checks inside:

Assets/

Recommended splash size:

800x450

Recommended video splash format:

MP4
H.264
800x450
No audio

Example:

WarThunderLauncherV2.exe
splash.mp4

or:

WarThunderLauncherV2.exe
splash.gif
UI Sounds

Sounds are optional.

Create a folder named:

Sounds

Place it next to the launcher .exe.

Supported sound names:

hover.wav
click.wav
launch.wav
success.wav
error.wav
support.wav

Also supported:

hover.mp3
click.mp3
launch.mp3
success.mp3
error.mp3
support.mp3

Sound usage:

File	Used for
hover.wav	Mouse hover
click.wav	Button click
launch.wav	Launch button
support.wav	Support button
error.wav	Error feedback
success.wav	Reserved for success feedback

If a sound file is missing, the launcher continues working normally.

Settings File

The launcher stores settings here:

C:\Users\<YourUser>\AppData\Roaming\AGULA\WarThunderLauncherV2\settings.json

Example:

{
  "WarThunderPath": "D:\\Games\\WarThunder",
  "LauncherExePath": "D:\\Games\\WarThunder\\launcher.exe",
  "AcesExePath": "D:\\Games\\WarThunder\\win64\\aces.exe",
  "LaunchViaAces": true,
  "BackgroundPath": null,
  "SplashGifPath": null,
  "DonateUrl": "https://linktr.ee/AGULA.VFX",
  "SupportUrl": "https://linktr.ee/AGULA.VFX",
  "SoundsEnabled": true
}

If the launcher still opens an old donate/support link, delete or edit this file:

settings.json

The launcher will create a new one on next start.

Build from Source

Open the project folder:

cd C:\Users\User\RiderProjects\WarThunderLauncherV2\WarThunderLauncherV2

Build debug:

dotnet build

Publish release as a single self-contained .exe:

dotnet clean

dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:DebugType=None /p:DebugSymbols=false /p:PublishTrimmed=false

Published files will be here:

bin\Release\net8.0-windows\win-x64\publish
Recommended Final Release Folder
WarThunderLauncherV2
│
├─ WarThunderLauncherV2.exe
├─ background.mp4
├─ splash.mp4
│
└─ Sounds
   ├─ hover.wav
   ├─ click.wav
   ├─ launch.wav
   ├─ support.wav
   └─ error.wav

Then zip the folder and share it.

Notes
The launcher prefers aces.exe over launcher.exe.
The custom splash closes only when the real War Thunder window appears.
During startup, if War Thunder shows an update/error dialog with a No button, the launcher can automatically click it.
The launcher does not modify game files.
User skins and sights folders are opened only for convenience.
Disclaimer

This project is an unofficial fan-made/custom launcher.
War Thunder and related names belong to their respective owners.
Use at your own risk.


Можно ещё добавить в GitHub рядом папки:

```text
screenshots/
release/
Sounds/
