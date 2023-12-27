// https://blog.csdn.net/m0_46555380/article/details/129945529

using System;
using System.Diagnostics;

namespace WindowsSystemTheme
{
    public static class Windows10Theme
    {
        public static bool IsSystemThemeDark => GetWindowsTheme();
        public static bool IsSystemThemeLight => !GetWindowsTheme();

        public static bool IsSupport()
        {
            // Windows 10 1809
            // see "https://www.addictivetips.com/windows-tips/how-to-enable-the-dark-theme-in-windows-10/"
            Debug.WriteLine(Environment.OSVersion.Version.ToString());
            // https://blog.csdn.net/qing666888/article/details/50843640
            // https://blog.csdn.net/k1988/article/details/47614529
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 17763;
        }

        public static bool IsWindows11()
        {
            // https://blog.csdn.net/marin1993/article/details/127069281
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 22000;
        }

        // true为深色模式 反之false
        private static bool GetWindowsTheme()
        {
            const string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string registryValueName = "AppsUseLightTheme";
            // 这里也可能是LocalMachine(HKEY_LOCAL_MACHINE)
            // see "https://www.addictivetips.com/windows-tips/how-to-enable-the-dark-theme-in-windows-10/"
            object registryValueObject = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryKeyPath)?.GetValue(registryValueName);
            if (registryValueObject is null) return false;
            return (int)registryValueObject <= 0;
        }
    }
}
