using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImageViewer.BingHelper;

namespace Tama.ImageViewer
{
    public static class Program
    {
        public static OpenFileDialog Ofd;
        public static string Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png, *.gif, *.tiff, *.tif, *.ico, *.tga, *.dds, *.webp, *.obi)|" +
            "*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tiff;*.tif;*.ico;*.tga;*.dds;*.webp;*.obi";

        public static string CurrentFile = null;

        private static string[] ExampleImageURLs = new string[]
        {
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/A Colorful Aurora Bore Over a Rockey Beach.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/A Colorful Sunset Over a City with a Mountain in the Background.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/A Mushroom in the Middle of a Forest.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/A Scenic View of a Valley With Mountains in the Background.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/A Small Waterfall in the Middle of a Mountain.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/A Yellow Object in the Middle of a Black Background.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/Desert During Nighttime.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/Starship and Super Heavy Stack.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/The Sun is Shining Brightly Over the Water.jpg",
            "https://raw.githubusercontent.com/SpaceDandy-Tama/ImageViewer/master/ImageViewer/Example Images/Two Orange Flowers with Green Stems in Front of a Black Background.jpg",
        };

        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.jpg -quality 80
        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.webp -quality 75 -simple
        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.webp -lossless -advanced 9
        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.webp -quality 40 -nearLossless 9
        //./ImageViewer.exe -encodeAll C:/Images -to C:/Images/converted .webp -quality 75 -advanced 9

        [STAThread]
        static void Main(string[] args)
        {
            AppSetting.Load();

            if (args.Length > 3)
            {
                if (args[0] == ("-encode"))
                {
                    string sourceFile = Path.GetFullPath(args[1]);
                    string outputFile = Path.GetFullPath(args[3]);
                    ImageEncoderQuality quality = null;
                    //Optional arguments
                    if (args.Length > 4)
                    {
                        int position = 4;
                        quality = ImageEncoder.ParseEncoderArguments(args, ref position);
                    }
                    ImageEncoder.Encode(sourceFile, outputFile, quality);
                }
                else if (args[0] == "-encodeAll" && args.Length > 4)
                {
                    string sourceDir = Path.GetFullPath(args[1]);
                    string outputDir = Path.GetFullPath(args[3]);
                    string extension = args[4];
                    ImageEncoderQuality quality = null;
                    //Optional arguments
                    if (args.Length > 5)
                    {
                        int position = 5;
                        quality = ImageEncoder.ParseEncoderArguments(args, ref position);
                    }
                }

                return;
            }
            if (args.Length > 1)
            {
                if (args[0] == "-print")
                {
                    ImagePrinter.PrintImage(args[1]);
                }

                return;
            }
            if (args.Length == 1)
            {
                if (args[0].StartsWith("-"))
                {
                    if (args[0] == "-silentBing")
                    {
                        BingImageDownloader.DownloadImageOfTheDay(AppSetting.Current.BingImageSavePath, null).GetAwaiter().GetResult();
                        AppSetting.Current.Save();
                    }
                    else if (args[0] == "-silentBingMultiple")
                    {
                        BingImageDownloader.DownloadLast8Images(AppSetting.Current.BingImageSavePath, null).GetAwaiter().GetResult();
                    }
                    else if (args[0] == "-downloadExampleImages")
                    {
                        DownloadExampleImages();
                    }
                    else if (args[0] == "-createShortcuts")
                    {
                        GetShortcutFullPaths(out string desktopUser, out string startMenuUser);

                        Helpers.Message(desktopUser);
                        Helpers.Message(startMenuUser);

                        Helpers.CreateShortcut(desktopUser, Application.ExecutablePath, Application.StartupPath, "Provides lightning fast image viewing for your everyday pleasure.", null);
                        Helpers.CreateShortcut(startMenuUser, Application.ExecutablePath, Application.StartupPath, "Provides lightning fast image viewing for your everyday pleasure.", null);
                    }
                    else if (args[0] == "-removeShortcuts")
                    {
                        GetShortcutFullPaths(out string desktopUser, out string startMenuUser);

                        if (File.Exists(desktopUser))
                            File.Delete(desktopUser);

                        if (File.Exists(startMenuUser))
                            File.Delete(startMenuUser);
                    }

                    return;
                }
                else
                {
                    if (Helpers.IsExtensionSupported(args[0], ImageDecoder.Filters))
                    {
                        CurrentFile = Path.GetFullPath(args[0]);
                    }
                    else
                    {
                        Helpers.Message($"Reading {Path.GetExtension(args[0])} files not supported. Please refer to the Readme.md file for a list of supported formats.");
                        return;
                    }
                }
            }

            Program.Ofd = new OpenFileDialog();
            Program.Ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        public static void DownloadExampleImages()
        {
            string desktopDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Example Images");

            if (!Directory.Exists(desktopDir))
            {
                Directory.CreateDirectory(desktopDir);
            }

            for (int i = 0; i < ExampleImageURLs.Length; i++)
            {
                string filePath = Path.Combine(desktopDir, Path.GetFileName(ExampleImageURLs[i]));

                if (!File.Exists(filePath))
                {
                    Helpers.DownloadFileAsync(ExampleImageURLs[i], filePath).GetAwaiter().GetResult();
                    Console.WriteLine($"{filePath} downloaded");
                }
            }
        }

        public static void GetShortcutFullPaths(out string desktopUser, out string startMenuUser)
        {
            #region DO NOT CHANGE THESE EVER
            //These must not be changed, because installer uses this to create/remove shortcuts.
            desktopUser = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            desktopUser = Path.Combine(desktopUser, "ImageViewer.lnk");
            startMenuUser = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            if (Directory.Exists(Path.Combine(startMenuUser, "Programs")))
                startMenuUser = Path.Combine(startMenuUser, "Programs");
            startMenuUser = Path.Combine(startMenuUser, "ImageViewer.lnk");
            #endregion
        }
    }
}