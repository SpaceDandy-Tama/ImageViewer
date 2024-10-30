using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Tama.ImageViewer
{
    public static class Helpers
    {
        public static Size GetSizeFromString(string sizeString)
        {
            string[] split = sizeString.Split('x');
            Size size = new Size();
            try
            {
                size.Width = int.Parse(split[0]);
                size.Height = int.Parse(split[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return size;
        }

        public static bool IsExtensionSupported(string filePath, string[] filters)
        {
            if (!Path.HasExtension(filePath))
            {
                return false;
            }

            for (int i = 0; i < filters.Length; i++)
            {
                if (Path.GetExtension(filePath).Equals(filters[i], StringComparison.OrdinalIgnoreCase))
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
    }
}
