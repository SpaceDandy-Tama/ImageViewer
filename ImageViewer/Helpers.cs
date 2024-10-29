using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Tama.ImageViewer
{
    public static class Helpers
    {
        public static Size GetSizeFromString(string sizeString)
        {
            string[] split = sizeString.Split('x');
            Size size = new Size();
            try
            {
                size.Width = int.Parse(split[0]);
                size.Height = int.Parse(split[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return size;
        }

        public static bool IsExtensionSupported(string filePath, string[] filters)
        {
            if (!Path.HasExtension(filePath))
            {
                return false;
            }

            for (int i = 0; i < filters.Length; i++)
            {
                if (Path.GetExtension(filePath).Equals(filters[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
