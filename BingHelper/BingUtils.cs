using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingHelper
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
            try
            {
                //Parse the Data without using json implementations.
                string[] strJsonData = jsonString.Split(new char[] { ',' });
                string imageDateTemp = strJsonData[0].Split(new char[] { '"' })[5];
                strJsonData = strJsonData[3].Split(new char[] { '"' });
                string imageUrlTemp = BingUrl + strJsonData[3];

                if (!BingUtils.WebsiteExists(ref imageUrlTemp))
                    imageUrlTemp = BingUrl + strJsonData[2];

                if (BingUtils.WebsiteExists(ref imageUrlTemp))
                {
                    imageData = new BingImageData(imageDateTemp, imageUrlTemp);
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
            //Parse the Data without using json implementations.
            string[] strJsonData = jsonString.Split(new char[] { ',' });
            imageDatas = new BingImageData[strJsonData.Length];

            for (int i = 0; i < strJsonData.Length; i++)
            {
                if (strJsonData[i].StartsWith("\"url\"", StringComparison.Ordinal))
                {
                    string imageDateTemp;
                    if (strJsonData[i - 3].StartsWith("{\"images\"", StringComparison.Ordinal))
                        imageDateTemp = strJsonData[i - 3].Split(new char[] { '"' })[5];
                    else
                        imageDateTemp = strJsonData[i - 3].Split(new char[] { '"' })[3];
                    string[] strJsonData2 = strJsonData[i].Split(new char[] { '"' });
                    string imageUrlTemp = BingUrl + strJsonData2[3];

                    imageDatas[i] = new BingImageData(imageDateTemp, imageUrlTemp);
                }
                else
                {
                    imageDatas[i] = null;
                }
            }
        }
    }
}
