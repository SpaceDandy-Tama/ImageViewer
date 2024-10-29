using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Tama.ImageViewer
{
    public static class ImagePrinter
    {
        public static void PrintImage(string imagePath)
        {
            // Check if the image file exists
            if (!System.IO.File.Exists(imagePath))
            {
                throw new ArgumentException("Image file does not exist: " + imagePath);
            }

            // Load the image
            Image imageToPrint = Image.FromFile(imagePath);

            // Create a PrintDocument
            using (PrintDocument printDocument = new PrintDocument())
            {
                // Set up the PrintPage event
                printDocument.PrintPage += (sender, e) =>
                {
                    // Draw the image on the printer graphics
                    e.Graphics.DrawImage(imageToPrint, e.MarginBounds);
                };

                // Show the print dialog and print if the user confirms
                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.Document = printDocument;

                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        printDocument.Print();
                    }
                }
            }
        }
    }
}