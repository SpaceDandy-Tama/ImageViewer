using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

using WebPWrapper;
using LibObiNet;
using Imaging.DDSReader.Utils;
using LibObiNet.Dithering;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

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
        public static bool SkipExisting = false;

        public static void Encode(DisposableImage image, string outputFile, ImageEncoderQuality quality = null)
        {
            if (!Helpers.IsExtensionSupported(outputFile, ImageEncoder.Filters))
            {
                throw new EncodingNotSupported($"Writing {Path.GetExtension(outputFile)} files not supported.");
            }

            if (quality == null)
                quality = new ImageEncoderQuality();

            Image resizedImage = null;
            if (!outputFile.EndsWith(".obi"))
            {
                if (quality.RescaleMode == 2)
                {
                    //This is a bit of a cheat, but it should work just fine
                    resizedImage = ObiUtils.RescaleBitmapToMaxDimensions((Bitmap)image, quality.RescaleSize.Width, quality.RescaleSize.Height, LibObiNet.PixelFormat.Format8Grayscale);
                }
                else if (quality.RescaleMode == 3)
                {
                    //This is a bit of a cheat, but it should work just fine
                    resizedImage = ObiUtils.StretchBitmap((Bitmap)image, quality.RescaleSize.Width, quality.RescaleSize.Height, LibObiNet.PixelFormat.Format8Grayscale);
                }
                else if (quality.RescaleMode == 4)
                {
                    //This is a bit of a cheat, but it should work just fine
                    resizedImage = ObiUtils.FillBitmap((Bitmap)image, quality.RescaleSize.Width, LibObiNet.PixelFormat.Format8Grayscale);
                }
            }

            if (outputFile.EndsWith(".obi", StringComparison.OrdinalIgnoreCase))
            {
                if (quality.RescaleMode == 1)
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
                    ditheredImage = ObiUtils.ConvertToGrayscale((Bitmap)(quality.RescaleMode == 0 ? image : resizedImage), (byte)quality.ObiPixelFormat);
                }
                else if (quality.DitheringTechnique == 3)
                {
                    if ((byte)quality.ObiPixelFormat > 1)
                    {
                        Console.WriteLine("Blue Noise Dithering only works for 1 bpp.");
                        ditheredImage = ObiUtils.ConvertToGrayscale((Bitmap)(quality.RescaleMode == 0 ? image : resizedImage), (byte)quality.ObiPixelFormat);
                    }
                    else
                    {
                        ditheredImage = BlueNoise.Dither((Bitmap)(quality.RescaleMode == 0 ? image : resizedImage));
                    }
                }
                else if (quality.DitheringTechnique == 2)
                {
                    ditheredImage = Stucki.Dither((Bitmap)(quality.RescaleMode == 0 ? image : resizedImage), (byte)quality.ObiPixelFormat);
                }
                else if (quality.DitheringTechnique == 1)
                {
                    ditheredImage = FloydSteinberg.Dither((Bitmap)(quality.RescaleMode == 0 ? image : resizedImage), (byte)quality.ObiPixelFormat);
                }
                else
                {
                    ditheredImage = ObiUtils.ConvertToGrayscale((Bitmap)(quality.RescaleMode == 0 ? image : resizedImage), (byte)quality.ObiPixelFormat);
                }

                ObiFile obiFile = new ObiFile(ditheredImage, quality.ObiPixelFormat, (quality.ObiFlags & ObiFlags.RLE) != 0);
                obiFile.Save(outputFile);
                ditheredImage.Dispose();
            }
            else if (outputFile.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                Image imageRef = quality.RescaleMode == 0 ? image : resizedImage;
                imageRef.Save(outputFile, ImageFormat.Bmp);
            }
            else if (outputFile.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || outputFile.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality.Quality);

                Image imageRef = quality.RescaleMode == 0 ? image : resizedImage;
                imageRef.Save(outputFile, jpegCodec, encoderParams);
            }
            else if (outputFile.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                Image imageRef = quality.RescaleMode == 0 ? image : resizedImage;
                imageRef.Save(outputFile, ImageFormat.Png);
            }
            else if (outputFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                byte[] rawWebP = null;
                using (WebP webp = new WebP())
                {
                    Image imageRef = quality.RescaleMode == 0 ? image : resizedImage;

                    if (quality.WebpComplexity == WebPEncodingComplexity.NearLossless)
                    {
                        rawWebP = webp.EncodeNearLossless((Bitmap)imageRef, quality.Quality, quality.WebpSpeed);
                    }
                    else if(quality.WebpComplexity == WebPEncodingComplexity.Advanced)
                    {
                        if(quality.WebpType == WebPEncodingType.Lossy)
                        {
                            rawWebP = webp.EncodeLossy((Bitmap)imageRef, quality.Quality, quality.WebpSpeed);
                        }
                        else if(quality.WebpType < WebPEncodingType.Lossless)
                        {
                            rawWebP = webp.EncodeLossless((Bitmap)imageRef, quality.WebpSpeed);
                        }
                    }
                    else if(quality.WebpComplexity == WebPEncodingComplexity.Simple)
                    {
                        if (quality.WebpType == WebPEncodingType.Lossy)
                        {
                            rawWebP = webp.EncodeLossy((Bitmap)imageRef, quality.Quality);
                        }
                        else if (quality.WebpType < WebPEncodingType.Lossless)
                        {
                            rawWebP = webp.EncodeLossless((Bitmap)imageRef);
                        }
                    }
                }

                File.WriteAllBytes(outputFile, rawWebP);
            }

            if(resizedImage != null)
                resizedImage.Dispose();
        }

        public static void Encode(string sourceFile, string outputFile, ImageEncoderQuality quality = null)
        {
            using(DisposableImage image = ImageDecoder.Decode(sourceFile))
            {
                ImageEncoder.Encode(image, outputFile, quality);
            }
        }

        public static void EncodeAll(string sourceDir, string outputDir, string extension, ImageEncoderQuality quality = null)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string[] files = Directory.GetFiles(sourceDir);
            List<string> imageFiles = new List<string>();

            foreach(string file in files)
            {
                if(Helpers.IsExtensionSupported(file, ImageDecoder.Filters) && Helpers.IsExtensionSupported(file, ImageEncoder.Filters))
                {
                    string outputPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) + extension);

                    if (ImageEncoder.SkipExisting && File.Exists(outputPath))
                        continue;

                    imageFiles.Add(file);
                }
            }

            // Use Parallel.ForEach to process files in parallel
            ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(imageFiles, parallelOptions, (imageFile) =>
            {
                string outputPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(imageFile) + extension);
                ImageEncoder.Encode(imageFile, outputPath, quality);
            });

            /*
            //Singlethreaded version for testing
            foreach (string imageFile in imageFiles)
            {
                string outputPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(imageFile) + extension);
                ImageEncoder.Encode(imageFile, outputPath, quality);
            }
            */

            sw.Stop();

            Helpers.Message($"All images re-encoded in {sw.ElapsedMilliseconds} milliseconds.", Application.ProductName);
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
                    position++;
                }
                else if (args[position] == "-4bit")
                {
                    quality.ObiPixelFormat = LibObiNet.PixelFormat.Format4Grayscale;
                    position++;
                }
                else if (args[position] == "-2bit")
                {
                    quality.ObiPixelFormat = LibObiNet.PixelFormat.Format2Grayscale;
                    position++;
                }
                else if (args[position] == "-1bit")
                {
                    quality.ObiPixelFormat = LibObiNet.PixelFormat.Monochromatic;
                    position++;
                }
                else if (args[position] == "-min" || args[position] == "-minimum")
                {
                    quality.RescaleMode = 1;
                    position++;
                }
                else if (args[position].StartsWith("-max:") || args[position].StartsWith("-maximum:"))
                {
                    quality.RescaleMode = 2;
                    quality.RescaleSize = ParseSize(args[position]);
                    position++;
                }
                else if (args[position].StartsWith("-stretch:"))
                {
                    quality.RescaleMode = 3;
                    quality.RescaleSize = ParseSize(args[position]);
                    position++;
                }
                else if (args[position].StartsWith("-fill:"))
                {
                    quality.RescaleMode = 4;
                    quality.RescaleSize = ParseSize(args[position]);
                    position++;
                }
                else if (args[position] == "-blueNoise")
                {
                    quality.DitheringTechnique = 3;
                    position++;
                }
                else if (args[position] == "-stucki")
                {
                    quality.DitheringTechnique = 2;
                    position++;
                }
                else if (args[position] == "-floydSteinberg")
                {
                    quality.DitheringTechnique = 1;
                    position++;
                }
                else if (args[position] == "-RLE")
                {
                    quality.UseRLE = true;
                    position++;
                }
                else if (args[position] == "-skipExisting")
                {
                    ImageEncoder.SkipExisting = true;
                    position++;
                }
                else
                {
                    position++;
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