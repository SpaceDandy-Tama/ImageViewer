using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Imaging.DDSReader;
using Imaging.DDSReader.Utils;
using Paloma;
using WebPWrapper;

namespace Tama.ImageViewer
{
    [Serializable]
    public class DecodingNotSupported : Exception
    {
        public DecodingNotSupported()
        { }

        public DecodingNotSupported(string message)
            : base(message)
        { }

        public DecodingNotSupported(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public class ImageDecoder
    {
        public static string[] Filters = new string[] { ".bmp", ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".tif", ".ico", ".tga", ".dds", ".webp", ".obi" };

        public static DisposableImage Decode(string sourceFile)
        {
            if (!File.Exists(sourceFile))
            {
                throw new FileNotFoundException($"{sourceFile} not found.");
            }
            if (!Helpers.IsExtensionSupported(sourceFile, ImageDecoder.Filters))
            {
                throw new DecodingNotSupported($"Reading {Path.GetExtension(sourceFile)} files not supported.");
            }

            DisposableImage image = null;

            if (sourceFile.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            {
                using (TargaImage targaImage = new TargaImage(sourceFile))
                {
                    image = new DisposableImage(TargaImage.CopyToBitmap(targaImage));
                    image.TargaPixelDepth = targaImage.Header.PixelDepth;
                    image.TargaRLE = targaImage.Header.IsRLE;
                }
            }
            else if (sourceFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
            {
                byte[] ddsRaw = File.ReadAllBytes(sourceFile);
                using (DDSImage ddsImage = new DDSImage(ddsRaw))
                {
                    image = new DisposableImage(DDS.CopyToBitmap(ddsImage));
                    image.DDSPixelFormat = ddsImage.PixelFormat;
                }
            }
            else if (sourceFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                byte[] webpRaw = File.ReadAllBytes(sourceFile);
                using (WebP webp = new WebP())
                {
                    int width;
                    int height;
                    bool hasAlpha;
                    bool hasAnimation;
                    string format;
                    webp.GetInfo(webpRaw, out width, out height, out hasAlpha, out hasAnimation, out format);

                    image = new DisposableImage(webp.Decode(webpRaw));
                    image.WEBPFormat = format;
                }
            }
            else
            {
                image = new DisposableImage(sourceFile);
            }

            return image;
        }
    }
}