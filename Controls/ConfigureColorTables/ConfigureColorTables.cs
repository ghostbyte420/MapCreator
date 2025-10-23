using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MapCreator.Engine.Compiler;
using MapCreator.Engine.UltimaSDK;

namespace MapCreator.Controls.ConfigureColorTables
{
    public partial class configureColorTables : UserControl
    {
        private int i_Menu;
        private ClsAltitudeTable i_Altitude;
        private ClsTerrainTable i_Terrain;

        public configureColorTables()
        {
            InitializeComponent();

            configureColorTables cCT = this;

            base.Load += new EventHandler(cCT.configureColorTables_Load);
            this.i_Menu = 0;
            this.i_Altitude = new ClsAltitudeTable();
            this.i_Terrain = new ClsTerrainTable();

            #region  Screen Flickering Management
            
            /// For UserControl Transitioning
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();

            #endregion
        }

        private void configureColorTables_Load(object sender, EventArgs e)
        {
            this.i_Menu = 0;

            this.configureColorTables_pictureBox_tileDisplay.Visible = false;
            this.configureColorTables_pictureBox_altitudeTiles.Visible = false;

            this.configureColorTables_label_fileTypeWarning.Show();
            this.configureColorTables_label_altitudeColorGradient.Hide();

            this.configureColorTables_pictureBox_colorTables.Show();

            /// Label Transparency: Adobe Photoshop Color Palette
            configureColorTables_label_colorTableHeader.FlatStyle = FlatStyle.Standard;
            configureColorTables_label_colorTableHeader.BackColor = Color.Transparent;
        }

        #region menuStrip Buttons

        private void configureColorTables_menuStrip_menuStripButton_getAdobePhotoshop_Click(object sender, EventArgs e)
        {
            ProcessStartInfo getAdobePhotoshop = new ProcessStartInfo
            {
                FileName = "https://www.adobe.com/products/photoshop/",
                UseShellExecute = true
            };

            Process.Start(getAdobePhotoshop);
        }

        private void configureColorTables_menuStrip_menuStripButton_gotoExportFolder_Click(object sender, EventArgs e)
        {
            var path = Path.Combine("Development", "DrawingTools", "AdobePhotoshop", "ColorTables");

            _ = Directory.CreateDirectory(path);

            _ = Process.Start("explorer.exe", path);
        }

        private void configureColorTables_menuStrip_menuStripButton_loadColorSwatch_terrain_Click(object sender, EventArgs e)
        {
            this.i_Menu = 0;
            this.configureColorTables_label_colorTableHeader.Text = "Terrain Color Table";

            this.i_Terrain.Load();
            this.i_Terrain.Display(this.configureColorTables_listBox_swatchList);

            this.configureColorTables_pictureBox_colorTables.Hide();
            this.configureColorTables_pictureBox_altitudeTiles.Visible = false;
            this.configureColorTables_pictureBox_tileDisplay.Visible = true;

            this.configureColorTables_label_fileTypeWarning.Show();
            this.configureColorTables_label_altitudeColorGradient.Hide();
            this.configureColorTables_pictureBox_altitudeTiles.Hide();
        }

        private void configureColorTables_menuStrip_menuStripButton_loadColorSwatch_altitude_Click(object sender, EventArgs e)
        {
            this.i_Menu = 1;
            this.configureColorTables_label_colorTableHeader.Text = "Altitude Color Table";

            this.i_Altitude.Load();
            this.i_Altitude.Display(this.configureColorTables_listBox_swatchList);

            this.configureColorTables_pictureBox_colorTables.Hide();
            this.configureColorTables_pictureBox_tileDisplay.Visible = false;
            this.configureColorTables_pictureBox_altitudeTiles.Visible = true;

            this.configureColorTables_label_fileTypeWarning.Hide();
            this.configureColorTables_label_altitudeColorGradient.Show();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_terrain_act_Click(object sender, EventArgs e)
        {
            this.i_Terrain.SaveACT();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_terrain_aco_Click(object sender, EventArgs e)
        {
            this.i_Terrain.SaveACO();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_altitude_act_Click(object sender, EventArgs e)
        {
            this.i_Altitude.SaveACT();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_altitude_aco_Click(object sender, EventArgs e)
        {
            this.i_Altitude.SaveACO();
        }


        #endregion

        private void configureColorTables_listBox_swatchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.configureColorTables_listBox_swatchList.SelectedItem != null)
            {
                switch (this.i_Menu)
                {
                    case 0:
                        {
                            ClsTerrain selectedItem = (ClsTerrain)this.configureColorTables_listBox_swatchList.SelectedItem;
                            this.configureColorTables_propertyGrid_swatchDetails.SelectedObject = selectedItem;
                            this.configureColorTables_pictureBox_tileDisplay.Image = Art.GetLand(selectedItem.TileID);
                            break;
                        }
                    case 1:
                        {
                            ClsAltitude clsAltitude = (ClsAltitude)this.configureColorTables_listBox_swatchList.SelectedItem;
                            this.configureColorTables_propertyGrid_swatchDetails.SelectedObject = clsAltitude;
                            break;
                        }
                }
            }
        }
    }
}
