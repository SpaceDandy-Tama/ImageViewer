using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Imaging.DDSReader;
using Paloma;

[ComImport]
[Guid("3C7A3205-B80E-41A8-9D72-4F8C596EA7D8")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IThumbnailProvider
{
    HRESULT GetThumbnail(
        IShellItem shellItem,  // Pass the shell item here
        uint size,
        out IntPtr phbmp,
        out WTS_ALPHATYPE pdwAlpha);
}

public enum HRESULT : int
{
    S_OK = 0,
    E_FAIL = -1,
}

public enum WTS_ALPHATYPE
{
    WTSAT_UNKNOWN = 0,
    WTSAT_RGBA = 1,
    WTSAT_RGB = 2,
}

[ComImport]
[Guid("0000000C-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellItem
{
    HRESULT GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);
    // Additional IShellItem methods can be added here as needed
}

public enum SIGDN : uint
{
    SIGDN_FILESYSPATH = 0x0000000000000001,
}

[ComVisible(true)]
[Guid("8F5AC546-2733-4163-8F41-2700A9927E9F")]
[ClassInterface(ClassInterfaceType.None)]
public class ThumbnailProvider : IThumbnailProvider
{
    public HRESULT GetThumbnail(IShellItem shellItem, uint size, out IntPtr phbmp, out WTS_ALPHATYPE pdwAlpha)
    {
        phbmp = IntPtr.Zero;
        pdwAlpha = WTS_ALPHATYPE.WTSAT_UNKNOWN;

        try
        {
            // Retrieve the image path from IShellItem
            string imagePath = GetImagePath(shellItem); // Now GetImagePath will take an IShellItem

            using (var originalImage = GetBitmap(imagePath))
            {
                // Calculate the new dimensions while maintaining the aspect ratio
                float aspectRatio = (float)originalImage.Width / originalImage.Height;
                int newWidth, newHeight = (int)size;

                if (aspectRatio > 1) // Wider than tall
                {
                    newWidth = (int)size;
                    newHeight = (int)(size / aspectRatio);
                }
                else // Taller than wide or square
                {
                    newHeight = (int)size;
                    newWidth = (int)(size * aspectRatio);
                }

                using (var thumbnail = new Bitmap(originalImage, new Size(newWidth, newHeight)))
                {
                    // Convert the thumbnail to HBITMAP
                    phbmp = thumbnail.GetHbitmap();
                    pdwAlpha = WTS_ALPHATYPE.WTSAT_RGB; // Set the alpha type if applicable
                }
            }
            return HRESULT.S_OK; // Success
        }
        catch (Exception ex)
        {
            // Log or handle exception as needed
            Console.WriteLine("Error: " + ex.Message);
            return HRESULT.E_FAIL; // Failed to create thumbnail
        }
    }

    private string GetImagePath(IShellItem shellItem)
    {
        // Initialize the path variable
        IntPtr ppszName;
        HRESULT hr = shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out ppszName);

        if (hr == HRESULT.S_OK)
        {
            // Convert the pointer to a string
            string filePath = Marshal.PtrToStringAuto(ppszName);
            Marshal.FreeCoTaskMem(ppszName); // Free the memory allocated for the string
            return filePath;
        }

        throw new Exception("Could not get image path from IShellItem.");
    }

    private Bitmap GetBitmap(string imagePath)
    {
        Bitmap bitmap = null;

        if (imagePath.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
        {
            TargaImage targaImage = new TargaImage(imagePath);
            bitmap = TargaImage.CopyToBitmap(targaImage);
            targaImage.Dispose();
        }

        return bitmap;
    }
}