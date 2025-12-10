using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using ImageViewer.Properties;
using ImageViewer.BingHelper;
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
        public bool CanRotate => CurrentImage != null && !Program.CurrentFile.ToLower().EndsWith(".gif") && !Program.CurrentFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase) && !Program.CurrentFile.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) && !Program.CurrentFile.EndsWith(".tif", StringComparison.OrdinalIgnoreCase) && CurrentImage.WEBPFormat?.Length < 1;

        public string[] FilesInDirectory;
        public int CurrentIndex;

        public DisposableImage CurrentImage;
        public bool IsImageDirty;
        public Bitmap TempBitmap; //For zooming purposes
        public int ZoomFactor = 1;
        public int ActivePage = 0;
        public int PageCount;
        public Timer AnimatedWebpTimer;

        public static int[] ButtonSizes = { 48, 64, 80, 96, 128, 160, 192, 256, 320, 416, 512 };

        #region Constructor
        public FormMain()
        {
            InitializeComponent();

            SetParentsOfUIControlsForTransparencyToWork();

            AdjustControlSizes();

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

            AnimatedWebpTimer = new Timer();
            AnimatedWebpTimer.Tick += AnimatedWebpTimer_Tick;
            AnimatedWebpTimer.Enabled = false;

            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            BingImageDownloader.OnImageDownloadedAndSaved += OnImageDownloadedAndSaved;

            if (string.IsNullOrEmpty(Program.CurrentFile))
            {
                BingAction();
            }
            else
            {
                OpenImage();
            }

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
            BingImageDownloader.DownloadImageOfTheDay(AppSetting.Current.BingImageSavePath, AppSetting.Current.BingRegion);
        }

        public void BingActionMultiple()
        {
            BingImageDownloader.DownloadLast8Images(AppSetting.Current.BingImageSavePath, AppSetting.Current.BingRegion);
        }

        public void OnImageDownloadedAndSaved(BingImageDownloadedEventArgs args)
        {
            if (CurrentDir == Path.GetDirectoryName(args.ImagePath[0]))
            {
                RefreshFilesInDirectory();
            }

            if (!string.IsNullOrEmpty(Program.CurrentFile) && AppSetting.Current.OpenBingImageAfterDownload == AskOrNoOption.Never)
            {
                return;
            }

            string message = $"The following image{(args.ImagePath.Length > 1 ? "s" : string.Empty)} have been downloaded and saved:\n";
            for (int i = args.ImagePath.Length - 1; i >= 0; i--)
            {
                message += $"\n\"{args.ImagePath[i]}\"";
            }

            if (!string.IsNullOrEmpty(Program.CurrentFile) && AppSetting.Current.OpenBingImageAfterDownload == AskOrNoOption.Ask)
            {
                message += $"\n\nWould you like to open {(args.ImagePath.Length > 1 ? "the last image" : "it")}?";
                DialogResult result = MessageBox.Show(message, Application.ProductName, MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            Program.CurrentFile = args.ImagePath[0]; //Index increases as image becomes older, so index 0 is the latest image.
            OpenImage();
        }

        public void SaveAction()
        {
            SaveImage();
        }

        public void PrevPageAction()
        {
            if (CurrentImage.WEBPFormat?.Length > 0)
                return;

            ChangeMultiPageImage(ActivePage - 1);
        }

        public void NextPageAction()
        {
            if (CurrentImage.WEBPFormat?.Length > 0)
                return;

            ChangeMultiPageImage(ActivePage + 1);
        }

        public void SettingsAction()
        {
            Helpers.Message("Settings not implemented");
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

        public void RefreshFilesInDirectory()
        {
            List<string> tempList = new List<string>(Directory.GetFiles(CurrentDir));
            List<int> elementsToRemove = new List<int>();

            for (int i = 0; i < tempList.Count; i++)
            {
                if (!Helpers.IsExtensionSupported(tempList[i], ImageDecoder.Filters))
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

        public void OpenImage(bool showDialog = false)
        {
            if(showDialog)
                OpenFileDialog();

            if (!File.Exists(Program.CurrentFile))
                return;

            RefreshDisplayedImage();

            if (CurrentDir != PreviousDir)
            {
                RefreshFilesInDirectory();
            }

            RefreshUI();

            PreviousDir = CurrentDir;
        }

        public void RefreshDisplayedImage()
        {
            CurrentImage?.Dispose();
            TempBitmap?.Dispose();
            pictureBox1.Image?.Dispose();
            ZoomFactor = 1;
            IsImageDirty = false;

            try
            {
                CurrentImage = ImageDecoder.Decode(Program.CurrentFile);
            }
            catch (Exception exception)
            {
#if !DEBUG
                if (exception is DecodingNotSupported)
#endif
                {
                    Helpers.Message(exception.Message);
                    return;
                }
            }

            pictureBox1.Image = CurrentImage;

            if (Program.CurrentFile.EndsWith(".obi", StringComparison.OrdinalIgnoreCase))
            {
                string extra = string.Empty;
                if (CurrentImage.ObiFormat == LibObiNet.PixelFormat.Format8Grayscale)
                    extra = "8-bit Grayscale";
                else if (CurrentImage.ObiFormat == LibObiNet.PixelFormat.Format4Grayscale)
                    extra = "4-bit Grayscale";
                else if (CurrentImage.ObiFormat == LibObiNet.PixelFormat.Format2Grayscale)
                    extra = "2-bit Grayscale";
                else if (CurrentImage.ObiFormat == LibObiNet.PixelFormat.Monochromatic)
                    extra = "Monochromatic";

                extra += $" | {((CurrentImage.ObiFlags & LibObiNet.ObiFlags.RLE) != 0 ? "RLE" : "RAW")}";

                SetFileNameText(CurrentFileName, extra);
            }
            else if (Program.CurrentFile.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            {
                SetFileNameText(CurrentFileName, $"{CurrentImage.TargaPixelDepth} bpp {(CurrentImage.TargaRLE ? "RLE" : "RAW")}");
            }
            else if (Program.CurrentFile.EndsWith(".dds", StringComparison.OrdinalIgnoreCase))
            {
                SetFileNameText(CurrentFileName, CurrentImage.DDSPixelFormat.ToString());
            }
            else if(Program.CurrentFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                SetFileNameText(CurrentFileName, CurrentImage.WEBPFormat);
            }
            else
            {
                if(Program.CurrentFile.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    FrameDimension dimension = new FrameDimension(CurrentImage.FrameDimensionsList[0]);
                    SetFileNameText(CurrentFileName, $"{CurrentImage.GetFrameCount(dimension)} frames total");
                }
                else if (Program.CurrentFile.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase) || Program.CurrentFile.EndsWith(".tif", StringComparison.OrdinalIgnoreCase))
                {
                    SetFileNameText(CurrentFileName, $"{ActivePage + 1}/{CurrentImage.GetFrameCount(FrameDimension.Page)} pages");
                }
                else
                {
                    SetFileNameText(CurrentFileName);
                }
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
            else
            {
                SetCopyrightText(null);
            }

            ChangeMultiPageImage(0);

            if (CurrentImage.Width > this.Width || CurrentImage.Height > this.Height)
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

            if(CurrentImage.WEBPFormat?.Length > 0 && CurrentImage.GetFrameCount(FrameDimension.Page) > 1)
            {
                AnimatedWebpTimer.Interval = CurrentImage.WEBPFrameDuration / 10;
                AnimatedWebpTimer.Enabled = true;
            }
            else
            {
                AnimatedWebpTimer.Enabled = false;
            }
        }

        private void AnimatedWebpTimer_Tick(object sender, EventArgs e)
        {
            ChangeMultiPageImage(ActivePage + 1);
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

            if (CurrentImage.WEBPFormat?.Length > 0 && i >= PageCount)
                i = 0;

            if (i < 0 || i >= PageCount)
                return;

            CurrentImage.SelectActiveFrame(FrameDimension.Page, i);
            ActivePage = i;
            pictureBox1.Image = CurrentImage;

            if(CurrentImage.WEBPFormat == null || CurrentImage.WEBPFormat.Length < 1)
                SetFileNameText(CurrentFileName, String.Format("Page {0} of {1}", i + 1, PageCount));
        }

        private void SetFileNameText(string name, string extra = null)
		{
            //fileNameText
            this.Text = name;
            fileNameText.Text = $"{name}{(string.IsNullOrEmpty(extra) ? string.Empty : $" ({extra})")}";
            fileNameBox.Size = fileNameText.Size;
        }

        private void SetCopyrightText(string copyright)
        {
            if (copyright != null && copyright.Length > 0)
            {
                copyrightText.Text = copyright;
                Size size = Size.Empty;
                size.Width = fileNameText.Width > copyrightText.Width ? fileNameText.Width : copyrightText.Width;
                size.Height = AppSetting.Current.DesiredUISize;
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

                if (TempBitmap != null)
                    TempBitmap.Dispose();

                return;
            }
            else
                ZoomFactor += direction;

            if(TempBitmap != null)
                TempBitmap.Dispose();

            int width = pictureBox1.Width / ZoomFactor;
            int height = pictureBox1.Height / ZoomFactor;
            Size size = new Size(width, height);

            Point point;
            if (x < 0 && y < 0)
                point = new Point((CurrentImage.Width / 2) - (width / 2), (CurrentImage.Height / 2) - (height / 2));
            else
                point = new Point(x - (width / 2), y - (height / 2));

            TempBitmap =  new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Graphics bmGraphics = Graphics.FromImage(TempBitmap);

            bmGraphics.Clear(this.BackColor);

            bmGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle dstRect = new Rectangle(Point.Empty, size);
            Rectangle srcRect = new Rectangle(point, size);
            bmGraphics.DrawImage(CurrentImage, dstRect, srcRect, GraphicsUnit.Pixel);

            pictureBox1.Image = TempBitmap;
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
            if ((e.Alt && e.KeyCode == Keys.Enter) || (e.Control && e.Shift && e.KeyCode == Keys.F))
            {
                FullscreenAction();
                return;
            }
            else if (e.Control && e.KeyCode == Keys.B)
            {
                BingActionMultiple();
                return;
            }
            else if (e.Control && e.KeyCode == Keys.O)
            {
                OpenImage(true);
                return;
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                PrintAction();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Escape: //Quit
                    Application.Exit();
                    break;
                case Keys.Right: //Next Image in the CurrentDir
                    NextImage();
                    break;
                case Keys.Left: //Previous Image in the CurrentDir
                    PreviousImage();
                    break;
                case Keys.Add: //ZoomIn
                    ZoomDisplayedImage(1);
                    break;
                case Keys.Subtract: //ZoomOut
                    ZoomDisplayedImage(-1);
                    break;
                case Keys.NumPad1:
                    ChangeScreen(0);
                    break;
                case Keys.NumPad2:
                    ChangeScreen(1);
                    break;
                case Keys.NumPad3:
                    ChangeScreen(2);
                    break;
                case Keys.NumPad4:
                    ChangeScreen(3);
                    break;
                case Keys.NumPad5:
                    ChangeScreen(4);
                    break;
                case Keys.NumPad6:
                    ChangeScreen(5);
                    break;
                case Keys.PageUp:
                    PrevPageAction();
                    break;
                case Keys.PageDown:
                    NextPageAction();
                    break;
                case Keys.Up:
                    PrevPageAction();
                    break;
                case Keys.Down:
                    NextPageAction();
                    break;
                case Keys.Space:
                    NextPageAction();
                    break;
                case Keys.B:
                    BingAction();
                    break;
                case Keys.F11:
                    FullscreenAction();
                    break;
            }
        }

        private void SetParentsOfUIControlsForTransparencyToWork()
        {
            //Set the parents for the UI Elements. Otherwise their transparency doesn't work. From front to back.
            arrowLeft.Parent = pictureBox1;
            arrowRight.Parent = pictureBox1;
            rotateLeft.Parent = pictureBox1;
            rotateRight.Parent = pictureBox1;
            bing.Parent = pictureBox1;
            save.Parent = pictureBox1;
            fullscreen.Parent = pictureBox1;
            prevPage.Parent = pictureBox1;
            nextPage.Parent = pictureBox1;
            settings.Parent = pictureBox1;
            print.Parent = pictureBox1;
            file.Parent = pictureBox1;

            fileNameBox.Parent = pictureBox1;
            fileNameText.Parent = fileNameBox;
            copyrightText.Parent = fileNameBox;
        }

        public int FindClosestButtonSize(int targetSize)
        {
            int closestSize = ButtonSizes[0];
            int smallestDifference = Math.Abs(ButtonSizes[0] - targetSize);

            foreach (int size in ButtonSizes)
            {
                int difference = Math.Abs(size - targetSize);
                if (difference < smallestDifference)
                {
                    smallestDifference = difference;
                    closestSize = size;
                }
            }

            return closestSize;
        }

        public int CalculateOptimumButtonSize()
        {
            int resolutionSum = Screen.PrimaryScreen.Bounds.Width + Screen.PrimaryScreen.Bounds.Height;
            int magicNumber = 42;
            return (int)Math.Round((double)resolutionSum / magicNumber);
        }

        private void AdjustControlSizes()
        {
            if (AppSetting.Current.AutoUISize)
            {
                AppSetting.Current.DesiredUISize = CalculateOptimumButtonSize();
            }
            AppSetting.Current.DesiredUISize = FindClosestButtonSize(AppSetting.Current.DesiredUISize);

            Size size = new Size(AppSetting.Current.DesiredUISize, AppSetting.Current.DesiredUISize);

            //BottomLeftButtonsFromLeftToRight
            arrowLeft.Size = size;
            arrowRight.Size = size;
            rotateLeft.Size = size;
            rotateRight.Size = size;
            save.Size = size;
            arrowLeft.Location = new Point(0, this.ClientSize.Height - size.Height);
            arrowRight.Location = new Point(size.Width, this.ClientSize.Height - size.Height);
            rotateLeft.Location = new Point(size.Width * 2, this.ClientSize.Height - size.Height);
            rotateRight.Location = new Point(size.Width * 3, this.ClientSize.Height - size.Height);
            save.Location = new Point(size.Width * 4, this.ClientSize.Height - size.Height);

            //BottomRightButtonsFromRightToLeft
            fullscreen.Size = size;
            settings.Size = size;
            file.Size = size;
            print.Size = size;
            fullscreen.Location = new Point(this.ClientSize.Width - size.Width, this.ClientSize.Height - size.Height);
            settings.Location = new Point(this.ClientSize.Width - size.Width * 2, this.ClientSize.Height - size.Height);
            file.Location = new Point(this.ClientSize.Width - size.Width * 3, this.ClientSize.Height - size.Height);
            print.Location = new Point(this.ClientSize.Width - size.Width * 4, this.ClientSize.Height - size.Height);

            //TopRightButtonsFromRightToLeft
            bing.Size = size;
            bing.Location = new Point(this.ClientSize.Width - size.Width, 0);
            
            //RightButtonsFromTopToBottom
            prevPage.Size = size;
            nextPage.Size = size;
            prevPage.Location = new Point(this.ClientSize.Width - size.Width, this.ClientSize.Height / 2 - size.Height / 2);
            nextPage.Location = new Point(this.ClientSize.Width - size.Width, this.ClientSize.Height / 2 + size.Height / 2);

            //OtherControls
            fileNameBox.Size = size;
            fileNameText.Font = new Font("Segoe UI", size.Height / 4, FontStyle.Bold); //AutoSize controls Size property
            copyrightText.Font = new Font("Segoe UI", size.Height / 4, FontStyle.Regular); //AutoSize controls Size property
            fileNameBox.Location = new Point(0, 0);
            fileNameText.Location = new Point(0, 0);
            copyrightText.Location = new Point(0, size.Height / 2);
        }

        private void RefreshUI()
        {
            if (AppSetting.Current.UIVisible)
            {
                fileNameBox.Visible = true;
                //Todo: Instead of making buttons invisible, disable them.
                arrowLeft.Visible = FilesInDirectory != null && FilesInDirectory.Length > 1;
                arrowRight.Visible = FilesInDirectory != null && FilesInDirectory.Length > 1;
                rotateLeft.Visible = CanRotate;
                rotateRight.Visible = CanRotate;
                fullscreen.Visible = true;
                bing.Visible = AppSetting.Current.BingButtonVisible;
                save.Visible = IsImageDirty;
                prevPage.Visible = PageCount > 1 && CurrentImage.WEBPFormat.Length < 1;
                nextPage.Visible = PageCount > 1 && CurrentImage.WEBPFormat.Length < 1;
                settings.Visible = true;
                print.Visible = true;
                file.Visible = true;
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
                file.Visible = false;
            }
        }

        private void RotateImage(bool clockwise = true)
        {
            if (CurrentImage == null)
                return;

            if(CanRotate == false)
            {
                Helpers.Message("Sorry, can't rotate this type of file");
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
            DialogResult result =  MessageBox.Show($"Are you sure you want to overwrite {Program.CurrentFile}?", Application.ProductName, MessageBoxButtons.OKCancel);

            if(result == DialogResult.Cancel)
            {
                return;
            }

            ImageEncoderQuality quality = null;
            if (Program.CurrentFile.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
            {
                quality = new ImageEncoderQuality() { Quality = 40, WebpComplexity = WebPEncodingComplexity.NearLossless };
            }
            ImageEncoder.Encode(CurrentImage, Program.CurrentFile, quality);

            IsImageDirty = false;
            RefreshUI();
        }

        private void NextImage()
        {
            if (FilesInDirectory.Length > 1 && Directory.Exists(CurrentDir))
            {
                if (CurrentIndex >= FilesInDirectory.Length - 1)
                    return;

                if (Helpers.IsExtensionSupported(FilesInDirectory[CurrentIndex + 1], ImageDecoder.Filters))
                {
                    CurrentIndex += 1;
                    Program.CurrentFile = FilesInDirectory[CurrentIndex];
                    RefreshDisplayedImage();
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

                if (Helpers.IsExtensionSupported(FilesInDirectory[CurrentIndex - 1], ImageDecoder.Filters))
                {
                    CurrentIndex -= 1;
                    Program.CurrentFile = FilesInDirectory[CurrentIndex];
                    RefreshDisplayedImage();
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

            AnimatedWebpTimer.Tick -= AnimatedWebpTimer_Tick;
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
            save.Image = Resources.floppyHover;
        }
        private void save_MouseDown(object sender, MouseEventArgs e)
        {
            save.Image = Resources.floppyDown;
        }
        private void save_MouseLeave(object sender, EventArgs e)
        {
            save.Image = Resources.floppy;
        }
        private void save_MouseUp(object sender, MouseEventArgs e)
        {
            save.Image = Resources.floppy;
            SaveAction();
        }

        private void prevPage_MouseEnter(object sender, EventArgs e)
        {
            prevPage.Image = Resources.arrowUpHover;
        }
        private void prevPage_MouseDown(object sender, MouseEventArgs e)
        {
            prevPage.Image = Resources.arrowUpDown;
        }
        private void prevPage_MouseLeave(object sender, EventArgs e)
        {
            prevPage.Image = Resources.arrowUp;
        }
        private void prevPage_MouseUp(object sender, MouseEventArgs e)
        {
            prevPage.Image = Resources.arrowUp;
            PrevPageAction();
        }

        private void nextPage_MouseEnter(object sender, EventArgs e)
        {
            nextPage.Image = Resources.arrowDownHover;
        }
        private void nextPage_MouseDown(object sender, MouseEventArgs e)
        {
            nextPage.Image = Resources.arrowDownDown;
        }
        private void nextPage_MouseLeave(object sender, EventArgs e)
        {
            nextPage.Image = Resources.arrowDown;
        }
        private void nextPage_MouseUp(object sender, MouseEventArgs e)
        {
            nextPage.Image = Resources.arrowDown;
            NextPageAction();
        }

        private void settings_MouseEnter(object sender, EventArgs e)
        {
            settings.Image = Resources.cogHover;
        }
        private void settings_MouseDown(object sender, MouseEventArgs e)
        {
            settings.Image = Resources.cogDown;
        }
        private void settings_MouseLeave(object sender, EventArgs e)
        {
            settings.Image = Resources.cog;
        }
        private void settings_MouseUp(object sender, MouseEventArgs e)
        {
            settings.Image = Resources.cog;
            SettingsAction();
        }

        private void print_MouseEnter(object sender, EventArgs e)
        {
            print.Image = Resources.printerHover;
        }
        private void print_MouseDown(object sender, MouseEventArgs e)
        {
            print.Image = Resources.printerDown;
        }
        private void print_MouseLeave(object sender, EventArgs e)
        {
            print.Image = Resources.printer;
        }
        private void print_MouseUp(object sender, MouseEventArgs e)
        {
            print.Image = Resources.printer;
            PrintAction();
        }

        private void file_MouseEnter(object sender, EventArgs e)
        {
            file.Image = Resources.fileHover;
        }
        private void file_MouseDown(object sender, MouseEventArgs e)
        {
            file.Image = Resources.fileDown;
        }
        private void file_MouseLeave(object sender, EventArgs e)
        {
            file.Image = Resources.file;
        }
        private void file_MouseUp(object sender, MouseEventArgs e)
        {
            file.Image = Resources.file;
            OpenImage(true);
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

                    if(Helpers.IsExtensionSupported(file, ImageDecoder.Filters))
                    {
                        Program.CurrentFile = file;
                        OpenImage();
                    }
                }
            }
        }
    }
}
