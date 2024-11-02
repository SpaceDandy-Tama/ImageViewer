using System;
using System.Diagnostics;
using System.IO;
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

        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.jpg -quality 80
        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.webp -quality 75 -simple
        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.webp -lossless -advanced 9
        //./ImageViewer.exe -encode C:/Users/Desktop/bird.png -to C:/Users/Desktop/bird.webp -quality 40 -nearLossless 9
        //./ImageViewer.exe -encodeAll C:/Images -to C:/Images/converted .webp -quality 75 -advanced 9

        [STAThread]
        static void Main(string[] args)
        {
            AppSetting.Load();

            if(args.Length > 3)
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
    }
}
