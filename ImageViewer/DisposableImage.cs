using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class DisposableImage : IDisposable
{
    public Image Image { get; private set; }
    private MemoryStream MemoryStream;

    //These Additional Values have been put here because they are inaccasible once the respective image is converted to bitmap and disposed
    public byte TargaPixelDepth;
    public bool TargaRLE;
    public Imaging.DDSReader.Utils.PixelFormat DDSPixelFormat;
    public string WEBPFormat;
    public int WEBPFrameDuration;
    public LibObiNet.PixelFormat ObiFormat;
    public LibObiNet.ObiFlags ObiFlags;

    //These Properties and Methods are for ease of use
    public int Width => Image.Width;
    public int Height => Image.Height;
    public Guid[] FrameDimensionsList => Image.FrameDimensionsList;
    public int GetFrameCount(FrameDimension dimension) => Image.GetFrameCount(dimension);
    public int SelectActiveFrame(FrameDimension dimension, int frameIndex) => Image.SelectActiveFrame(dimension, frameIndex);
    public void RotateFlip(RotateFlipType rotateFlipType) => Image.RotateFlip(rotateFlipType);
    public void Save(string filename) => Image.Save(filename);

    public DisposableImage(Bitmap bitmap)
    {
        MemoryStream = null;
        Image = bitmap.Clone() as Bitmap;
    }
    public DisposableImage(string filePath)
    {
        MemoryStream = new MemoryStream(File.ReadAllBytes(filePath));
        Image = Image.FromStream(MemoryStream);
    }
    public DisposableImage(byte[] bytes)
    {
        MemoryStream = new MemoryStream(bytes);
        Image = Image.FromStream(MemoryStream);
    }

    // Implicit conversion operator to convert DisposableImage to Image
    public static implicit operator Image(DisposableImage disposableImage)
    {
        return disposableImage.Image;
    }

    // Implicit conversion operator to convert Bitmap to DisposableImage
    public static implicit operator DisposableImage(Bitmap bitmap)
    {
        return new DisposableImage(bitmap);
    }

    public void Dispose()
    {
        // Dispose of the Image and MemoryStream when disposing
        Image?.Dispose();
        MemoryStream?.Dispose();
    }
}