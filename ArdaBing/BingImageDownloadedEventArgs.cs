using System;

namespace ArdaBing
{
    public delegate void BingImageDownloadedEventHandler(object sender, BingImageDownloadedEventArgs args);

    public class BingImageDownloadedEventArgs : EventArgs
    {
        public BingImageDownloadedEventArgs(BingImageData imageData, string imagePath)
        {
            ImageData = imageData;
            ImagePath = imagePath;
        }

        public BingImageData ImageData { get; set; }
        public string ImagePath { get; set; }
    }
}