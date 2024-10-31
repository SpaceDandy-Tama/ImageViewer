using System.IO;
using System.Windows.Forms;

using Tiny;

namespace Tama.ImageViewer
{
    [System.Serializable]
    public enum AskOrNoOption
    {
        Ask,
        Always,
        Never,
    }

    [System.Serializable]
    public class AppSetting
    {
        public static AppSetting Current;
        public static string FullPath = AppSetting.GetPath();
        private static string GetPath()
        {
            string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(appDataPath, Application.CompanyName);
            if(!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            return Path.Combine(directoryPath, $"{Application.ProductName}Settings.tiny");
        }

        public bool Fullscreen = true;
        public bool Maximized = false;
        public Vector2Int WindowSize = new Vector2Int(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
        public Vector2Int WindowLocation = new Vector2Int(Screen.PrimaryScreen.Bounds.Width / 4, Screen.PrimaryScreen.Bounds.Height / 4);
        public bool UIVisible = true;
        public bool BingButtonVisible = true;
        public string BingImageSavePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "Bing Images");
        public AskOrNoOption OpenBingImageAfterDownload = AskOrNoOption.Ask;
        public string BingRegion = null;
        public Theme Theme = Theme.Dark;
        public Color CustomBackgroundColor = new Color(88, 22, 44);
        public int ThemeCheckInterval = 1;
        public bool AutoUISize = true;
        public int DesiredUISize = 48;

        public static AppSetting Load()
        {
            if (Current == null)
            {
                if (File.Exists(FullPath))
                {
                    string tiny = File.ReadAllText(FullPath);
                    Current = Deserializer.Deserialize<AppSetting>(tiny);
                    File.Delete(FullPath);
                    if(Current == null)
                    {
                        Current = new AppSetting();
                    }
                }
                else
                {
                    Current = new AppSetting();
                }

                Current.Save();
            }
            return Current;
        }

        public void Save()
        {
            string tiny = Serializer.Serialize(this);
            File.WriteAllText(FullPath, tiny);
        }
    }
}