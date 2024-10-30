using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Tama.ImageViewer
{
    public static class ImagePrinter
    {
        public static string[] Filters = new string[] { ".bmp", ".jpg", ".jpeg", ".png", ".tiff", ".tif", ".ico" };

        public static void PrintImage(string imagePath)
        {
            // Check if the image file exists
            if (!System.IO.File.Exists(imagePath))
            {
                throw new ArgumentException("Image file does not exist: " + imagePath);
            }

            if(!Helpers.IsExtensionSupported(imagePath, ImagePrinter.Filters))
            {
                MessageBox.Show("Print isn't supported for this file extension", "Tama's ImageViewer");
                return;
            }

            // Load the image
            using (Image imageToPrint = Image.FromFile(imagePath))
            {
                // Create a PrintDocument
                using (PrintDocument printDocument = new PrintDocument())
                {
                    // Variable to keep track of the current page/frame
                    int currentPage = 0;

                    // Determine if the image is a multi-page TIFF
                    bool isMultiPageTiff = imageToPrint.FrameDimensionsList.Length > 0 &&
                                           imageToPrint.GetFrameCount(FrameDimension.Page) > 1;

                    // Set up the PrintPage event
                    printDocument.PrintPage += (sender, e) =>
                    {
                        // Select the current frame from the TIFF image if it's a multi-page TIFF
                        if (isMultiPageTiff)
                        {
                            if (currentPage < imageToPrint.GetFrameCount(FrameDimension.Page))
                            {
                                imageToPrint.SelectActiveFrame(FrameDimension.Page, currentPage);
                                e.Graphics.DrawImage(imageToPrint, e.MarginBounds);
                                currentPage++; // Move to the next frame/page
                                e.HasMorePages = currentPage < imageToPrint.GetFrameCount(FrameDimension.Page);
                            }
                            else
                            {
                                e.HasMorePages = false; // No more pages to print
                            }
                        }
                        else
                        {
                            // For single-page images, just draw the image once
                            e.Graphics.DrawImage(imageToPrint, e.MarginBounds);
                            e.HasMorePages = false; // No more pages to print
                        }
                    };

                    // Set up the Page Setup dialog
                    using (PageSetupDialog pageSetupDialog = new PageSetupDialog())
                    {
                        pageSetupDialog.Document = printDocument;

                        // Show the Page Setup dialog
                        if (pageSetupDialog.ShowDialog() == DialogResult.OK)
                        {
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
        }
    }
}
