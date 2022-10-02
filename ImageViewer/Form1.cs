using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Net;
using BingHelper;

namespace ImageViewer
{
    public partial class Form1 : Form
    {
        public OpenFileDialog Ofd;
        public string Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png, *.tiff, *.tif, *.gif, *.tga, *.dds)|*.bmp;*.jpg;*.jepg;*.png;*.tiff;*.tif;*.gif;*.tga;.dds";
        public string[] Filters = new string[]{".bmp", ".jpg", ".jpeg", ".png", ".tiff", "tif", ".gif", ".tga", ".dds" };

        public string CurrentFile;
        public string CurrentDir;
        
        public string[] OtherFiles;
        public int CurrentIndex;

        public Image CurrentImage;
        public Image TempImage; //For zooming purposes
        public int ZoomFactor = 1;
        public int ActivePage = 0;

        public bool Fullscreen;
        public bool HideUi;

        public string AppSettingPath = "ImageViewerSettings.json";
        //public string AppSettingPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"ImageViewer"), "AppSetting.json");

        public DataContractJsonSerializer Ser;
        public AppSetting CurAppSetting;
        public string SaveFilePathBing;

        public bool SilentBingInvoked = false;
        public int SilentBingNumberOfImages = 8; //Bing API doesn't seem to return more than 8 images
        public bool ArdaVirusInvoked = false;
        public bool SaveFilePathBingInvoked = false;
        public string SaveFilePathBingOverride = null;
        public bool NoCommandInvoked = false;

