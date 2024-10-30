namespace ImageViewer.BingHelper
{
    public class BingImageData
    {
        public string Date = null;
        public string Url = null;
        public string Copyright = null;
        public string CopyrightLink = null;

        public BingImageData(string date, string url, string copyright, string copyrightLink)
        {
            Date = date;
            Url = url;
            Copyright = copyright;
            CopyrightLink = copyrightLink;
        }
    }
}
