using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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

        public static void CreateShortcut(string shortcutPath, string targetPath, string workingDir, string description, string iconPath)
        {
            IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();

            // Create the shortcut
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)wsh.CreateShortcut(shortcutPath);

            // Set the properties for the shortcut
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = workingDir;
            if(description != null)
            {
                shortcut.Description = description;
            }
            if (iconPath != null)
            {
                shortcut.IconLocation = iconPath;
            }

            // Save the shortcut
            shortcut.Save();
        }

        public static async Task DownloadFileAsync(string url, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    using (var fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (log them, show message, etc.)
                    Console.WriteLine($"Error downloading file: {ex.Message}");
                }
            }
        }
    }
}
