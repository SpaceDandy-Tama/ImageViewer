using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ImageViewer.BingHelper
{
    public static class BingUtils
    {
        public const string BingUrl = "http://www.bing.com";

        public static bool WebsiteExists(ref string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> WebsiteExists(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                    return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<string> GetJsonStringOfImageOfTheDay(int numOfImages = 1, string strRegion = "en-US")
        {
            string strBingImageURL;
            if(strRegion == null)
                strBingImageURL = string.Format("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n={0}", numOfImages);
            else
                strBingImageURL = string.Format("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n={0}&mkt={1}", numOfImages, strRegion);

            using (HttpClient client = new HttpClient())
            {
                // Using an Async call makes sure the app is responsive during the time the response is fetched.
                // GetAsync sends an Async GET request to the Specified URI.
                using (HttpResponseMessage response = await client.GetAsync(new Uri(strBingImageURL)))
                {

                    // Content property get or sets the content of a HTTP response message. 
                    // ReadAsStringAsync is a method of the HttpContent which asynchronously 
                    // reads the content of the HTTP Response and returns as a string.
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public static bool ParseSingleImageJsonString(string jsonString, ref BingImageData imageData)
        {
#if DEBUG
            System.IO.File.WriteAllText("singleJson.json", jsonString);
#endif

            try
            {
                //Parse the Data without using json implementations.
                string[] strJsonData = jsonString.Split(new char[] { ',' });
                string imageDateTemp = strJsonData[0].Split(new char[] { '"' })[5];
                strJsonData = strJsonData[3].Split(new char[] { '"' });
                string imageUrlTemp = BingUrl + strJsonData[3];

                strJsonData = jsonString.Split(new char[] { '"' });
                string copyrightTemp = strJsonData[25];
                string copyrightLinkTemp = strJsonData[29];

                if (!BingUtils.WebsiteExists(ref imageUrlTemp))
                    imageUrlTemp = BingUrl + strJsonData[2];

#if DEBUG
                System.IO.File.WriteAllText("data.txt", imageDateTemp);
                System.IO.File.AppendAllText("data.txt", "\n" + imageUrlTemp);
                System.IO.File.AppendAllText("data.txt", "\n" + copyrightTemp);
                System.IO.File.AppendAllText("data.txt", "\n" + copyrightLinkTemp);
#endif

                if (BingUtils.WebsiteExists(ref imageUrlTemp))
                {
                    imageData = new BingImageData(imageDateTemp, imageUrlTemp, copyrightTemp, copyrightLinkTemp);
                    return true;
                }
                else
                {
                    throw new InvalidBingUrlException(imageUrlTemp);
                }
            }
#if DEBUG
            catch(Exception e)
            {
                throw new Exception(e.Message, e);
            }
#else
            catch
            {
                return false;
            }
#endif
        }

        public static void ParseMultipleImageJsonString(string jsonString, ref BingImageData[] imageDatas)
        {

#if DEBUG
            System.IO.File.WriteAllText("multipleJson.json", jsonString);
#endif

            //Parse the Data without using json implementations.
            string[] strJsonData = jsonString.Split(new char[] { ',' });

            int counter = 0;
            for (int i = 0; i < strJsonData.Length; i++)
            {
                if (strJsonData[i].StartsWith("\"url\"", StringComparison.Ordinal))
                {
                    counter++;
                }
            }

            imageDatas = new BingImageData[counter];

            string[] strJsonData2 = jsonString.Split(new string[] { "\"copyright\":\"" }, StringSplitOptions.None);
            string[] strJsonData3 = jsonString.Split(new string[] { "\"copyrightlink\":\"" }, StringSplitOptions.None);

            counter = 0;
            for (int i = 0; i < strJsonData.Length; i++)
            {
                if (strJsonData[i].StartsWith("\"url\"", StringComparison.Ordinal))
                {
                    string imageDateTemp;
                    if (strJsonData[i - 3].StartsWith("{\"images\"", StringComparison.Ordinal))
                        imageDateTemp = strJsonData[i - 3].Split(new char[] { '"' })[5];
                    else
                        imageDateTemp = strJsonData[i - 3].Split(new char[] { '"' })[3];
                    string imageUrlTemp = BingUrl + strJsonData[i].Split(new char[] { '"' })[3];

                    string copyrightTemp = strJsonData2[counter + 1].Split(new string[] { "\",\"copyrightlink\"" }, StringSplitOptions.None)[0];
                    string copyrightLinkTemp = strJsonData3[counter + 1].Split(new string[] { "\",\"title\"" }, StringSplitOptions.None)[0];
#if DEBUG
                    System.IO.File.WriteAllText(imageDateTemp + ".txt", copyrightTemp);
                    System.IO.File.AppendAllText(imageDateTemp + ".txt", "\n" + copyrightLinkTemp);
#endif
                    imageDatas[counter] = new BingImageData(imageDateTemp, imageUrlTemp, copyrightTemp, copyrightLinkTemp);
                    counter++;
                }
            }
        }

        public static bool CheckIfID3TagExists(string imagePath)
        {
            TagLib.Id3v2.Tag.DefaultVersion = 3;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;
            TagLib.Id3v2.Tag.DefaultEncoding = TagLib.StringType.UTF8;
            TagLib.Id3v2.Tag.ForceDefaultEncoding = true;

            TagLib.File tagFile = TagLib.File.Create(imagePath, TagLib.ReadStyle.None);
            bool result = tagFile.TagTypes != TagLib.TagTypes.None;
            tagFile.Dispose();
            return result;
        }

        public static void WriteID3Tag(string imagePath, string copyrightText)
        {
            //ID3 Tag Writing
            TagLib.Id3v2.Tag.DefaultVersion = 3;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;
            TagLib.Id3v2.Tag.DefaultEncoding = TagLib.StringType.UTF8;
            TagLib.Id3v2.Tag.ForceDefaultEncoding = true;

            using (TagLib.File tagFile = TagLib.File.Create(imagePath, TagLib.ReadStyle.None))
            {
                tagFile.Mode = TagLib.File.AccessMode.Write;
                TagLib.Tag tag = tagFile.GetTag(TagLib.TagTypes.XMP, true);
                tag.Copyright = copyrightText;
                tag.SetInfoTag();
                tagFile.Save();
            }
        }
    }
}
