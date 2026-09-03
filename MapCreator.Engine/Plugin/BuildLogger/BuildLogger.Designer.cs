namespace MapCreator.Engine.Plugin.BuildLogger
{
    partial class buildLogger
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(buildLogger));
            buildLogger_menuStrip = new MenuStrip();
            buildLogger_menuStrip_menuStripButton_saveAs = new ToolStripMenuItem();
            buildLogger_menuStrip_menuStripButton_printLog = new ToolStripMenuItem();
            buildLogger_statusStrip = new StatusStrip();
            buildLogger_textBox_logDisplay = new TextBox();
            buildLogger_menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // buildLogger_menuStrip
            // 
            buildLogger_menuStrip.BackgroundImage = (Image)resources.GetObject("buildLogger_menuStrip.BackgroundImage");
            buildLogger_menuStrip.BackgroundImageLayout = ImageLayout.Stretch;
            buildLogger_menuStrip.Font = new Font("Segoe UI", 11F);
            buildLogger_menuStrip.ImageScalingSize = new Size(24, 24);
            buildLogger_menuStrip.Items.AddRange(new ToolStripItem[] { buildLogger_menuStrip_menuStripButton_saveAs, buildLogger_menuStrip_menuStripButton_printLog });
            buildLogger_menuStrip.Location = new Point(0, 0);
            buildLogger_menuStrip.Name = "buildLogger_menuStrip";
            buildLogger_menuStrip.Size = new Size(475, 32);
            buildLogger_menuStrip.TabIndex = 0;
            buildLogger_menuStrip.Text = "menuStrip1";
            // 
            // buildLogger_menuStrip_menuStripButton_saveAs
            // 
            buildLogger_menuStrip_menuStripButton_saveAs.Image = (Image)resources.GetObject("buildLogger_menuStrip_menuStripButton_saveAs.Image");
            buildLogger_menuStrip_menuStripButton_saveAs.Margin = new Padding(375, 0, -5, 0);
            buildLogger_menuStrip_menuStripButton_saveAs.Name = "buildLogger_menuStrip_menuStripButton_saveAs";
            buildLogger_menuStrip_menuStripButton_saveAs.Size = new Size(49, 28);
            buildLogger_menuStrip_menuStripButton_saveAs.Text = " ";
            buildLogger_menuStrip_menuStripButton_saveAs.Click += buildLogger_menuStrip_menuStripButton_saveAs_Click;
            // 
            // buildLogger_menuStrip_menuStripButton_printLog
            // 
            buildLogger_menuStrip_menuStripButton_printLog.Image = (Image)resources.GetObject("buildLogger_menuStrip_menuStripButton_printLog.Image");
            buildLogger_menuStrip_menuStripButton_printLog.Name = "buildLogger_menuStrip_menuStripButton_printLog";
            buildLogger_menuStrip_menuStripButton_printLog.Size = new Size(49, 28);
            buildLogger_menuStrip_menuStripButton_printLog.Text = " ";
            buildLogger_menuStrip_menuStripButton_printLog.Click += buildLogger_menuStrip_menuStripButton_printLog_Click;
            // 
            // buildLogger_statusStrip
            // 
            buildLogger_statusStrip.Location = new Point(0, 345);
            buildLogger_statusStrip.Name = "buildLogger_statusStrip";
            buildLogger_statusStrip.Size = new Size(475, 22);
            buildLogger_statusStrip.TabIndex = 1;
            buildLogger_statusStrip.Text = "statusStrip1";
            // 
            // buildLogger_textBox_logDisplay
            // 
            buildLogger_textBox_logDisplay.Dock = DockStyle.Right;
            buildLogger_textBox_logDisplay.Location = new Point(121, 32);
            buildLogger_textBox_logDisplay.Multiline = true;
            buildLogger_textBox_logDisplay.Name = "buildLogger_textBox_logDisplay";
            buildLogger_textBox_logDisplay.ScrollBars = ScrollBars.Vertical;
            buildLogger_textBox_logDisplay.Size = new Size(354, 313);
            buildLogger_textBox_logDisplay.TabIndex = 2;
            // 
            // buildLogger
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(475, 367);
            ControlBox = false;
            Controls.Add(buildLogger_textBox_logDisplay);
            Controls.Add(buildLogger_statusStrip);
            Controls.Add(buildLogger_menuStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = buildLogger_menuStrip;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "buildLogger";
            Text = "BuildLogger";
            Load += buildLogger_Load;
            buildLogger_menuStrip.ResumeLayout(false);
            buildLogger_menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip buildLogger_menuStrip;
        private StatusStrip buildLogger_statusStrip;
        private TextBox buildLogger_textBox_logDisplay;
        private ToolStripMenuItem buildLogger_menuStrip_menuStripButton_saveAs;
        private ToolStripMenuItem buildLogger_menuStrip_menuStripButton_printLog;
    }
}