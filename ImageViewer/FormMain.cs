using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using ImageViewer.Properties;
using BingHelper;
using WebPWrapper;
using Imaging.DDSReader;
using Paloma;

namespace Tama.ImageViewer
{
    public partial class FormMain : Form
    {
        private string PreviousDir;
        public string CurrentDir => Path.GetDirectoryName(Program.CurrentFile);
        public string CurrentFileName => Path.GetFileName(Program.CurrentFile);
        public bool CanRotate => CurrentImage != null && !Program.CurrentFile.ToLower().EndsWith(".gif") && !Program.CurrentFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase) && !Program.CurrentFile.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) && !Program.CurrentFile.EndsWith(".tif", StringComparison.OrdinalIgnoreCase);

        public string[] FilesInDirectory;
        public int CurrentIndex;

        public Image CurrentImage;
        public bool IsImageDirty;
        public Image TempImage; //For zooming purposes
        public int ZoomFactor = 1;
        public int ActivePage = 0;
        public int PageCount;

        #region Constructor
        public FormMain()
        {
            InitializeComponent();

            //Set the parents for the UI buttons. Otherwise their transparency doesn't work.
            fileNameBox.Parent = pictureBox1;
            fileNameText.Parent = fileNameBox;
            copyrightText.Parent = fileNameBox;

            arrowLeft.Parent = pictureBox1;
            arrowRight.Parent = pictureBox1;
            rotateLeft.Parent = pictureBox1;
            rotateRight.Parent = pictureBox1;
            fullscreen.Parent = pictureBox1;
            bing.Parent = pictureBox1;

            this.Icon = Resources.icon;
            this.KeyPreview = true;
            this.StartPosition = FormStartPosition.Manual; //This is required for being able to set this.Location

            if (ApplyTheme() && AppSetting.Current.ThemeCheckInterval > 0)
            {
                Timer timer = new Timer();
                timer.Interval = Math.Max(Math.Min(5, AppSetting.Current.ThemeCheckInterval), 1) * 1000;
                timer.Tick += Timer_Tick;
                timer.Start();
            }

            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            if (string.IsNullOrEmpty(Program.CurrentFile))
            {
                //Todo: remove this line and display a sample image, perhaps sample image could contain instructions
#if DEBUG
                throw new NotImplementedException();
#endif
            }
            OpenImage();

            ApplyWindowSettings();
        }
