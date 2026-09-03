namespace MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator
{
    partial class terrainGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(terrainGenerator));
            terrainGenerator_menuStrip = new MenuStrip();
            terrainGenerator_menuStrip_menuStripButton_generateLand = new ToolStripMenuItem();
            terrainGenerator_menuStrip_menuStripButton_saveImage = new ToolStripMenuItem();
            terrainGenerator_canvasDisplay = new CanvasDisplay();
            terrainGenerator_canvasDisplay_trackBar_threshold = new TrackBar();
            terrainGenerator_canvasDisplay_trackBar_roughness = new TrackBar();
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom = new NumericUpDown();
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold = new Label();
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness = new Label();
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel = new Label();
            terrainGenerator_menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)terrainGenerator_canvasDisplay_trackBar_threshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)terrainGenerator_canvasDisplay_trackBar_roughness).BeginInit();
            ((System.ComponentModel.ISupportInitialize)terrainGenerator_canvasDisplay_numericUpDown_canvasZoom).BeginInit();
            SuspendLayout();
            // 
            // terrainGenerator_menuStrip
            // 
            terrainGenerator_menuStrip.Font = new Font("Segoe UI", 11F);
            terrainGenerator_menuStrip.ImageScalingSize = new Size(25, 25);
            terrainGenerator_menuStrip.Items.AddRange(new ToolStripItem[] { terrainGenerator_menuStrip_menuStripButton_generateLand, terrainGenerator_menuStrip_menuStripButton_saveImage });
            terrainGenerator_menuStrip.Location = new Point(0, 0);
            terrainGenerator_menuStrip.Name = "terrainGenerator_menuStrip";
            terrainGenerator_menuStrip.Size = new Size(774, 33);
            terrainGenerator_menuStrip.TabIndex = 0;
            terrainGenerator_menuStrip.Text = "menuStrip1";
            // 
            // terrainGenerator_menuStrip_menuStripButton_generateLand
            // 
            terrainGenerator_menuStrip_menuStripButton_generateLand.Image = (Image)resources.GetObject("terrainGenerator_menuStrip_menuStripButton_generateLand.Image");
            terrainGenerator_menuStrip_menuStripButton_generateLand.Name = "terrainGenerator_menuStrip_menuStripButton_generateLand";
            terrainGenerator_menuStrip_menuStripButton_generateLand.Size = new Size(142, 29);
            terrainGenerator_menuStrip_menuStripButton_generateLand.Text = "Generate Land";
            terrainGenerator_menuStrip_menuStripButton_generateLand.Click += terrainGenerator_menuStrip_menuStripButton_generateLand_Click;
            // 
            // terrainGenerator_menuStrip_menuStripButton_saveImage
            // 
            terrainGenerator_menuStrip_menuStripButton_saveImage.Image = (Image)resources.GetObject("terrainGenerator_menuStrip_menuStripButton_saveImage.Image");
            terrainGenerator_menuStrip_menuStripButton_saveImage.Name = "terrainGenerator_menuStrip_menuStripButton_saveImage";
            terrainGenerator_menuStrip_menuStripButton_saveImage.Size = new Size(123, 29);
            terrainGenerator_menuStrip_menuStripButton_saveImage.Text = "Save Image";
            terrainGenerator_menuStrip_menuStripButton_saveImage.Click += terrainGenerator_menuStrip_menuStripButton_saveImage_Click;
            // 
            // terrainGenerator_canvasDisplay
            // 
            terrainGenerator_canvasDisplay.BackColor = Color.Black;
            terrainGenerator_canvasDisplay.Dock = DockStyle.Top;
            terrainGenerator_canvasDisplay.Location = new Point(0, 33);
            terrainGenerator_canvasDisplay.MapImage = null;
            terrainGenerator_canvasDisplay.Name = "terrainGenerator_canvasDisplay";
            terrainGenerator_canvasDisplay.Size = new Size(774, 387);
            terrainGenerator_canvasDisplay.TabIndex = 1;
            terrainGenerator_canvasDisplay.TabStop = true;
            terrainGenerator_canvasDisplay.Zoom = 1F;
            // 
            // terrainGenerator_canvasDisplay_trackBar_threshold
            // 
            terrainGenerator_canvasDisplay_trackBar_threshold.Location = new Point(3, 454);
            terrainGenerator_canvasDisplay_trackBar_threshold.Name = "terrainGenerator_canvasDisplay_trackBar_threshold";
            terrainGenerator_canvasDisplay_trackBar_threshold.Size = new Size(215, 45);
            terrainGenerator_canvasDisplay_trackBar_threshold.TabIndex = 2;
            terrainGenerator_canvasDisplay_trackBar_threshold.ValueChanged += terrainGenerator_canvasDisplay_trackBar_threshold_ValueChanged;
            // 
            // terrainGenerator_canvasDisplay_trackBar_roughness
            // 
            terrainGenerator_canvasDisplay_trackBar_roughness.Location = new Point(245, 454);
            terrainGenerator_canvasDisplay_trackBar_roughness.Name = "terrainGenerator_canvasDisplay_trackBar_roughness";
            terrainGenerator_canvasDisplay_trackBar_roughness.Size = new Size(215, 45);
            terrainGenerator_canvasDisplay_trackBar_roughness.TabIndex = 3;
            terrainGenerator_canvasDisplay_trackBar_roughness.ValueChanged += terrainGenerator_canvasDisplay_trackBar_roughness_ValueChanged;
            // 
            // terrainGenerator_canvasDisplay_numericUpDown_canvasZoom
            // 
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Location = new Point(758, 453);
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Name = "terrainGenerator_canvasDisplay_numericUpDown_canvasZoom";
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Size = new Size(84, 23);
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.TabIndex = 4;
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.ValueChanged += terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_ValueChanged;
            // 
            // terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold
            // 
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.AutoSize = true;
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.ForeColor = Color.Lavender;
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.Location = new Point(109, 436);
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.Name = "terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold";
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.Size = new Size(63, 15);
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.TabIndex = 5;
            terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.Text = "Threshold:";
            // 
            // terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness
            // 
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.AutoSize = true;
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.ForeColor = Color.Lavender;
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.Location = new Point(348, 436);
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.Name = "terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness";
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.Size = new Size(68, 15);
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.TabIndex = 6;
            terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.Text = "Roughness:";
            // 
            // terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel
            // 
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.AutoSize = true;
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.Font = new Font("Segoe UI", 11F);
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.ForeColor = Color.Lavender;
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.Location = new Point(660, 454);
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.Name = "terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel";
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.Size = new Size(90, 20);
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.TabIndex = 7;
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel.Text = "Zoom Level:";
            // 
            // terrainGenerator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            Controls.Add(terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness);
            Controls.Add(terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold);
            Controls.Add(terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel);
            Controls.Add(terrainGenerator_canvasDisplay_numericUpDown_canvasZoom);
            Controls.Add(terrainGenerator_canvasDisplay);
            Controls.Add(terrainGenerator_menuStrip);
            Controls.Add(terrainGenerator_canvasDisplay_trackBar_roughness);
            Controls.Add(terrainGenerator_canvasDisplay_trackBar_threshold);
            Name = "terrainGenerator";
            Size = new Size(774, 511);
            terrainGenerator_menuStrip.ResumeLayout(false);
            terrainGenerator_menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)terrainGenerator_canvasDisplay_trackBar_threshold).EndInit();
            ((System.ComponentModel.ISupportInitialize)terrainGenerator_canvasDisplay_trackBar_roughness).EndInit();
            ((System.ComponentModel.ISupportInitialize)terrainGenerator_canvasDisplay_numericUpDown_canvasZoom).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip terrainGenerator_menuStrip;
        private ToolStripMenuItem terrainGenerator_menuStrip_menuStripButton_generateLand;
        private ToolStripMenuItem terrainGenerator_menuStrip_menuStripButton_saveImage;
        private CanvasDisplay terrainGenerator_canvasDisplay;
        private TrackBar terrainGenerator_canvasDisplay_trackBar_threshold;
        private TrackBar terrainGenerator_canvasDisplay_trackBar_roughness;
        private NumericUpDown terrainGenerator_canvasDisplay_numericUpDown_canvasZoom;
        private Label terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold;
        private Label terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness;
        private Label terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_label_zoomLevel;
    }
}
