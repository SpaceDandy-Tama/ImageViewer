using System;

namespace ImageViewer.BingHelper
{
    public delegate void BingImageDownloadedEventHandler(BingImageDownloadedEventArgs args);

    public class BingImageDownloadedEventArgs : EventArgs
    {
        public BingImageDownloadedEventArgs(BingImageData[] imageDatas, string[] imagePaths)
        {
            ImageData = imageDatas;
            ImagePath = imagePaths;
        }

        public BingImageData[] ImageData { get; set; }
        public string[] ImagePath { get; set; }
    }
}