        #region Constructor
        public Form1(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "-silentBing")
                {
                    SilentBingInvoked = true;
                    SilentBingNumberOfImages = 1;
                }
                else if (args[0].StartsWith("-silentBingMultiple", StringComparison.Ordinal))
                {
                    SilentBingInvoked = true;
                }
                else if(args[0].StartsWith("-saveFilePathBing=", StringComparison.Ordinal))
                {
                    SaveFilePathBingInvoked = true;
                    SaveFilePathBingOverride = args[0].Split('=')[1];
                }
                else
                {
                    string[] tempFileName = args[0].Split('\\');
                    if (tempFileName[tempFileName.Length - 1] == "DesktopJoke.png")
                    {
                        ArdaVirusInvoked = true;
                    }
                }
            }
            else
            {
                NoCommandInvoked = true;
            }

            InitializeComponent();

            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string fileName = AppSettingPath;
            AppSettingPath = Path.Combine(appDataPath, "ArdaSuite");
            if (!Directory.Exists(AppSettingPath))
                Directory.CreateDirectory(AppSettingPath);
            AppSettingPath = Path.Combine(AppSettingPath, fileName);

            //Set the parents for the UI buttons. Otherwise their transparency doesn't work.
            fileNameBox.Parent = pictureBox1;
            fileNameText.Parent = fileNameBox;

            arrowLeft.Parent = pictureBox1;
            arrowRight.Parent = pictureBox1;
            rotateLeft.Parent = pictureBox1;
            rotateRight.Parent = pictureBox1;
            fullscreen.Parent = pictureBox1;
            bing.Parent = pictureBox1;

            if (LoadAppSetting())
            {
                Fullscreen = CurAppSetting.fullscreen;
                HideUi = CurAppSetting.hideUi;
                if (Fullscreen)
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    if (CurAppSetting.maximized)
                        GoMaximised();
                }
                this.Width = CurAppSetting.size.X;
                this.Height = CurAppSetting.size.Y;
                this.Location = CurAppSetting.location;
                if (HideUi)
                    ShowHide(true);
                SaveFilePathBing = CurAppSetting.savePathBingImage;
            }
            else
            {
                HideUi = false;
                GoDefault();
                SaveFilePathBing = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bing Images");
            }

            this.BackColor = Color.Black;
            this.KeyPreview = true;

            Ofd = new OpenFileDialog();
            Ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (args.Length > 0)
            {
                CurrentFile = args[0];
                OpenImage();
            }
        }
        #endregion

        #region Starting Screen Types
        private void GoDefault()
        {
            Fullscreen = false;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Width = Screen.PrimaryScreen.Bounds.Width / 2;
            this.Height = Screen.PrimaryScreen.Bounds.Height / 2;
            this.Location = new Point(Width / 2, Height / 2);
            this.WindowState = FormWindowState.Normal;
        }

        private void GoFullScreen()
        {
            Fullscreen = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new Point(0, 0);
            this.WindowState = FormWindowState.Normal;
        }

        private void ChangeScreen(int screenIndex)
        {
            if (!Fullscreen)
                return;

            if (screenIndex > Screen.AllScreens.Length - 1)
                return;

            this.Width = Screen.AllScreens[screenIndex].Bounds.Width;
            this.Height = Screen.AllScreens[screenIndex].Bounds.Height;
            this.Location = Screen.AllScreens[screenIndex].Bounds.Location;
        }

        private void GoMaximised()
        {
            Fullscreen = false;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;
        }
        #endregion

        #region App Settings
        [DataContract]
        public class AppSetting
        {
            [DataMember]
            public bool rememberLastSettings;

            [DataMember]
            public bool fullscreen;

            [DataMember]
            public bool maximized;

            [DataMember]
            public Point size;

            [DataMember]
            public Point location;

            [DataMember]
            public bool hideUi;

            [DataMember]
            public bool enableBingFeature;

            [DataMember]
            public bool enableBingUi;

            [DataMember]
            public string savePathBingImage;
        }

        private void SaveAppSetting()
        {
            if (CurAppSetting != null && CurAppSetting.rememberLastSettings == false)
                return;

            AppSetting appSetting = new AppSetting();
            appSetting.rememberLastSettings = true;
            appSetting.fullscreen = Fullscreen;
            appSetting.size = new Point(this.Width, this.Height);
            appSetting.location = this.Location;
            appSetting.hideUi = HideUi;
            appSetting.maximized = this.WindowState == FormWindowState.Maximized;
            if (CurAppSetting == null)
            {
                appSetting.enableBingFeature = true;
                appSetting.enableBingUi = true;
            }
            else
            {
                appSetting.enableBingFeature = CurAppSetting.enableBingFeature;
                appSetting.enableBingUi = CurAppSetting.enableBingUi;
            }
            appSetting.savePathBingImage = SaveFilePathBing;

            MemoryStream memoryStream = new MemoryStream();
            Ser.WriteObject(memoryStream, appSetting);
            using (FileStream fileStream = File.Create(AppSettingPath))
            {
                memoryStream.Position = 0;
                memoryStream.CopyTo(fileStream);
            }
            memoryStream.Dispose();
        }

        private bool LoadAppSetting()
        {
            Ser = new DataContractJsonSerializer(typeof(AppSetting));
            if (File.Exists(AppSettingPath))
            {
                using (FileStream fileStream = File.OpenRead(AppSettingPath))
                {
                    CurAppSetting = (AppSetting)Ser.ReadObject(fileStream);
                    if (CurAppSetting.enableBingFeature == false || CurAppSetting.enableBingUi == false)
                        bing.Visible = false;
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region User Interface Button Actions

        public void ArrowLeftAction()
        {
            PreviousImage();
        }
        public void ArrowRightAction()
        {
            NextImage();
        }
        public void RotateLeftAction()
        {
            RotateImage(false);
        }
        public void RotateRightAction()
        {
            RotateImage(true);
        }
        public void FullscreenAction()
        {
            if (Fullscreen)
                GoDefault();
            else
                GoFullScreen();
        }
        public void BingAction()
        {
            RetrieveBingImageOfTheDay();
        }
        public void BingActionMultiple()
        {
            RetrieveBingImageOfTheDayMultiple();
        }
        #endregion

        public string GetSafeName(string fileName) => Path.GetFileName(fileName);

        public void OpenFileDialog()
        {
            Ofd.Multiselect = false;
            Ofd.Filter = Filter;
            if (Ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            CurrentFile = Ofd.FileName;
        }

        public void OpenImage(bool showDialog = false)
        {
            if(showDialog)
                OpenFileDialog();

            if (!File.Exists(CurrentFile))
                return;

            RefleshDisplayedImage();

            CurrentDir = Path.GetDirectoryName(CurrentFile);
            List<string> tempList = new List<string>(Directory.GetFiles(CurrentDir));
            List<int> elementsToRemove = new List<int>();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (!FileIsImageType(tempList[i]))
                    elementsToRemove.Add(i);
            }

            for (int i = elementsToRemove.Count - 1; i > -1; i--)
			{
                tempList.RemoveAt(elementsToRemove[i]);               
            }

            OtherFiles = tempList.ToArray();

            for (int i = 0; i < OtherFiles.Length; i++)
            {
                if (OtherFiles[i] == CurrentFile)
                {
                    CurrentIndex = i;
                }
            }            
        }

        public bool FileIsImageType(string fileName)
        {
            for (int i = 0; i < Filters.Length; i++)
            {
                if (fileName.ToLower().EndsWith(Filters[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void RefleshDisplayedImage()
        {
            CurrentImage?.Dispose();
            TempImage?.Dispose();
            ZoomFactor = 1;

            if (CurrentFile.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            {
                Paloma.TargaImage targaImage = new Paloma.TargaImage(CurrentFile);
                CurrentImage = Paloma.TargaImage.CopyToBitmap(targaImage);
                pictureBox1.Image = CurrentImage;
                SetFileNameText(GetSafeName(CurrentFile), String.Format("{0} bpp {1}", targaImage.Header.PixelDepth, targaImage.Header.IsRLE ? "RLE" : "RAW"));
                targaImage.Dispose();
            }
            else if (CurrentFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
            {
                Imaging.DDSReader.DDSImage ddsImage = Imaging.DDSReader.DDS.LoadImage(CurrentFile);
                CurrentImage = Imaging.DDSReader.DDS.CopyToBitmap(ddsImage);
                pictureBox1.Image = CurrentImage;
                SetFileNameText(GetSafeName(CurrentFile), ddsImage.PixelFormat.ToString());
                ddsImage.Dispose();
            }
            else
            {
                CurrentImage = Image.FromFile(CurrentFile);
                pictureBox1.Image = CurrentImage;
                SetFileNameText(GetSafeName(CurrentFile));
            }

            ChangeMultiPageImage(0);

            if (CurrentImage.Width > this.Width || CurrentImage.Height > this.Height)
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        public void ChangeMultiPageImage(int i)
        {
            if (CurrentFile.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                return;

            int pageCount = CurrentImage.GetFrameCount(FrameDimension.Page);
            
            if (pageCount == 1)
			{
                ActivePage = 0;
                return;
			}

            if (i < 0 || i >= pageCount)
                return;

            CurrentImage.SelectActiveFrame(FrameDimension.Page, i);
            ActivePage = i;
            pictureBox1.Image = CurrentImage;
            SetFileNameText(GetSafeName(CurrentFile), String.Format("Page {0} of {1}", i + 1, pageCount));
        }

        private void SetFileNameText(string name, string extra = null)
		{
            this.Text = ArdaVirusInvoked ? "Desktop" : name;
            fileNameText.Text = name;
            if (extra != null && extra.Length > 0)
                fileNameText.Text += String.Format(" ({0})", extra);
            fileNameBox.Size = fileNameText.Size;
        }
       
        public void ZoomDisplayedImage(int direction, int x = -1, int y = -1)
        {
            if (ArdaVirusInvoked)
                return;

            if (CurrentImage == null)
                return;

            if (CurrentFile.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                return;

            if (direction < 0 && ZoomFactor + direction <= 1)
            {
                ZoomFactor = 1;

                pictureBox1.Image = CurrentImage;

                if (CurrentImage.Width > this.Width || CurrentImage.Height > this.Height)
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                else
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                if (TempImage != null)
                    TempImage.Dispose();

                return;
            }
            else
                ZoomFactor += direction;

            if(TempImage != null)
                TempImage.Dispose();

            int width = pictureBox1.Width / ZoomFactor;
            int height = pictureBox1.Height / ZoomFactor;
            Size size = new Size(width, height);

            Point point;
            if (x < 0 && y < 0)
                point = new Point((CurrentImage.Width / 2) - (width / 2), (CurrentImage.Height / 2) - (height / 2));
            else
                point = new Point(x - (width / 2), y - (height / 2));

            TempImage =  new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Graphics bmGraphics = Graphics.FromImage(TempImage);

            bmGraphics.Clear(Color.Black);

            bmGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle dstRect = new Rectangle(Point.Empty, size);
            Rectangle srcRect = new Rectangle(point, size);
            bmGraphics.DrawImage(CurrentImage, dstRect, srcRect, GraphicsUnit.Pixel);

            pictureBox1.Image = TempImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            bmGraphics.Dispose();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ArdaVirusInvoked)
                return;

            if (e.Button == MouseButtons.Left)  //Enable-Disable UI
                ShowHide();
            else if (e.Button == MouseButtons.Right) //Quit
                Application.Exit();
            else if (e.Button == MouseButtons.XButton1) //Previous Image in the CurrentDir
                PreviousImage();
            else if (e.Button == MouseButtons.XButton2) //Next Image in the CurrentDir
                NextImage();
            else if (e.Button == MouseButtons.Middle) // Zoom to 100%
                ZoomDisplayedImage(-ZoomFactor);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ArdaVirusInvoked)
                return;

            int direction = e.Delta / Math.Abs(e.Delta);

            ZoomDisplayedImage(direction, e.X, e.Y);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ArdaVirusInvoked)
                return;

            switch (e.KeyCode)
            {
                case Keys.Escape: //Quit
                    Application.Exit();
                    return;
                case Keys.Right: //Next Image in the CurrentDir
                    NextImage();
                    return;
                case Keys.Left: //Previous Image in the CurrentDir
                    PreviousImage();
                    return;
                case Keys.Add: //ZoomIn
                    ZoomDisplayedImage(1);
                    return;
                case Keys.Subtract: //ZoomOut
                    ZoomDisplayedImage(-1);
                    return;
                case Keys.NumPad1:
                    ChangeScreen(0);
                    return;
                case Keys.NumPad2:
                    ChangeScreen(1);
                    return;
                case Keys.NumPad3:
                    ChangeScreen(2);
                    return;
                case Keys.NumPad4:
                    ChangeScreen(3);
                    return;
                case Keys.NumPad5:
                    ChangeScreen(4);
                    return;
                case Keys.NumPad6:
                    ChangeScreen(5);
                    return;
                case Keys.PageUp:
                    ChangeMultiPageImage(ActivePage-1);
                    return;
                case Keys.PageDown:
                    ChangeMultiPageImage(ActivePage+1);
                    return;
                case Keys.B: //Bing Image of the Day
                    RetrieveBingImageOfTheDay();
                    return;
            }

            if (e.Alt && e.KeyCode == Keys.Enter)
                FullscreenAction();
            else if (e.Control && e.KeyCode == Keys.B)
                RetrieveBingImageOfTheDayMultiple();
            else if (e.Control && e.KeyCode == Keys.O)
                OpenImage(true);
        }

        private void ShowHide(bool justApply = false)
        {
            if (!justApply)
                HideUi = !HideUi;

            if (HideUi)
            {
                fileNameBox.Visible = false;
                arrowLeft.Visible = false;
                arrowRight.Visible = false;
                rotateLeft.Visible = false;
                rotateRight.Visible = false;
                fullscreen.Visible = false;
                bing.Visible = false;
            }
            else
            {
                fileNameBox.Visible = true;
                arrowLeft.Visible = true;
                arrowRight.Visible = true;
                rotateLeft.Visible = true;
                rotateRight.Visible = true;
                fullscreen.Visible = true;
                if (CurAppSetting != null) {
                    if(CurAppSetting.enableBingFeature && CurAppSetting.enableBingUi)
                        bing.Visible = true;
                }
                else
                {
                    bing.Visible = true;
                }
            }
        }

        private void RotateImage(bool clockwise = true)
        {
            if (CurrentImage == null || CurrentFile.ToLower().EndsWith(".gif") || CurrentFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase) || CurrentFile.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) || CurrentFile.EndsWith(".tif", StringComparison.OrdinalIgnoreCase))
                return;

            if(clockwise)
                CurrentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            else
                CurrentImage.RotateFlip(RotateFlipType.Rotate270FlipNone);

            pictureBox1.Image = CurrentImage;

            if(!(CurrentFile.EndsWith(".tga", StringComparison.OrdinalIgnoreCase) ))
                CurrentImage.Save(CurrentFile);
        }

        private void NextImage()
        {
            if (Directory.Exists(CurrentDir)) {
                if (CurrentIndex >= OtherFiles.Length - 1)
                    return;
                if (FileIsImageType(OtherFiles[CurrentIndex + 1]))
                {
                    CurrentIndex += 1;
                    CurrentFile = OtherFiles[CurrentIndex];
                    RefleshDisplayedImage();
                }
            }
        }

        private void PreviousImage()
        {
            if (Directory.Exists(CurrentDir))
            {
                if (CurrentIndex <= 0)
                    return;
                if (FileIsImageType(OtherFiles[CurrentIndex - 1]))
                {
                    CurrentIndex -= 1;
                    CurrentFile = OtherFiles[CurrentIndex];
                    RefleshDisplayedImage();
                }
            }
        }

        #region BingStuff
        private void RetrieveBingImageOfTheDay()
        {
            if (CurAppSetting != null && CurAppSetting.enableBingFeature == false)
                return;

            BingImageDownloader.OnImageDownloadedAndSaved += BingImageDownloader_OnImageDownloadedAndSaved;

            BingImageDownloader.DownloadImageOfTheDay(SaveFilePathBing, null);
        }

        private void BingImageDownloader_OnImageDownloadedAndSaved(object sender, BingImageDownloadedEventArgs args)
        {
            if (SilentBingInvoked)
                Application.Exit();
            else
            {
                CurrentFile = args.ImagePath;
                OpenImage();
            }
        }

        private async void RetrieveBingImageOfTheDayMultiple()
        {
            string jsonString = await BingUtils.GetJsonStringOfImageOfTheDay(SilentBingNumberOfImages, null);

#if DEBUG
            using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "jsonStringDump.json")))
                await sw.WriteAsync(jsonString);
#endif

            if (!Directory.Exists(SaveFilePathBing))
                Directory.CreateDirectory(SaveFilePathBing);

            BingImageData[] bingImages = null;
            BingUtils.ParseMultipleImageJsonString(jsonString, ref bingImages);
            for (int i = 0; i < bingImages.Length; i++)
            {
                if (bingImages[i] == null)
                    continue;

                string imagePath = Path.Combine(SaveFilePathBing, bingImages[i].Date + ".jpg");

                if (File.Exists(imagePath))
                    continue;

                byte[] bytes = await BingImageDownloader.GetImageOfTheDayData(bingImages[i]);
                using (FileStream fs = new FileStream(imagePath, FileMode.Create))
                    await fs.WriteAsync(bytes, 0, bytes.Length);
            }

            if (SilentBingInvoked)
                Application.Exit();
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (SilentBingInvoked)
                return;

            SaveAppSetting();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (NoCommandInvoked)
                BingAction();
            else if (SilentBingInvoked)
            {
                this.Hide();
                if (SilentBingNumberOfImages == 1)
                    BingAction();
                else
                    BingActionMultiple();
            }
            else if (SaveFilePathBingInvoked)
            {
                this.Hide();
                SaveFilePathBing = SaveFilePathBingOverride;
                Application.Exit();
            }
        }

        #region User Interface Button Events
        private void fullscreen_MouseEnter(object sender, EventArgs e)
        {
            fullscreen.Image = Properties.Resources.fullscreenHover;
        }
        private void fullscreen_MouseDown(object sender, MouseEventArgs e)
        {
            fullscreen.Image = Properties.Resources.fullscreenDown;
        }
        private void fullscreen_MouseLeave(object sender, EventArgs e)
        {
            fullscreen.Image = Properties.Resources.fullscreen;
        }
        private void fullscreen_MouseUp(object sender, MouseEventArgs e)
        {
            fullscreen.Image = Properties.Resources.fullscreen;
            FullscreenAction();
        }

        private void rotateRight_MouseEnter(object sender, EventArgs e)
        {
            rotateRight.Image = Properties.Resources.rotateRightHover;
        }
        private void rotateRight_MouseDown(object sender, MouseEventArgs e)
        {
            rotateRight.Image = Properties.Resources.rotateRightDown;
        }
        private void rotateRight_MouseLeave(object sender, EventArgs e)
        {
            rotateRight.Image = Properties.Resources.rotateRight;
        }
        private void rotateRight_MouseUp(object sender, MouseEventArgs e)
        {
            rotateRight.Image = Properties.Resources.rotateRight;
            RotateRightAction();
        }

        private void rotateLeft_MouseEnter(object sender, EventArgs e)
        {
            rotateLeft.Image = Properties.Resources.rotateLeftHover;
        }
        private void rotateLeft_MouseDown(object sender, MouseEventArgs e)
        {
            rotateLeft.Image = Properties.Resources.rotateLeftDown;
        }
        private void rotateLeft_MouseLeave(object sender, EventArgs e)
        {
            rotateLeft.Image = Properties.Resources.rotateLeft;
        }
        private void rotateLeft_MouseUp(object sender, MouseEventArgs e)
        {
            rotateLeft.Image = Properties.Resources.rotateLeft;
            RotateLeftAction();
        }

        private void arrowRight_MouseEnter(object sender, EventArgs e)
        {
            arrowRight.Image = Properties.Resources.arrowRightHover;
        }
        private void arrowRight_MouseDown(object sender, MouseEventArgs e)
        {
            arrowRight.Image = Properties.Resources.arrowRightDown;
        }
        private void arrowRight_MouseLeave(object sender, EventArgs e)
        {
            arrowRight.Image = Properties.Resources.arrowRight;
        }
        private void arrowRight_MouseUp(object sender, MouseEventArgs e)
        {
            arrowRight.Image = Properties.Resources.arrowRight;
            ArrowRightAction();
        }

        private void arrowLeft_MouseEnter(object sender, EventArgs e)
        {
            arrowLeft.Image = Properties.Resources.arrowLeftHover;
        }
        private void arrowLeft_MouseDown(object sender, MouseEventArgs e)
        {
            arrowLeft.Image = Properties.Resources.arrowLeftDown;
        }
        private void arrowLeft_MouseLeave(object sender, EventArgs e)
        {
            arrowLeft.Image = Properties.Resources.arrowLeft;
        }
        private void arrowLeft_MouseUp(object sender, MouseEventArgs e)
        {
            arrowLeft.Image = Properties.Resources.arrowLeft;
            ArrowLeftAction();
        }

        private void bing_MouseEnter(object sender, EventArgs e)
        {
            bing.Image = Properties.Resources.bingHover;
        }
        private void bing_MouseDown(object sender, MouseEventArgs e)
        {
            bing.Image = Properties.Resources.bingDown;
        }
        private void bing_MouseLeave(object sender, EventArgs e)
        {
            bing.Image = Properties.Resources.bing;
        }
        private void bing_MouseUp(object sender, MouseEventArgs e)
        {
            bing.Image = Properties.Resources.bing;
            BingAction();
        }

        #endregion

    }
}
