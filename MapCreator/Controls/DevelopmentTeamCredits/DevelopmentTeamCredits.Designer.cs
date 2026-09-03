namespace MapCreator.Controls.DevelopmentTeamCredits
{
    partial class developmentTeamCredits
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(developmentTeamCredits));
            developmentTeamCredits_panel = new Panel();
            developmentTeamCredits_pictureBox_cartographer = new PictureBox();
            developmentTeamCredits_pictureBox_creditsHeader = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)developmentTeamCredits_pictureBox_cartographer).BeginInit();
            ((System.ComponentModel.ISupportInitialize)developmentTeamCredits_pictureBox_creditsHeader).BeginInit();
            SuspendLayout();
            // 
            // developmentTeamCredits_panel
            // 
            developmentTeamCredits_panel.Location = new Point(329, 146);
            developmentTeamCredits_panel.Name = "developmentTeamCredits_panel";
            developmentTeamCredits_panel.Size = new Size(246, 301);
            developmentTeamCredits_panel.TabIndex = 0;
            // 
            // developmentTeamCredits_pictureBox_cartographer
            // 
            developmentTeamCredits_pictureBox_cartographer.BackColor = Color.Transparent;
            developmentTeamCredits_pictureBox_cartographer.BackgroundImage = (Image)resources.GetObject("developmentTeamCredits_pictureBox_cartographer.BackgroundImage");
            developmentTeamCredits_pictureBox_cartographer.BackgroundImageLayout = ImageLayout.Stretch;
            developmentTeamCredits_pictureBox_cartographer.Image = (Image)resources.GetObject("developmentTeamCredits_pictureBox_cartographer.Image");
            developmentTeamCredits_pictureBox_cartographer.Location = new Point(11, 146);
            developmentTeamCredits_pictureBox_cartographer.Name = "developmentTeamCredits_pictureBox_cartographer";
            developmentTeamCredits_pictureBox_cartographer.Size = new Size(320, 301);
            developmentTeamCredits_pictureBox_cartographer.TabIndex = 0;
            developmentTeamCredits_pictureBox_cartographer.TabStop = false;
            // 
            // developmentTeamCredits_pictureBox_creditsHeader
            // 
            developmentTeamCredits_pictureBox_creditsHeader.BackColor = Color.Transparent;
            developmentTeamCredits_pictureBox_creditsHeader.Image = (Image)resources.GetObject("developmentTeamCredits_pictureBox_creditsHeader.Image");
            developmentTeamCredits_pictureBox_creditsHeader.Location = new Point(377, 61);
            developmentTeamCredits_pictureBox_creditsHeader.Name = "developmentTeamCredits_pictureBox_creditsHeader";
            developmentTeamCredits_pictureBox_creditsHeader.Size = new Size(152, 50);
            developmentTeamCredits_pictureBox_creditsHeader.TabIndex = 0;
            developmentTeamCredits_pictureBox_creditsHeader.TabStop = false;
            // 
            // developmentTeamCredits
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(developmentTeamCredits_pictureBox_creditsHeader);
            Controls.Add(developmentTeamCredits_panel);
            Controls.Add(developmentTeamCredits_pictureBox_cartographer);
            DoubleBuffered = true;
            Name = "developmentTeamCredits";
            Size = new Size(774, 511);
            ((System.ComponentModel.ISupportInitialize)developmentTeamCredits_pictureBox_cartographer).EndInit();
            ((System.ComponentModel.ISupportInitialize)developmentTeamCredits_pictureBox_creditsHeader).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel developmentTeamCredits_panel;
        private PictureBox developmentTeamCredits_pictureBox_cartographer;
        private PictureBox developmentTeamCredits_pictureBox_creditsHeader;
    }
}
