using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WarThunderLauncherV2.Windows;

public partial class SplashOverlayWindow : Window
{
    private readonly DispatcherTimer _gifTimer = new();

    private List<BitmapSource> _gifFrames = new();
    private List<int> _gifDelays = new();

    private int _currentFrameIndex;
    private bool _isClosing;

    public SplashOverlayWindow()
    {
        InitializeComponent();

        _gifTimer.Tick += GifTimer_Tick;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Opacity = 0;

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(280),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        BeginAnimation(OpacityProperty, fadeIn);
    }

    public void SetSplashPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            ShowFallback();
            return;
        }

        var extension = Path.GetExtension(path).ToLowerInvariant();

        if (extension == ".gif")
        {
            LoadGif(path);
            return;
        }

        LoadStaticImage(path);
    }

    private void LoadStaticImage(string path)
    {
        try
        {
            _gifTimer.Stop();

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(path, UriKind.Absolute);
            image.EndInit();
            image.Freeze();

            SplashImage.Source = image;
            SplashImage.Visibility = Visibility.Visible;
            FallbackLayer.Visibility = Visibility.Collapsed;
        }
        catch
        {
            ShowFallback();
        }
    }

    private void LoadGif(string path)
    {
        try
        {
            _gifTimer.Stop();

            using var stream = File.OpenRead(path);
            var decoder = new GifBitmapDecoder(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad
            );

            _gifFrames = decoder.Frames
                .Select(frame =>
                {
                    var copy = new WriteableBitmap(frame);
                    copy.Freeze();
                    return (BitmapSource)copy;
                })
                .ToList();

            _gifDelays = decoder.Frames
                .Select(GetGifFrameDelay)
                .ToList();

            if (_gifFrames.Count == 0)
            {
                ShowFallback();
                return;
            }

            _currentFrameIndex = 0;

            SplashImage.Source = _gifFrames[0];
            SplashImage.Visibility = Visibility.Visible;
            FallbackLayer.Visibility = Visibility.Collapsed;

            _gifTimer.Interval = TimeSpan.FromMilliseconds(_gifDelays[0]);
            _gifTimer.Start();
        }
        catch
        {
            ShowFallback();
        }
    }

    private static int GetGifFrameDelay(BitmapFrame frame)
    {
        try
        {
            if (frame.Metadata is BitmapMetadata metadata &&
                metadata.ContainsQuery("/grctlext/Delay"))
            {
                var delayObject = metadata.GetQuery("/grctlext/Delay");

                if (delayObject is ushort delay)
                {
                    var milliseconds = delay * 10;
                    return Math.Clamp(milliseconds, 40, 500);
                }
            }
        }
        catch
        {
            // ignored
        }

        return 90;
    }

    private void GifTimer_Tick(object? sender, EventArgs e)
    {
        if (_gifFrames.Count == 0)
            return;

        _currentFrameIndex++;

        if (_currentFrameIndex >= _gifFrames.Count)
            _currentFrameIndex = 0;

        SplashImage.Source = _gifFrames[_currentFrameIndex];

        var delay = _gifDelays.Count > _currentFrameIndex
            ? _gifDelays[_currentFrameIndex]
            : 90;

        _gifTimer.Interval = TimeSpan.FromMilliseconds(delay);
    }

    private void ShowFallback()
    {
        _gifTimer.Stop();

        SplashImage.Source = null;
        SplashImage.Visibility = Visibility.Collapsed;
        FallbackLayer.Visibility = Visibility.Visible;
    }

    public void CloseWithFade()
    {
        if (_isClosing)
            return;

        _isClosing = true;
        _gifTimer.Stop();

        var fadeOut = new DoubleAnimation
        {
            From = Opacity,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(260),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };

        fadeOut.Completed += (_, _) => Close();

        BeginAnimation(OpacityProperty, fadeOut);
    }
}