using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tama
{
    public static class Helpers
    {
        public static bool IsExtensionSupported(string filePath, string[] supportedExtensions)
        {
            if (!Path.HasExtension(filePath))
            {
                return false;
            }

            for (int i = 0; i < supportedExtensions.Length; i++)
            {
                if (Path.GetExtension(filePath).Equals(supportedExtensions[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsLightMode()
        {
            // Check if Windows version supports the Light/Dark mode setting
            if (!IsWindows10OrHigher())
            {
                // Default to Light mode or use a manual setting for Windows 8/8.1
                return true;
            }

            // Path to the registry key for Windows 10+
            string registryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            string valueName = "AppsUseLightTheme";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath, writable: false))
                {
                    if (key == null)
                    {
                        return true;
                    }

                    object currentValue = key.GetValue(valueName);
                    if (currentValue == null)
                    {
                        return true;
                    }

                    int mode = (int)currentValue;
                    return mode == 1; // true for Light mode, false for Dark mode
                }
            }
            catch (Exception e)
            {
#if DEBUG
                throw e;
#else
                return true;
#endif
            }
        }

        public static bool IsWindows10OrHigher()
        {
            Version osVersion = Environment.OSVersion.Version;
            return osVersion.Major >= 10;
        }

        public static void Message(string message, string caption = "")
        {
            Console.WriteLine(message);
            MessageBox.Show(message, caption == string.Empty ? Application.ProductName : caption);
        }
    }
}
