namespace MapCreator.Controls.ConfigureColorTables
{
    partial class configureColorTables
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(configureColorTables));
            configureColorTables_menuStrip = new MenuStrip();
            configureColorTables_menuStrip_menuStripButton_terrain = new ToolStripMenuItem();
            configureColorTables_menuStrip_menuStripButton_altitude = new ToolStripMenuItem();
            configureColorTables_menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // configureColorTables_menuStrip
            // 
            configureColorTables_menuStrip.BackgroundImage = (Image)resources.GetObject("configureColorTables_menuStrip.BackgroundImage");
            configureColorTables_menuStrip.BackgroundImageLayout = ImageLayout.Stretch;
            configureColorTables_menuStrip.Font = new Font("Segoe UI", 11F);
            configureColorTables_menuStrip.ImageScalingSize = new Size(24, 24);
            configureColorTables_menuStrip.Items.AddRange(new ToolStripItem[] { configureColorTables_menuStrip_menuStripButton_terrain, configureColorTables_menuStrip_menuStripButton_altitude });
            configureColorTables_menuStrip.Location = new Point(0, 0);
            configureColorTables_menuStrip.Name = "configureColorTables_menuStrip";
            configureColorTables_menuStrip.Size = new Size(774, 28);
            configureColorTables_menuStrip.TabIndex = 0;
            configureColorTables_menuStrip.Text = "menuStrip1";
            // 
            // configureColorTables_menuStrip_menuStripButton_terrain
            // 
            configureColorTables_menuStrip_menuStripButton_terrain.ForeColor = Color.Lavender;
            configureColorTables_menuStrip_menuStripButton_terrain.Margin = new Padding(0, 0, 10, 0);
            configureColorTables_menuStrip_menuStripButton_terrain.Name = "configureColorTables_menuStrip_menuStripButton_terrain";
            configureColorTables_menuStrip_menuStripButton_terrain.Size = new Size(66, 24);
            configureColorTables_menuStrip_menuStripButton_terrain.Text = "Terrain";
            // 
            // configureColorTables_menuStrip_menuStripButton_altitude
            // 
            configureColorTables_menuStrip_menuStripButton_altitude.ForeColor = Color.Lavender;
            configureColorTables_menuStrip_menuStripButton_altitude.Name = "configureColorTables_menuStrip_menuStripButton_altitude";
            configureColorTables_menuStrip_menuStripButton_altitude.Size = new Size(74, 24);
            configureColorTables_menuStrip_menuStripButton_altitude.Text = "Altitude";
            // 
            // configureColorTables
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            Controls.Add(configureColorTables_menuStrip);
            Name = "configureColorTables";
            Size = new Size(774, 511);
            configureColorTables_menuStrip.ResumeLayout(false);
            configureColorTables_menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip configureColorTables_menuStrip;
        private ToolStripMenuItem configureColorTables_menuStrip_menuStripButton_terrain;
        private ToolStripMenuItem configureColorTables_menuStrip_menuStripButton_altitude;
    }
}
