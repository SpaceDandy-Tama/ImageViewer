using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ArdaBing
{
    public class BingImageDownloader
    {
        public static event BingImageDownloadedEventHandler OnImageDownloadedAndSaved;

        public static async void DownloadImageOfTheDay(string directory, string strRegion = "en-US")
        {
            string jsonString = await BingUtils.GetJsonStringOfImageOfTheDay(1, strRegion);

            BingImageData imageData = null;
            if (BingUtils.ParseSingleImageJsonString(jsonString, ref imageData))
                DownloadImageOfTheDay(directory, imageData);
        }
        public static void DownloadImageOfTheDay(string directory, string imageDate, string imageUrl)
        {
            DownloadImageOfTheDay(directory, new BingImageData(imageDate, imageUrl));
        }
        public static async void DownloadImageOfTheDay(string directory, BingImageData imageData, bool abortIfFileExists = true)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string imagePath = Path.Combine(directory, imageData.Date + ".jpg");

            if (!(abortIfFileExists && File.Exists(imagePath)))
            {
                byte[] bytes = await GetImageOfTheDayData(imageData);

                using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                    await fs.WriteAsync(bytes, 0, bytes.Length);
            }

            OnImageDownloadedAndSaved?.Invoke(null, new BingImageDownloadedEventArgs(imageData, imagePath));
        }

        public static async Task<byte[]> GetImageOfTheDayData(BingImageData imageData)
        {
            using (WebClient wc = new WebClient())
                return await wc.DownloadDataTaskAsync(imageData.Url);
        }
    }
}