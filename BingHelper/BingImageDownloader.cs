using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ImageViewer.BingHelper
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

                BingUtils.WriteID3Tag(imagePath, imageData.Copyright);
            }

            OnImageDownloadedAndSaved?.Invoke(new BingImageDownloadedEventArgs(new BingImageData[] { imageData }, new string[] { imagePath }));
        }

        public static async Task DownloadLast8Images(string directory, string strRegion = "en-US")
        {
            string[] imagePaths = new string[8]; //Can't download more than 8 images from bing for some reason

            string jsonString = await BingUtils.GetJsonStringOfImageOfTheDay(imagePaths.Length, strRegion);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            BingImageData[] imageDatas = null;
            BingUtils.ParseMultipleImageJsonString(jsonString, ref imageDatas);
            for (int i = 0; i < imageDatas.Length; i++)
            {
                if (imageDatas[i] == null)
                    continue;

                imagePaths[i] = Path.Combine(directory, imageDatas[i].Date + ".jpg");

                if (File.Exists(imagePaths[i]))
                    continue;

                byte[] bytes = await BingImageDownloader.GetImageOfTheDayData(imageDatas[i]);
                using (FileStream fs = new FileStream(imagePaths[i], FileMode.Create))
                    await fs.WriteAsync(bytes, 0, bytes.Length);

                BingUtils.WriteID3Tag(imagePaths[i], imageDatas[i].Copyright);
            }

            OnImageDownloadedAndSaved?.Invoke(new BingImageDownloadedEventArgs(imageDatas, imagePaths));
        }

        public static async Task<byte[]> GetImageOfTheDayData(BingImageData imageData)
        {
            using (WebClient wc = new WebClient())
                return await wc.DownloadDataTaskAsync(imageData.Url);
        }
    }
}