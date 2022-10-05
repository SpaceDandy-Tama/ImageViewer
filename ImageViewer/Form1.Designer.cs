namespace ImageViewer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullscreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(240, 217);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // arrowLeft
            // 
            this.arrowLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.arrowLeft.BackColor = System.Drawing.Color.Transparent;
            this.arrowLeft.Image = global::ImageViewer.Properties.Resources.arrowLeft;
            this.arrowLeft.Location = new System.Drawing.Point(0, 169);
            this.arrowLeft.Name = "arrowLeft";
            this.arrowLeft.Size = new System.Drawing.Size(48, 48);
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
            this.arrowRight.Location = new System.Drawing.Point(48, 169);
            this.arrowRight.Name = "arrowRight";
            this.arrowRight.Size = new System.Drawing.Size(48, 48);
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
            this.rotateLeft.Location = new System.Drawing.Point(96, 169);
            this.rotateLeft.Name = "rotateLeft";
            this.rotateLeft.Size = new System.Drawing.Size(48, 48);
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
            this.rotateRight.Location = new System.Drawing.Point(144, 169);
            this.rotateRight.Name = "rotateRight";
            this.rotateRight.Size = new System.Drawing.Size(48, 48);
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
            this.fullscreen.Location = new System.Drawing.Point(192, 169);
            this.fullscreen.Name = "fullscreen";
            this.fullscreen.Size = new System.Drawing.Size(48, 48);
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
            this.bing.Location = new System.Drawing.Point(192, 0);
            this.bing.Name = "bing";
            this.bing.Size = new System.Drawing.Size(48, 48);
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
            this.fileNameText.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.fileNameText.ForeColor = System.Drawing.Color.White;
            this.fileNameText.Location = new System.Drawing.Point(0, -1);
            this.fileNameText.Name = "fileNameText";
            this.fileNameText.Size = new System.Drawing.Size(134, 30);
            this.fileNameText.TabIndex = 3;
            this.fileNameText.Text = "fileNameText";
            // 
            // copyrightText
            // 
            this.copyrightText.AutoSize = true;
            this.copyrightText.BackColor = System.Drawing.Color.Transparent;
            this.copyrightText.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.copyrightText.ForeColor = System.Drawing.Color.Silver;
            this.copyrightText.Location = new System.Drawing.Point(0, 29);
            this.copyrightText.Name = "copyrightText";
            this.copyrightText.Size = new System.Drawing.Size(137, 30);
            this.copyrightText.TabIndex = 4;
            this.copyrightText.Text = "copyrightText";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 217);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ImageViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrowRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotateRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullscreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameBox)).EndInit();
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
    }
}

