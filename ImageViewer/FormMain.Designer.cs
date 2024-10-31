namespace Tama.ImageViewer
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.arrowLeft = new System.Windows.Forms.PictureBox();
            this.arrowRight = new System.Windows.Forms.PictureBox();
            this.rotateLeft = new System.Windows.Forms.PictureBox();
            this.rotateRight = new System.Windows.Forms.PictureBox();
            this.fullscreen = new System.Windows.Forms.PictureBox();
            this.bing = new System.Windows.Forms.PictureBox();
            this.fileNameBox = new System.Windows.Forms.PictureBox();
            this.fileNameText = new System.Windows.Forms.Label();
            this.copyrightText = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.PictureBox();
            this.prevPage = new System.Windows.Forms.PictureBox();
            this.nextPage = new System.Windows.Forms.PictureBox();
            this.settings = new System.Windows.Forms.PictureBox();
            this.print = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullscreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.save)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.prevPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.print)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(453, 268);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.onDragDrop);
            this.pictureBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.onDragEnter);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // arrowLeft
            // 
            this.arrowLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.arrowLeft.BackColor = System.Drawing.Color.Transparent;
            this.arrowLeft.Image = global::ImageViewer.Properties.Resources.arrowLeft;
            this.arrowLeft.Location = new System.Drawing.Point(64, 204);
            this.arrowLeft.Name = "arrowLeft";
            this.arrowLeft.Size = new System.Drawing.Size(64, 64);
            this.arrowLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.arrowLeft.TabIndex = 1;
            this.arrowLeft.TabStop = false;
            this.arrowLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.arrowLeft_MouseDown);
            this.arrowLeft.MouseEnter += new System.EventHandler(this.arrowLeft_MouseEnter);
            this.arrowLeft.MouseLeave += new System.EventHandler(this.arrowLeft_MouseLeave);
            this.arrowLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.arrowLeft_MouseUp);
            // 
            // arrowRight
            // 
            this.arrowRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.arrowRight.BackColor = System.Drawing.Color.Transparent;
            this.arrowRight.Image = global::ImageViewer.Properties.Resources.arrowRight;
            this.arrowRight.Location = new System.Drawing.Point(128, 204);
            this.arrowRight.Name = "arrowRight";
            this.arrowRight.Size = new System.Drawing.Size(64, 64);
            this.arrowRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.arrowRight.TabIndex = 1;
            this.arrowRight.TabStop = false;
            this.arrowRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.arrowRight_MouseDown);
            this.arrowRight.MouseEnter += new System.EventHandler(this.arrowRight_MouseEnter);
            this.arrowRight.MouseLeave += new System.EventHandler(this.arrowRight_MouseLeave);
            this.arrowRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.arrowRight_MouseUp);
            // 
            // rotateLeft
            // 
            this.rotateLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rotateLeft.BackColor = System.Drawing.Color.Transparent;
            this.rotateLeft.Image = global::ImageViewer.Properties.Resources.rotateLeft;
            this.rotateLeft.Location = new System.Drawing.Point(192, 204);
            this.rotateLeft.Name = "rotateLeft";
            this.rotateLeft.Size = new System.Drawing.Size(64, 64);
            this.rotateLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.rotateLeft.TabIndex = 1;
            this.rotateLeft.TabStop = false;
            this.rotateLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rotateLeft_MouseDown);
            this.rotateLeft.MouseEnter += new System.EventHandler(this.rotateLeft_MouseEnter);
            this.rotateLeft.MouseLeave += new System.EventHandler(this.rotateLeft_MouseLeave);
            this.rotateLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rotateLeft_MouseUp);
            // 
            // rotateRight
            // 
            this.rotateRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rotateRight.BackColor = System.Drawing.Color.Transparent;
            this.rotateRight.Image = global::ImageViewer.Properties.Resources.rotateRight;
            this.rotateRight.Location = new System.Drawing.Point(256, 204);
            this.rotateRight.Name = "rotateRight";
            this.rotateRight.Size = new System.Drawing.Size(64, 64);
            this.rotateRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.rotateRight.TabIndex = 1;
            this.rotateRight.TabStop = false;
            this.rotateRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rotateRight_MouseDown);
            this.rotateRight.MouseEnter += new System.EventHandler(this.rotateRight_MouseEnter);
            this.rotateRight.MouseLeave += new System.EventHandler(this.rotateRight_MouseLeave);
            this.rotateRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rotateRight_MouseUp);
            // 
            // fullscreen
            // 
            this.fullscreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fullscreen.BackColor = System.Drawing.Color.Transparent;
            this.fullscreen.Image = global::ImageViewer.Properties.Resources.fullscreen;
            this.fullscreen.Location = new System.Drawing.Point(389, 204);
            this.fullscreen.Name = "fullscreen";
            this.fullscreen.Size = new System.Drawing.Size(64, 64);
            this.fullscreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.fullscreen.TabIndex = 1;
            this.fullscreen.TabStop = false;
            this.fullscreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fullscreen_MouseDown);
            this.fullscreen.MouseEnter += new System.EventHandler(this.fullscreen_MouseEnter);
            this.fullscreen.MouseLeave += new System.EventHandler(this.fullscreen_MouseLeave);
            this.fullscreen.MouseUp += new System.Windows.Forms.MouseEventHandler(this.fullscreen_MouseUp);
            // 
            // bing
            // 
            this.bing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bing.BackColor = System.Drawing.Color.Transparent;
            this.bing.Image = global::ImageViewer.Properties.Resources.bing;
            this.bing.Location = new System.Drawing.Point(325, 0);
            this.bing.Name = "bing";
            this.bing.Size = new System.Drawing.Size(64, 64);
            this.bing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bing.TabIndex = 1;
            this.bing.TabStop = false;
            this.bing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bing_MouseDown);
            this.bing.MouseEnter += new System.EventHandler(this.bing_MouseEnter);
            this.bing.MouseLeave += new System.EventHandler(this.bing_MouseLeave);
            this.bing.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bing_MouseUp);
            // 
            // fileNameBox
            // 
            this.fileNameBox.BackColor = System.Drawing.Color.Transparent;
            this.fileNameBox.Image = global::ImageViewer.Properties.Resources.blank;
            this.fileNameBox.Location = new System.Drawing.Point(0, 0);
            this.fileNameBox.Name = "fileNameBox";
            this.fileNameBox.Size = new System.Drawing.Size(64, 64);
            this.fileNameBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.fileNameBox.TabIndex = 2;
            this.fileNameBox.TabStop = false;
            // 
            // fileNameText
            // 
            this.fileNameText.AutoSize = true;
            this.fileNameText.BackColor = System.Drawing.Color.Transparent;
            this.fileNameText.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.fileNameText.ForeColor = System.Drawing.Color.White;
            this.fileNameText.Location = new System.Drawing.Point(0, 0);
            this.fileNameText.Name = "fileNameText";
            this.fileNameText.Size = new System.Drawing.Size(150, 30);
            this.fileNameText.TabIndex = 3;
            this.fileNameText.Text = "fileNameText";
            // 
            // copyrightText
            // 
            this.copyrightText.AutoSize = true;
            this.copyrightText.BackColor = System.Drawing.Color.Transparent;
            this.copyrightText.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.copyrightText.ForeColor = System.Drawing.Color.Silver;
            this.copyrightText.Location = new System.Drawing.Point(0, 32);
            this.copyrightText.Name = "copyrightText";
            this.copyrightText.Size = new System.Drawing.Size(144, 30);
            this.copyrightText.TabIndex = 4;
            this.copyrightText.Text = "copyrightText";
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.save.BackColor = System.Drawing.Color.Transparent;
            this.save.Image = global::ImageViewer.Properties.Resources.blank;
            this.save.Location = new System.Drawing.Point(320, 204);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(64, 64);
            this.save.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.save.TabIndex = 5;
            this.save.TabStop = false;
            this.save.MouseDown += new System.Windows.Forms.MouseEventHandler(this.save_MouseDown);
            this.save.MouseEnter += new System.EventHandler(this.save_MouseEnter);
            this.save.MouseLeave += new System.EventHandler(this.save_MouseLeave);
            this.save.MouseUp += new System.Windows.Forms.MouseEventHandler(this.save_MouseUp);
            // 
            // prevPage
            // 
            this.prevPage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.prevPage.BackColor = System.Drawing.Color.Transparent;
            this.prevPage.Image = global::ImageViewer.Properties.Resources.blank;
            this.prevPage.Location = new System.Drawing.Point(389, 70);
            this.prevPage.Name = "prevPage";
            this.prevPage.Size = new System.Drawing.Size(64, 64);
            this.prevPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.prevPage.TabIndex = 6;
            this.prevPage.TabStop = false;
            this.prevPage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.prevPage_MouseDown);
            this.prevPage.MouseEnter += new System.EventHandler(this.prevPage_MouseEnter);
            this.prevPage.MouseLeave += new System.EventHandler(this.prevPage_MouseLeave);
            this.prevPage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.prevPage_MouseUp);
            // 
            // nextPage
            // 
            this.nextPage.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.nextPage.BackColor = System.Drawing.Color.Transparent;
            this.nextPage.Image = global::ImageViewer.Properties.Resources.blank;
            this.nextPage.Location = new System.Drawing.Point(389, 134);
            this.nextPage.Name = "nextPage";
            this.nextPage.Size = new System.Drawing.Size(64, 64);
            this.nextPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.nextPage.TabIndex = 7;
            this.nextPage.TabStop = false;
            this.nextPage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.nextPage_MouseDown);
            this.nextPage.MouseEnter += new System.EventHandler(this.nextPage_MouseEnter);
            this.nextPage.MouseLeave += new System.EventHandler(this.nextPage_MouseLeave);
            this.nextPage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.nextPage_MouseUp);
            // 
            // settings
            // 
            this.settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.settings.BackColor = System.Drawing.Color.Transparent;
            this.settings.Image = global::ImageViewer.Properties.Resources.blank;
            this.settings.Location = new System.Drawing.Point(0, 204);
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(64, 64);
            this.settings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.settings.TabIndex = 8;
            this.settings.TabStop = false;
            this.settings.MouseDown += new System.Windows.Forms.MouseEventHandler(this.settings_MouseDown);
            this.settings.MouseEnter += new System.EventHandler(this.settings_MouseEnter);
            this.settings.MouseLeave += new System.EventHandler(this.settings_MouseLeave);
            this.settings.MouseUp += new System.Windows.Forms.MouseEventHandler(this.settings_MouseUp);
            // 
            // print
            // 
            this.print.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.print.BackColor = System.Drawing.Color.Transparent;
            this.print.Image = global::ImageViewer.Properties.Resources.blank;
            this.print.Location = new System.Drawing.Point(389, 0);
            this.print.Name = "print";
            this.print.Size = new System.Drawing.Size(64, 64);
            this.print.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.print.TabIndex = 9;
            this.print.TabStop = false;
            this.print.MouseDown += new System.Windows.Forms.MouseEventHandler(this.print_MouseDown);
            this.print.MouseEnter += new System.EventHandler(this.print_MouseEnter);
            this.print.MouseLeave += new System.EventHandler(this.print_MouseLeave);
            this.print.MouseUp += new System.Windows.Forms.MouseEventHandler(this.print_MouseUp);
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 268);
            this.Controls.Add(this.print);
            this.Controls.Add(this.settings);
            this.Controls.Add(this.nextPage);
            this.Controls.Add(this.prevPage);
            this.Controls.Add(this.save);
            this.Controls.Add(this.copyrightText);
            this.Controls.Add(this.fileNameText);
            this.Controls.Add(this.fileNameBox);
            this.Controls.Add(this.bing);
            this.Controls.Add(this.rotateRight);
            this.Controls.Add(this.rotateLeft);
            this.Controls.Add(this.arrowRight);
            this.Controls.Add(this.arrowLeft);
            this.Controls.Add(this.fullscreen);
            this.Controls.Add(this.pictureBox1);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ImageViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.onDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.onDragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullscreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.save)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.prevPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nextPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.print)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox arrowLeft;
        private System.Windows.Forms.PictureBox arrowRight;
        private System.Windows.Forms.PictureBox rotateLeft;
        private System.Windows.Forms.PictureBox rotateRight;
        private System.Windows.Forms.PictureBox fullscreen;
        private System.Windows.Forms.PictureBox bing;
        private System.Windows.Forms.PictureBox fileNameBox;
        private System.Windows.Forms.Label fileNameText;
        private System.Windows.Forms.Label copyrightText;
        private System.Windows.Forms.PictureBox save;
        private System.Windows.Forms.PictureBox prevPage;
        private System.Windows.Forms.PictureBox nextPage;
        private System.Windows.Forms.PictureBox settings;
        private System.Windows.Forms.PictureBox print;
    }
}

