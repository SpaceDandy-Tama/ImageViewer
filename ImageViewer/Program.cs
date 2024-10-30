using System;
using System.IO;
using System.Windows.Forms;

using ImageViewer.BingHelper;

namespace Tama.ImageViewer
{
    public static class Program
    {
        public static OpenFileDialog Ofd;
        public static string Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png, *.gif, *.tiff, *.tif, *.ico, *.tga, *.dds, *.webp)|" +
            "*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tiff;*.tif;*.ico;*.tga;*.dds;*.webp";
        public static string[] Filters = new string[] { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".tif", ".ico", ".tga", ".dds", ".webp" };

        public static string CurrentFile = null;

        [STAThread]
        static void Main(string[] args)
        {
            AppSetting.Load();

            if (args.Length == 2)
            {
                if (args[0].StartsWith("-print"))
                {
                    ImagePrinter.PrintImage(args[1]);
                }

                return;
            }
            if(args.Length == 1)
            {
                if (args[0].StartsWith("-"))
                {
                    if(args[0] == "-silentBing")
                    {
                        BingImageDownloader.DownloadImageOfTheDay(AppSetting.Current.BingImageSavePath, null).GetAwaiter().GetResult();
                        AppSetting.Current.Save();
                    }
                    else if(args[0] == "-silentBingMultiple")
                    {
                        BingImageDownloader.DownloadLast8Images(AppSetting.Current.BingImageSavePath, null).GetAwaiter().GetResult();
                    }

                    return;
                }
                else
                {
                    if (Helpers.IsExtensionSupported(args[0], Filters))
                    {
                        CurrentFile = Path.GetFullPath(args[0]);
                    }
                    else
                    {
                        MessageBox.Show("Unrecognized file extension. Please refer to Readme.md for a list of supported file extensions.", "Tama's Image Viewer");
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
    }
}
