using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using WebPWrapper;
using System.Diagnostics;
using LibObiNet;
using Imaging.DDSReader.Utils;
using LibObiNet.Dithering;

namespace Tama.ImageViewer
{
    [Serializable]
    public class EncodingNotSupported : Exception
    {
        public EncodingNotSupported()
        { }

        public EncodingNotSupported(string message)
            : base(message)
        { }

        public EncodingNotSupported(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public enum WebPEncodingComplexity
    {
        Simple,
        Advanced,
        NearLossless,
    }

    public enum WebPEncodingType
    {
        Lossy,
        Lossless,
    }

    public class ImageEncoderQuality
    {
        public int Quality = 100;
        public WebPEncodingComplexity WebpComplexity = WebPEncodingComplexity.Simple;
        public WebPEncodingType WebpType = WebPEncodingType.Lossy;
        public int WebpSpeed = 9;
        public LibObiNet.PixelFormat ObiPixelFormat = LibObiNet.PixelFormat.Format8Grayscale;
        public ObiFlags ObiFlags = ObiFlags.None;
        public int DitheringTechnique = 0;
        public bool UseRLE = false;
        public int RescaleMode = 0;
        public Size RescaleSize;
    }

    public class ImageEncoder
    {
        public static string[] Filters = new string[] { ".bmp", ".jpg", ".jpeg", ".png", ".webp", ".obi" };

        public static void Encode(DisposableImage image, string outputFile, ImageEncoderQuality quality = null)
        {
            if (!Helpers.IsExtensionSupported(outputFile, ImageEncoder.Filters))
            {
                throw new EncodingNotSupported($"Writing {Path.GetExtension(outputFile)} files not supported.");
            }

            if (quality == null)
                quality = new ImageEncoderQuality();

            Image resizedImage = null;
            if (!outputFile.EndsWith(".obi") && quality.RescaleMode == 2)
            {
                //This is a bit of a cheat, but it should work just fine
                resizedImage = ObiUtils.RescaleBitmapToMaxDimensions((Bitmap)image, quality.RescaleSize.Width, quality.RescaleSize.Height, LibObiNet.PixelFormat.Format8Grayscale);
            }
            else if (!outputFile.EndsWith(".obi") && quality.RescaleMode == 3)
            {
                //This is a bit of a cheat, but it should work just fine
                resizedImage = ObiUtils.StretchBitmap((Bitmap)image, quality.RescaleSize.Width, quality.RescaleSize.Height, LibObiNet.PixelFormat.Format8Grayscale);
            }
            else if (!outputFile.EndsWith(".obi") && quality.RescaleMode == 4)
            {
                //This is a bit of a cheat, but it should work just fine
                resizedImage = ObiUtils.FillBitmap((Bitmap)image, quality.RescaleSize.Width, LibObiNet.PixelFormat.Format8Grayscale);
            }
            else
            {
                resizedImage = new Bitmap(image);
            }

            if (outputFile.EndsWith(".obi", StringComparison.OrdinalIgnoreCase))
            {
                if (quality.RescaleMode == 0)
                {
                    resizedImage = new Bitmap(image);
                }
                else if (quality.RescaleMode == 1)
                {
                    resizedImage = ObiUtils.StretchBitmapToMinWidthRequired((Bitmap)image, quality.ObiPixelFormat);
                }
                else if (quality.RescaleMode == 2)
                {
                    resizedImage = ObiUtils.RescaleBitmapToMaxDimensions((Bitmap)image, quality.RescaleSize.Width, quality.RescaleSize.Height, quality.ObiPixelFormat);
                }
                else if (quality.RescaleMode == 3)
                {
                    resizedImage = ObiUtils.StretchBitmap((Bitmap)image, quality.RescaleSize.Width, quality.RescaleSize.Height, quality.ObiPixelFormat);
                }
                else if (quality.RescaleMode == 4)
                {
                    resizedImage = ObiUtils.FillBitmap((Bitmap)image, quality.RescaleSize.Width, quality.ObiPixelFormat);
                }

                Bitmap ditheredImage = null;
                if (quality.ObiPixelFormat == LibObiNet.PixelFormat.Format8Grayscale && quality.DitheringTechnique > 0)
                {
                    Console.WriteLine("Can't apply dithering for 8-bit.");
                    ditheredImage = ObiUtils.ConvertToGrayscale((Bitmap)resizedImage, (byte)quality.ObiPixelFormat);
                }
                else if (quality.DitheringTechnique == 3)
                {
                    if ((byte)quality.ObiPixelFormat > 1)
                    {
                        Console.WriteLine("Blue Noise Dithering only works for 1 bpp.");
                        ditheredImage = ObiUtils.ConvertToGrayscale((Bitmap)resizedImage, (byte)quality.ObiPixelFormat);
                    }
                    else
                    {
                        ditheredImage = BlueNoise.Dither((Bitmap)resizedImage);
                    }
                }
                else if (quality.DitheringTechnique == 2)
                {
                    ditheredImage = Stucki.Dither((Bitmap)resizedImage, (byte)quality.ObiPixelFormat);
                }
                else if (quality.DitheringTechnique == 1)
                {
                    ditheredImage = FloydSteinberg.Dither((Bitmap)resizedImage, (byte)quality.ObiPixelFormat);
                }
                else
                {
                    ditheredImage = ObiUtils.ConvertToGrayscale((Bitmap)resizedImage, (byte)quality.ObiPixelFormat);
                }

                ObiFile obiFile = new ObiFile(ditheredImage, quality.ObiPixelFormat, (quality.ObiFlags & ObiFlags.RLE) != 0);
                obiFile.Save(outputFile);
                ditheredImage.Dispose();
            }
            else if (outputFile.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                resizedImage.Save(outputFile, ImageFormat.Bmp);
            }
            else if (outputFile.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || outputFile.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality.Quality);
                resizedImage.Save(outputFile, jpegCodec, encoderParams);
            }
            else if (outputFile.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                resizedImage.Save(outputFile, ImageFormat.Png);
            }
            else if (outputFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                byte[] rawWebP = null;
                using (WebP webp = new WebP())
                {
                    if (quality.WebpComplexity == WebPEncodingComplexity.NearLossless)
                    {
                        rawWebP = webp.EncodeNearLossless((Bitmap)resizedImage, quality.Quality, quality.WebpSpeed);
                    }
                    else if(quality.WebpComplexity == WebPEncodingComplexity.Advanced)
                    {
                        if(quality.WebpType == WebPEncodingType.Lossy)
                        {
                            rawWebP = webp.EncodeLossy((Bitmap)resizedImage, quality.Quality, quality.WebpSpeed);
                        }
                        else if(quality.WebpType < WebPEncodingType.Lossless)
                        {
                            rawWebP = webp.EncodeLossless((Bitmap)resizedImage, quality.WebpSpeed);
                        }
                    }
                    else if(quality.WebpComplexity == WebPEncodingComplexity.Simple)
                    {
                        if (quality.WebpType == WebPEncodingType.Lossy)
                        {
                            rawWebP = webp.EncodeLossy((Bitmap)resizedImage, quality.Quality);
                        }
                        else if (quality.WebpType < WebPEncodingType.Lossless)
                        {
                            rawWebP = webp.EncodeLossless((Bitmap)resizedImage);
                        }
                    }
                }

                File.WriteAllBytes(outputFile, rawWebP);
            }

            resizedImage.Dispose();
        }

        public static void Encode(string sourceFile, string outputFile, ImageEncoderQuality quality = null)
        {
            using(DisposableImage image = ImageDecoder.Decode(sourceFile))
            {
                ImageEncoder.Encode(image, outputFile, quality);
            }
        }

        public static ImageEncoderQuality ParseEncoderArguments(string[] args, ref int position)
        {
            ImageEncoderQuality quality = new ImageEncoderQuality();

            while (position < args.Length)
            {
                if (args[position] == "-quality")
                {
                    position++;
                    if (position < args.Length)
                    {
                        quality.Quality = int.Parse(args[position]);
                        position++;
                    }
                }
                else if (args[position] == "-simple")
                {
                    quality.WebpComplexity = WebPEncodingComplexity.Simple;
                    position++;
                }
                else if (args[position] == "-advanced")
                {
                    quality.WebpComplexity = WebPEncodingComplexity.Advanced;
                    position++;
                    if (position < args.Length)
                    {
                        quality.WebpSpeed = int.Parse(args[position]);
                        position++;
                    }
                }
                else if (args[position] == "-nearLossless")
                {
                    quality.WebpComplexity = WebPEncodingComplexity.NearLossless;
                    position++;
                    if (position < args.Length)
                    {
                        quality.WebpSpeed = int.Parse(args[position]);
                        position++;
                    }
                }
                else if (args[position] == "-lossless")
                {
                    quality.WebpType = WebPEncodingType.Lossless;
                    position++;
                }
                else if (args[position] == "-8bit")
                {
                    quality.ObiPixelFormat = LibObiNet.PixelFormat.Format8Grayscale;
                }
                else if (args[position] == "-4bit")
                {
                    quality.ObiPixelFormat = LibObiNet.PixelFormat.Format4Grayscale;
                }
                else if (args[position] == "-2bit")
                {
                    quality.ObiPixelFormat = LibObiNet.PixelFormat.Format2Grayscale;
                }
                else if (args[position] == "-1bit")
                {
                    quality.ObiPixelFormat = LibObiNet.PixelFormat.Monochromatic;
                }
                else if (args[position] == "-min" || args[position] == "-minimum")
                {
                    quality.RescaleMode = 1;
                }
                else if (args[position].StartsWith("-max:") || args[position].StartsWith("-maximum:"))
                {
                    quality.RescaleMode = 2;
                    quality.RescaleSize = ParseSize(args[position]);
                }
                else if (args[position].StartsWith("-stretch:"))
                {
                    quality.RescaleMode = 3;
                    quality.RescaleSize = ParseSize(args[position]);
                }
                else if (args[position].StartsWith("-fill:"))
                {
                    quality.RescaleMode = 4;
                    quality.RescaleSize = ParseSize(args[position]);
                }
                else if (args[position] == "-blueNoise")
                {
                    quality.DitheringTechnique = 3;
                }
                else if (args[position] == "-stucki")
                {
                    quality.DitheringTechnique = 2;
                }
                else if (args[position] == "-floydSteinberg")
                {
                    quality.DitheringTechnique = 1;
                }
                else if (args[position] == "-RLE")
                {
                    quality.UseRLE = true;
                }
            }

            return quality;
        }

        private static Size ParseSize(string arg)
        {
            string[] split = arg.Split(':');
            string[] maxResolution = split[1].Split('x');

            return new Size(int.Parse(maxResolution[0]), int.Parse(maxResolution[1]));
        }
    }
}