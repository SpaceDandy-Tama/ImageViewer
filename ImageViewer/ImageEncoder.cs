using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using WebPWrapper;
using System.Diagnostics;

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
    }

    public class ImageEncoder
    {
        public static string[] Filters = new string[] { ".bmp", ".jpg", ".jpeg", ".png", ".webp" };

        public static void Encode(DisposableImage image, string outputFile, ImageEncoderQuality quality = null)
        {
            if (!Helpers.IsExtensionSupported(outputFile, ImageEncoder.Filters))
            {
                throw new EncodingNotSupported($"Writing {Path.GetExtension(outputFile)} files not supported.");
            }

            if (quality == null)
                quality = new ImageEncoderQuality();

            if (outputFile.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                image.Image.Save(outputFile, ImageFormat.Bmp);
            }
            else if (outputFile.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || outputFile.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality.Quality);
                image.Image.Save(outputFile, jpegCodec, encoderParams);
            }
            else if (outputFile.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                image.Image.Save(outputFile, ImageFormat.Png);
            }
            else if (outputFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                byte[] rawWebP = null;
                using (WebP webp = new WebP())
                {
                    if (quality.WebpComplexity == WebPEncodingComplexity.NearLossless)
                    {
                        rawWebP = webp.EncodeNearLossless((Bitmap)image, quality.Quality, quality.WebpSpeed);
                    }
                    else if(quality.WebpComplexity == WebPEncodingComplexity.Advanced)
                    {
                        if(quality.WebpType == WebPEncodingType.Lossy)
                        {
                            rawWebP = webp.EncodeLossy((Bitmap)image, quality.Quality, quality.WebpSpeed);
                        }
                        else if(quality.WebpType < WebPEncodingType.Lossless)
                        {
                            rawWebP = webp.EncodeLossless((Bitmap)image, quality.WebpSpeed);
                        }
                    }
                    else if(quality.WebpComplexity == WebPEncodingComplexity.Simple)
                    {
                        if (quality.WebpType == WebPEncodingType.Lossy)
                        {
                            rawWebP = webp.EncodeLossy((Bitmap)image, quality.Quality);
                        }
                        else if (quality.WebpType < WebPEncodingType.Lossless)
                        {
                            rawWebP = webp.EncodeLossless((Bitmap)image);
                        }
                    }
                }

                File.WriteAllBytes(outputFile, rawWebP);
            }
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
            }

            return quality;
        }
    }
}