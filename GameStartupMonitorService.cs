using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WarThunderLauncherV2.Windows;

namespace WarThunderLauncherV2.Services;

public sealed class GameStartupMonitorService
{
    private const int BM_CLICK = 0x00F5;

    public async Task MonitorStartupAsync(
        Process? startedProcess,
        SplashOverlayWindow splashWindow,
        Action<string>? reportStatus = null)
    {
        var clickedDialogs = new HashSet<IntPtr>();
        var startedAt = DateTime.Now;

        Report(splashWindow, reportStatus, "MONITOR: WAITING FOR GAME WINDOW");

        while (true)
        {
            await Task.Delay(350);

            try
            {
                if (!splashWindow.IsVisible)
                    return;

                var candidateProcessIds = GetCandidateProcessIds(startedProcess);

                AutoClickNoDialogs(candidateProcessIds, clickedDialogs, splashWindow, reportStatus);
                KeepSplashOnTop(splashWindow);

                var readyGameWindow = FindReadyWarThunderWindow(candidateProcessIds);

                if (readyGameWindow != IntPtr.Zero)
                {
                    Report(splashWindow, reportStatus, "MONITOR: GAME WINDOW DETECTED");

                    await Task.Delay(1200);

                    Report(splashWindow, reportStatus, "MONITOR: CLOSING SPLASH");

                    CloseSplash(splashWindow);
                    return;
                }

                if (DateTime.Now - startedAt > TimeSpan.FromMinutes(5))
                {
                    Report(splashWindow, reportStatus, "MONITOR: TIMEOUT, CLOSING SPLASH");

                    CloseSplash(splashWindow);
                    return;
                }
            }
            catch
            {
                // Игнорируем один цикл мониторинга.
            }
        }
    }

    private static HashSet<int> GetCandidateProcessIds(Process? startedProcess)
    {
        var ids = new HashSet<int>();

        try
        {
            if (startedProcess != null)
            {
                startedProcess.Refresh();

                if (!startedProcess.HasExited)
                    ids.Add(startedProcess.Id);
            }
        }
        catch
        {
            // ignored
        }

        AddProcessesByName(ids, "aces");
        AddProcessesByName(ids, "launcher");

        return ids;
    }

    private static void AddProcessesByName(HashSet<int> ids, string processName)
    {
        try
        {
            foreach (var process in Process.GetProcessesByName(processName))
            {
                try
                {
                    if (!process.HasExited)
                        ids.Add(process.Id);
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    process.Dispose();
                }
            }
        }
        catch
        {
            // ignored
        }
    }

    private static void AutoClickNoDialogs(
        HashSet<int> candidateProcessIds,
        HashSet<IntPtr> clickedDialogs,
        SplashOverlayWindow splashWindow,
        Action<string>? reportStatus)
    {
        if (candidateProcessIds.Count == 0)
            return;

        EnumWindows((hWnd, _) =>
        {
            if (!IsWindowVisible(hWnd))
                return true;

            GetWindowThreadProcessId(hWnd, out var windowProcessId);

            if (!candidateProcessIds.Contains(windowProcessId))
                return true;

            var className = GetClassNameSafe(hWnd);
            var title = GetWindowTextSafe(hWnd);

            var looksLikeDialog =
                className == "#32770" ||
                title.Contains("error", StringComparison.OrdinalIgnoreCase) ||
                title.Contains("warning", StringComparison.OrdinalIgnoreCase) ||
                title.Contains("update", StringComparison.OrdinalIgnoreCase);

            if (!looksLikeDialog)
                return true;

            if (clickedDialogs.Contains(hWnd))
                return true;

            var noButton = FindNoButton(hWnd);

            if (noButton != IntPtr.Zero)
            {
                clickedDialogs.Add(hWnd);
                SendMessage(noButton, BM_CLICK, IntPtr.Zero, IntPtr.Zero);

                Report(splashWindow, reportStatus, "MONITOR: NO BUTTON CLICKED");
            }

            return true;
        }, IntPtr.Zero);
    }

    private static IntPtr FindNoButton(IntPtr dialogHandle)
    {
        IntPtr foundButton = IntPtr.Zero;

        EnumChildWindows(dialogHandle, (childHandle, _) =>
        {
            var className = GetClassNameSafe(childHandle);

            if (!className.Equals("Button", StringComparison.OrdinalIgnoreCase))
                return true;

            var text = NormalizeButtonText(GetWindowTextSafe(childHandle));

            if (text is "no" or "нет" or "ні")
            {
                foundButton = childHandle;
                return false;
            }

            return true;
        }, IntPtr.Zero);

        return foundButton;
    }

    private static string NormalizeButtonText(string text)
    {
        return text
            .Replace("&", "")
            .Replace("_", "")
            .Trim()
            .ToLowerInvariant();
    }

    private static IntPtr FindReadyWarThunderWindow(HashSet<int> candidateProcessIds)
    {
        if (candidateProcessIds.Count == 0)
            return IntPtr.Zero;

        IntPtr foundHandle = IntPtr.Zero;

        EnumWindows((hWnd, _) =>
        {
            if (!IsWindowVisible(hWnd))
                return true;

            GetWindowThreadProcessId(hWnd, out var windowProcessId);

            if (!candidateProcessIds.Contains(windowProcessId))
                return true;

            var className = GetClassNameSafe(hWnd);
            var title = GetWindowTextSafe(hWnd).Trim();

            if (className == "#32770")
                return true;

            if (string.IsNullOrWhiteSpace(title))
                return true;

            if (title.Contains("error", StringComparison.OrdinalIgnoreCase))
                return true;

            if (title.Contains("warning", StringComparison.OrdinalIgnoreCase))
                return true;

            if (title.Contains("update", StringComparison.OrdinalIgnoreCase))
                return true;

            if (!title.Contains("War Thunder", StringComparison.OrdinalIgnoreCase))
                return true;

            foundHandle = hWnd;
            return false;
        }, IntPtr.Zero);

        return foundHandle;
    }

    private static void CloseSplash(SplashOverlayWindow splashWindow)
    {
        if (!splashWindow.IsVisible)
            return;

        splashWindow.Dispatcher.Invoke(splashWindow.CloseWithFade);
    }

    private static void KeepSplashOnTop(SplashOverlayWindow splashWindow)
    {
        if (!splashWindow.IsVisible)
            return;

        splashWindow.Dispatcher.Invoke(() =>
        {
            splashWindow.Topmost = false;
            splashWindow.Topmost = true;
        });
    }

    private static void Report(SplashOverlayWindow splashWindow, Action<string>? reportStatus, string text)
    {
        if (reportStatus == null)
            return;

        try
        {
            splashWindow.Dispatcher.Invoke(() => reportStatus(text));
        }
        catch
        {
            // ignored
        }
    }

    private static string GetWindowTextSafe(IntPtr hWnd)
    {
        var length = GetWindowTextLength(hWnd);

        if (length <= 0)
            return string.Empty;

        var builder = new StringBuilder(length + 1);
        GetWindowText(hWnd, builder, builder.Capacity);

        return builder.ToString();
    }

    private static string GetClassNameSafe(IntPtr hWnd)
    {
        var builder = new StringBuilder(256);
        GetClassName(hWnd, builder, builder.Capacity);

        return builder.ToString();
    }

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCount);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}