#endregion

        private void ApplyWindowSettings()
        {
            if (AppSetting.Current.Fullscreen == true)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Normal;

                this.Width = Screen.PrimaryScreen.Bounds.Width;
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.Location = new Point(0, 0);
            }
            else
            {
                //It is crucial the following code inside this scope are done in this exact order, otherwise weird things happen to windows forms.

                //Set the Borders to Resizable
                this.FormBorderStyle = FormBorderStyle.Sizable;
                //Set window state to maximized if Appsettings say they are maximized, or to normal windowed mode if not
                this.WindowState = AppSetting.Current.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;

                //Set the window size and location from appsetting regardless of maximized or normal, this is great for when coming out of maximized
                //Because this code will not execute again when user decides to come out of maximize mode, there is no event for maximize.
                this.Width = AppSetting.Current.WindowSize.x;
                this.Height = AppSetting.Current.WindowSize.y;
                this.Location = new Point(AppSetting.Current.WindowLocation.x, AppSetting.Current.WindowLocation.y);
            }

            RefreshUI();
        }
        private void ChangeScreen(int screenIndex)
        {
            if (AppSetting.Current.Fullscreen == false || screenIndex > Screen.AllScreens.Length - 1)
                return;

            this.Width = Screen.AllScreens[screenIndex].Bounds.Width;
            this.Height = Screen.AllScreens[screenIndex].Bounds.Height;
            this.Location = Screen.AllScreens[screenIndex].Bounds.Location;
        }

        private bool ApplyTheme()
        {
            bool result = false;

            //Determine what Theme the user wants
            bool isCustom = AppSetting.Current.Theme == Theme.CustomColor;
            bool isLight = true;
            if (!isCustom)
            {
                if (AppSetting.Current.Theme == Theme.Default)
                {
                    isLight = Helpers.IsLightMode();

                    //Modify AppSetting if below windows 10, because only win10 and higher support light/dark themes
                    if (!Helpers.IsWindows10OrHigher())
                    {
                        AppSetting.Current.Theme = isLight ? Theme.Light : Theme.Dark;
                    }
                    else
                    {
                        result = true;
                    }
                }
                else if (AppSetting.Current.Theme == Theme.Dark)
                {
                    isLight = false;
                }
            }

            //Adjust background color
            if (isCustom)
            {
                this.BackColor = AppSetting.Current.CustomBackgroundColor.ToSystemDrawingColor();
            }
            else
            {
                if (isLight)
                {
                    this.BackColor = System.Drawing.Color.WhiteSmoke;
                }
                else
                {
                    this.BackColor = System.Drawing.Color.Black;
                }
            }

            return result;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ApplyTheme();
        }

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
            if(AppSetting.Current.Fullscreen)
            {
                AppSetting.Current.Fullscreen = false;
            }
            else
            {
                AppSetting.Current.WindowSize = new Vector2Int(this.Width, this.Height);
                AppSetting.Current.WindowLocation = new Vector2Int(this.Location.X, this.Location.Y);
                AppSetting.Current.Fullscreen = true;
            }

            ApplyWindowSettings();
        }
        public void BingAction()
        {
            BingImageDownloader.OnImageDownloadedAndSaved += (object sender, BingImageDownloadedEventArgs args) =>
            {
                Program.CurrentFile = args.ImagePath;
                OpenImage();
            };
            BingImageDownloader.DownloadImageOfTheDay(AppSetting.Current.BingImageSavePath, null);
        }

        public void BingActionMultiple()
        {
            BingImageDownloader.DownloadLast8Images(AppSetting.Current.BingImageSavePath, null);
        }

        public void SaveAction()
        {
            SaveImage();
        }

        public void PrevPageAction()
        {
            ChangeMultiPageImage(ActivePage - 1);
        }

        public void NextPageAction()
        {
            ChangeMultiPageImage(ActivePage + 1);
        }

        public void SettingsAction()
        {
#if DEBUG
            throw new NotImplementedException();
#else
            MessageBox.Show("Not Implemented");
#endif
        }

        public void PrintAction()
        {
            ImagePrinter.PrintImage(Program.CurrentFile);
        }
        #endregion

        public void OpenFileDialog()
        {
            Program.Ofd.Multiselect = false;
            Program.Ofd.Filter = Program.Filter;
            if (Program.Ofd.ShowDialog() != DialogResult.OK)
                return;
            Program.CurrentFile = Program.Ofd.FileName;
        }

        public void OpenImage(bool showDialog = false)
        {
            if(showDialog)
                OpenFileDialog();

            if (!File.Exists(Program.CurrentFile))
                return;

            RefleshDisplayedImage();

            if (CurrentDir != PreviousDir)
            {

                List<string> tempList = new List<string>(Directory.GetFiles(CurrentDir));
                List<int> elementsToRemove = new List<int>();

                for (int i = 0; i < tempList.Count; i++)
                {
                    if (!Helpers.IsExtensionSupported(tempList[i], Program.Filters))
                        elementsToRemove.Add(i);
                }

                for (int i = elementsToRemove.Count - 1; i > -1; i--)
                {
                    tempList.RemoveAt(elementsToRemove[i]);
                }

                FilesInDirectory = tempList.ToArray();

                for (int i = 0; i < FilesInDirectory.Length; i++)
                {
                    if (FilesInDirectory[i] == Program.CurrentFile)
                    {
                        CurrentIndex = i;
                    }
                }
            }

            RefreshUI();

            PreviousDir = CurrentDir;
        }

        public void RefleshDisplayedImage()
        {
            CurrentImage?.Dispose();
            TempImage?.Dispose();
            ZoomFactor = 1;

            if (Program.CurrentFile.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            {
                Paloma.TargaImage targaImage = new Paloma.TargaImage(Program.CurrentFile);
                CurrentImage = Paloma.TargaImage.CopyToBitmap(targaImage);
                pictureBox1.Image = CurrentImage;
                SetFileNameText(CurrentFileName, String.Format("{0} bpp {1}", targaImage.Header.PixelDepth, targaImage.Header.IsRLE ? "RLE" : "RAW"));
                targaImage.Dispose();
            }
            else if (Program.CurrentFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
            {
                Imaging.DDSReader.DDSImage ddsImage = Imaging.DDSReader.DDS.LoadImage(Program.CurrentFile);
                CurrentImage = Imaging.DDSReader.DDS.CopyToBitmap(ddsImage);
                pictureBox1.Image = CurrentImage;
                SetFileNameText(CurrentFileName, ddsImage.PixelFormat.ToString());
                ddsImage.Dispose();
            }
            else if(Program.CurrentFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                byte[] rawWebp = File.ReadAllBytes(Program.CurrentFile);
                using (WebP webp = new WebP())
                {
                    CurrentImage = webp.Decode(rawWebp);
                    pictureBox1.Image = CurrentImage;

                    int width;
                    int height;
                    bool hasAlpha;
                    bool hasAnimation;
                    string format;
                    webp.GetInfo(rawWebp, out width, out height, out hasAlpha, out hasAnimation, out format);

                    SetFileNameText(CurrentFileName, $"{format}");
                }
            }
            else
            {
                CurrentImage = Image.FromFile(Program.CurrentFile);
                pictureBox1.Image = CurrentImage;
                SetFileNameText(CurrentFileName);
            }

            if (Program.CurrentFile.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                string copyright = null;

                TagLib.Id3v2.Tag.DefaultVersion = 3;
                TagLib.Id3v2.Tag.ForceDefaultVersion = true;
                using (TagLib.File tagFile = TagLib.File.Create(Program.CurrentFile, TagLib.ReadStyle.None))
                {
                    tagFile.Mode = TagLib.File.AccessMode.Read;
                    copyright = tagFile.Tag.Copyright;
                }

                SetCopyrightText(copyright);
            }

            ChangeMultiPageImage(0);

            if (CurrentImage.Width > this.Width || CurrentImage.Height > this.Height)
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        public void ChangeMultiPageImage(int i)
        {
            if (Program.CurrentFile.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
            {
                PageCount = 1;
                return;
            }
                
            PageCount = CurrentImage.GetFrameCount(FrameDimension.Page);
            
            if (PageCount == 1)
			{
                ActivePage = 0;
                return;
			}

            if (i < 0 || i >= PageCount)
                return;

            CurrentImage.SelectActiveFrame(FrameDimension.Page, i);
            ActivePage = i;
            pictureBox1.Image = CurrentImage;
            SetFileNameText(CurrentFileName, String.Format("Page {0} of {1}", i + 1, PageCount));
        }

        private void SetFileNameText(string name, string extra = null, string copyright = null, string copyrightUrl = null)
		{
            //fileNameText
            this.Text = name;
            fileNameText.Text = name;
            if (extra != null && extra.Length > 0)
                fileNameText.Text += String.Format(" ({0})", extra);
            fileNameBox.Size = fileNameText.Size;
        }

        private void SetCopyrightText(string copyright)
        {
            if (copyright != null && copyright.Length > 0)
            {
                copyrightText.Text = copyright;
                Size size = Size.Empty;
                size.Width = fileNameText.Width > copyrightText.Width ? fileNameText.Width : copyrightText.Width;
                size.Height = fileNameText.Height + copyrightText.Height;
                fileNameBox.Size = size;
            }
            else
            {
                copyrightText.Text = "";
                fileNameBox.Size = fileNameText.Size;
            }
        }
       
        public void ZoomDisplayedImage(int direction, int x = -1, int y = -1)
        {
            if (CurrentImage == null)
                return;

            if (Program.CurrentFile.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
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

            bmGraphics.Clear(this.BackColor);

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
            if (e.Button == MouseButtons.Left) //Enable-Disable UI
            {
                AppSetting.Current.UIVisible = !AppSetting.Current.UIVisible;
                RefreshUI();
            }
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
            int direction = e.Delta / Math.Abs(e.Delta);

            ZoomDisplayedImage(direction, e.X, e.Y);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
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
                    PrevPageAction();
                    return;
                case Keys.PageDown:
                    NextPageAction();
                    return;
                case Keys.B:
                    BingAction();
                    return;
                case Keys.F11:
                    FullscreenAction();
                    return;
            }

            if ((e.Alt && e.KeyCode == Keys.Enter) && (e.Control && e.Shift && e.KeyCode == Keys.F))
            {
                FullscreenAction();
            }
            else if (e.Control && e.KeyCode == Keys.B)
            {
                BingActionMultiple();
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                OpenImage(true);
            }
            else if(e.Control && e.KeyCode == Keys.P)
            {
                PrintAction();
            }
        }

        private void RefreshUI()
        {
            if (AppSetting.Current.UIVisible)
            {
                fileNameBox.Visible = true;
                //Todo: Instead of making buttons invisible, disable them.
                arrowLeft.Visible = FilesInDirectory.Length > 1;
                arrowRight.Visible = FilesInDirectory.Length > 1;
                rotateLeft.Visible = CanRotate;
                rotateRight.Visible = CanRotate;
                fullscreen.Visible = true;
                bing.Visible = AppSetting.Current.BingButtonVisible;
                save.Visible = IsImageDirty;
                prevPage.Visible = PageCount > 1;
                nextPage.Visible = PageCount > 1;
                settings.Visible = true;
                print.Visible = true;
            }
            else
            {
                fileNameBox.Visible = false;
                arrowLeft.Visible = false;
                arrowRight.Visible = false;
                rotateLeft.Visible = false;
                rotateRight.Visible = false;
                fullscreen.Visible = false;
                bing.Visible = false;
                save.Visible = false;
                prevPage.Visible = false;
                nextPage.Visible = false;
                settings.Visible = false;
                print.Visible = false;
            }
        }

        private void RotateImage(bool clockwise = true)
        {
            if (CurrentImage == null)
                return;

            if(CanRotate == false)
            {
                MessageBox.Show("Sorry, can't rotate this type of file", "Tama's Image Viewer");
                return;
            }

            if(clockwise)
                CurrentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
            else
                CurrentImage.RotateFlip(RotateFlipType.Rotate270FlipNone);

            pictureBox1.Image = CurrentImage;

            IsImageDirty = true;
            RefreshUI();
        }

        private void SaveImage()
        {
            if(!Helpers.IsExtensionSupported(Program.CurrentFile, Program.Filters))
            {
                MessageBox.Show("File extension not supported", "Tama's Image Viewer");
                return;
            }

            if (Program.CurrentFile.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Can't save .gif files", "Tama's Image Viewer");
            }
            else if (Program.CurrentFile.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Can't save .tiff files", "Tama's Image Viewer");
            }
            else if (Program.CurrentFile.EndsWith(".tif", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Can't save .tif files", "Tama's Image Viewer");
            }
            else if (Program.CurrentFile.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Can't save .tga files", "Tama's Image Viewer");
            }
            else if (Program.CurrentFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Can't save .dds files", "Tama's Image Viewer");
            }
            else if (Program.CurrentFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                using (WebP webp = new WebP())
                {
                    byte[] rawWebP = webp.EncodeLossless((Bitmap)CurrentImage);
                    File.WriteAllBytes(Program.CurrentFile, rawWebP);
                }
            }
            else
            {
                CurrentImage.Save(Program.CurrentFile);
            }

            IsImageDirty = false;
            RefreshUI();
        }

        private void NextImage()
        {
            if (FilesInDirectory.Length > 1 && Directory.Exists(CurrentDir))
            {
                if (CurrentIndex >= FilesInDirectory.Length - 1)
                    return;

                if (Helpers.IsExtensionSupported(FilesInDirectory[CurrentIndex + 1], Program.Filters))
                {
                    CurrentIndex += 1;
                    Program.CurrentFile = FilesInDirectory[CurrentIndex];
                    RefleshDisplayedImage();
                    RefreshUI();
                }
            }
        }

        private void PreviousImage()
        {
            if (FilesInDirectory.Length > 1 && Directory.Exists(CurrentDir))
            {
                if (CurrentIndex <= 0)
                    return;

                if (Helpers.IsExtensionSupported(FilesInDirectory[CurrentIndex - 1], Program.Filters))
                {
                    CurrentIndex -= 1;
                    Program.CurrentFile = FilesInDirectory[CurrentIndex];
                    RefleshDisplayedImage();
                    RefreshUI();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save the Maximized State of Window
            AppSetting.Current.Maximized = this.WindowState == FormWindowState.Maximized;

            //Save WindowSize and Location only when in windowed mode to ensure consistent windowing memory.
            if (!AppSetting.Current.Fullscreen && !AppSetting.Current.Maximized)
            {
                AppSetting.Current.WindowSize = new Vector2Int(this.Width, this.Height);
                AppSetting.Current.WindowLocation = new Vector2Int(this.Location.X, this.Location.Y);
            }

            AppSetting.Current.Save();
        }

#region User Interface Button Events
        private void fullscreen_MouseEnter(object sender, EventArgs e)
        {
            fullscreen.Image = Resources.fullscreenHover;
        }
        private void fullscreen_MouseDown(object sender, MouseEventArgs e)
        {
            fullscreen.Image = Resources.fullscreenDown;
        }
        private void fullscreen_MouseLeave(object sender, EventArgs e)
        {
            fullscreen.Image = Resources.fullscreen;
        }
        private void fullscreen_MouseUp(object sender, MouseEventArgs e)
        {
            fullscreen.Image = Resources.fullscreen;
            FullscreenAction();
        }

        private void rotateRight_MouseEnter(object sender, EventArgs e)
        {
            rotateRight.Image = Resources.rotateRightHover;
        }
        private void rotateRight_MouseDown(object sender, MouseEventArgs e)
        {
            rotateRight.Image = Resources.rotateRightDown;
        }
        private void rotateRight_MouseLeave(object sender, EventArgs e)
        {
            rotateRight.Image = Resources.rotateRight;
        }
        private void rotateRight_MouseUp(object sender, MouseEventArgs e)
        {
            rotateRight.Image = Resources.rotateRight;
            RotateRightAction();
        }

        private void rotateLeft_MouseEnter(object sender, EventArgs e)
        {
            rotateLeft.Image = Resources.rotateLeftHover;
        }
        private void rotateLeft_MouseDown(object sender, MouseEventArgs e)
        {
            rotateLeft.Image = Resources.rotateLeftDown;
        }
        private void rotateLeft_MouseLeave(object sender, EventArgs e)
        {
            rotateLeft.Image = Resources.rotateLeft;
        }
        private void rotateLeft_MouseUp(object sender, MouseEventArgs e)
        {
            rotateLeft.Image = Resources.rotateLeft;
            RotateLeftAction();
        }

        private void arrowRight_MouseEnter(object sender, EventArgs e)
        {
            arrowRight.Image = Resources.arrowRightHover;
        }
        private void arrowRight_MouseDown(object sender, MouseEventArgs e)
        {
            arrowRight.Image = Resources.arrowRightDown;
        }
        private void arrowRight_MouseLeave(object sender, EventArgs e)
        {
            arrowRight.Image = Resources.arrowRight;
        }
        private void arrowRight_MouseUp(object sender, MouseEventArgs e)
        {
            arrowRight.Image = Resources.arrowRight;
            ArrowRightAction();
        }

        private void arrowLeft_MouseEnter(object sender, EventArgs e)
        {
            arrowLeft.Image = Resources.arrowLeftHover;
        }
        private void arrowLeft_MouseDown(object sender, MouseEventArgs e)
        {
            arrowLeft.Image = Resources.arrowLeftDown;
        }
        private void arrowLeft_MouseLeave(object sender, EventArgs e)
        {
            arrowLeft.Image = Resources.arrowLeft;
        }
        private void arrowLeft_MouseUp(object sender, MouseEventArgs e)
        {
            arrowLeft.Image = Resources.arrowLeft;
            ArrowLeftAction();
        }

        private void bing_MouseEnter(object sender, EventArgs e)
        {
            bing.Image = Resources.bingHover;
        }
        private void bing_MouseDown(object sender, MouseEventArgs e)
        {
            bing.Image = Resources.bingDown;
        }
        private void bing_MouseLeave(object sender, EventArgs e)
        {
            bing.Image = Resources.bing;
        }
        private void bing_MouseUp(object sender, MouseEventArgs e)
        {
            bing.Image = Resources.bing;
            BingAction();
        }

        private void save_MouseEnter(object sender, EventArgs e)
        {
            save.Image = Resources.blankHover;
        }
        private void save_MouseDown(object sender, MouseEventArgs e)
        {
            save.Image = Resources.blankDown;
        }
        private void save_MouseLeave(object sender, EventArgs e)
        {
            save.Image = Resources.blank;
        }
        private void save_MouseUp(object sender, MouseEventArgs e)
        {
            save.Image = Resources.blank;
            SaveAction();
        }

        private void prevPage_MouseEnter(object sender, EventArgs e)
        {
            prevPage.Image = Resources.blankHover;
        }
        private void prevPage_MouseDown(object sender, MouseEventArgs e)
        {
            prevPage.Image = Resources.blankDown;
        }
        private void prevPage_MouseLeave(object sender, EventArgs e)
        {
            prevPage.Image = Resources.blank;
        }
        private void prevPage_MouseUp(object sender, MouseEventArgs e)
        {
            prevPage.Image = Resources.blank;
            PrevPageAction();
        }

        private void nextPage_MouseEnter(object sender, EventArgs e)
        {
            nextPage.Image = Resources.blankHover;
        }
        private void nextPage_MouseDown(object sender, MouseEventArgs e)
        {
            nextPage.Image = Resources.blankDown;
        }
        private void nextPage_MouseLeave(object sender, EventArgs e)
        {
            nextPage.Image = Resources.blank;
        }
        private void nextPage_MouseUp(object sender, MouseEventArgs e)
        {
            nextPage.Image = Resources.blank;
            NextPageAction();
        }

        private void settings_MouseEnter(object sender, EventArgs e)
        {
            settings.Image = Resources.blankHover;
        }
        private void settings_MouseDown(object sender, MouseEventArgs e)
        {
            settings.Image = Resources.blankDown;
        }
        private void settings_MouseLeave(object sender, EventArgs e)
        {
            settings.Image = Resources.blank;
        }
        private void settings_MouseUp(object sender, MouseEventArgs e)
        {
            settings.Image = Resources.blank;
            SettingsAction();
        }

        private void print_MouseEnter(object sender, EventArgs e)
        {
            print.Image = Resources.blankHover;
        }
        private void print_MouseDown(object sender, MouseEventArgs e)
        {
            print.Image = Resources.blankDown;
        }
        private void print_MouseLeave(object sender, EventArgs e)
        {
            print.Image = Resources.blank;
        }
        private void print_MouseUp(object sender, MouseEventArgs e)
        {
            print.Image = Resources.blank;
            PrintAction();
        }

        #endregion

        private void onDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void onDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if(files != null && files.Length > 0)
                {
                    string file = files[0];

                    if(Helpers.IsExtensionSupported(file, Program.Filters))
                    {
                        Program.CurrentFile = file;
                        OpenImage();
                    }
                    else
                    {
                        MessageBox.Show("File extension not supported", "Tama's ImageViewer");
                    }
                }
            }
        }
    }
}
