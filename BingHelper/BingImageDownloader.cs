using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BingHelper
{
    public class BingImageDownloader
    {
        public static event BingImageDownloadedEventHandler OnImageDownloadedAndSaved;

        public static async Task DownloadImageOfTheDay(string directory, string strRegion = "en-US")
        {
            string jsonString = await BingUtils.GetJsonStringOfImageOfTheDay(1, strRegion);

            BingImageData imageData = null;
            if (BingUtils.ParseSingleImageJsonString(jsonString, ref imageData))
                await DownloadImageOfTheDay(directory, imageData);
        }
        public static async Task DownloadImageOfTheDay(string directory, BingImageData imageData, bool abortIfFileExists = true)
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

            BingUtils.WriteID3Tag(imagePath, imageData.Copyright);

            OnImageDownloadedAndSaved?.Invoke(null, new BingImageDownloadedEventArgs(imageData, imagePath));
        }

        public static async Task DownloadLast8Images(string directory, string strRegion = "en-US")
        {
            string jsonString = await BingUtils.GetJsonStringOfImageOfTheDay(8, null);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            BingImageData[] bingImages = null;
            BingUtils.ParseMultipleImageJsonString(jsonString, ref bingImages);
            for (int i = 0; i < bingImages.Length; i++)
            {
                if (bingImages[i] == null)
                    continue;

                string imagePath = Path.Combine(directory, bingImages[i].Date + ".jpg");

                if (File.Exists(imagePath))
                    continue;

                byte[] bytes = await BingImageDownloader.GetImageOfTheDayData(bingImages[i]);
                using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                    await fs.WriteAsync(bytes, 0, bytes.Length);

                BingUtils.WriteID3Tag(imagePath, bingImages[i].Copyright);
            }
        }

        public static async Task<byte[]> GetImageOfTheDayData(BingImageData imageData)
        {
            using (WebClient wc = new WebClient())
                return await wc.DownloadDataTaskAsync(imageData.Url);
        }
    }
}