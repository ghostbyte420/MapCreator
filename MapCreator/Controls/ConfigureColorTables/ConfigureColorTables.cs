using MapCreator.Engine.Compiler;
using MapCreator.Engine.UltimaSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

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
            var path = Path.Combine("Development", "DrawingTools");

            _ = Directory.CreateDirectory(path);

            _ = Process.Start("explorer.exe", path);
        }

        private void configureColorTables_menuStrip_menuStripButton_loadColorSwatch_terrain_Click(object sender, EventArgs e)
        {
            this.i_Menu = 0;
            this.configureColorTables_label_colorTableHeader.Text = "Terrain Color Table";

            this.i_Terrain.Load();
            this.i_Terrain.Display(this.configureColorTables_listBox_swatchList);

            #region Load ListBox In Reverse Order

            // Get a reversed copy of the current items
            var reversed = this.configureColorTables_listBox_swatchList.Items.Cast<object>().Reverse().ToList();

            // Clear and refill the ListBox with the reversed items
            this.configureColorTables_listBox_swatchList.Items.Clear();
            foreach (var item in reversed)
                this.configureColorTables_listBox_swatchList.Items.Add(item);

            #endregion

            this.configureColorTables_pictureBox_colorTables.Hide();
            this.configureColorTables_pictureBox_altitudeTiles.Visible = false;
            this.configureColorTables_pictureBox_tileDisplay.Visible = true;

            this.configureColorTables_label_fileTypeWarning.Show();
            this.configureColorTables_label_altitudeColorGradient.Hide();
            this.configureColorTables_pictureBox_altitudeTiles.Hide();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_terrain_act_Click(object sender, EventArgs e)
        {
            this.i_Terrain.SaveACT();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_terrain_aco_Click(object sender, EventArgs e)
        {
            this.i_Terrain.SaveACO();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_terrain_png_Click(object sender, EventArgs e)
        {
            ExportColorSwatchWithLabels("Terrain", "terrain_swatch.png");
        }

        private void configureColorTables_menuStrip_menuStripButton_loadColorSwatch_altitude_Click(object sender, EventArgs e)
        {
            this.i_Menu = 1;
            this.configureColorTables_label_colorTableHeader.Text = "Altitude Color Table";

            this.i_Altitude.Load();
            this.i_Altitude.Display(this.configureColorTables_listBox_swatchList);

            #region Load ListBox In Reverse Order

            // Get a reversed copy of the current items
            var reversed = this.configureColorTables_listBox_swatchList.Items.Cast<object>().Reverse().ToList();

            // Clear and refill the ListBox with the reversed items
            this.configureColorTables_listBox_swatchList.Items.Clear();
            foreach (var item in reversed)
                this.configureColorTables_listBox_swatchList.Items.Add(item);

            #endregion

            this.configureColorTables_pictureBox_colorTables.Hide();
            this.configureColorTables_pictureBox_tileDisplay.Visible = false;
            this.configureColorTables_pictureBox_altitudeTiles.Visible = true;

            this.configureColorTables_label_fileTypeWarning.Hide();
            this.configureColorTables_label_altitudeColorGradient.Show();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_altitude_act_Click(object sender, EventArgs e)
        {
            this.i_Altitude.SaveACT();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_altitude_aco_Click(object sender, EventArgs e)
        {
            this.i_Altitude.SaveACO();
        }

        private void configureColorTables_menuStrip_menuStripButton_exportColorSwatch_altitude_png_Click(object sender, EventArgs e)
        {
            ExportColorSwatchWithLabels("Altitude", "altitude_swatch.png");
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

        /// <summary>
        /// Self-contained method to export color swatches as PNG
        /// </summary>
        private static void ExportColorSwatchWithLabels(string xmlFileName, string outputFileName)
        {
            try
            {
                // Build output path (XML now comes from the embedded FacetData, not disk)
                string appPath = Application.StartupPath;

                #region Directory Modification

                string outputFolder = Path.Combine(appPath, "Development", "DrawingTools", "OtherApplications", "ColorTables", "IMG");

                #endregion

                // Load XML from the embedded engine resources (Custom override honored)
                XDocument xdoc;
                using (var stream = FacetData.OpenRead($"{xmlFileName}.xml"))
                {
                    if (stream == null) return;
                    xdoc = XDocument.Load(stream);
                }
                var elements = xdoc.Root.Elements();
                if (!elements.Any()) return;

                // ===== CHANGE THESE NUMBERS TO MAKE THINGS BIGGER OR SMALLER =====
                int colorWidth = 80;
                int labelWidth = 350;
                int rowHeight = 60;
                // ===============================================================

                using (Bitmap bmp = new Bitmap(colorWidth + labelWidth, elements.Count() * rowHeight))
                {
                    using (Graphics graphics = Graphics.FromImage(bmp))
                    {
                        // Set up graphics
                        graphics.Clear(Color.White);
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        Font labelFont = new Font("Arial", 11, FontStyle.Regular);
                        Font rgbFont = new Font("Arial", 9, FontStyle.Regular);
                        Brush textBrush = Brushes.Black;
                        Pen borderPen = new Pen(Color.Black, 1);

                        int yPos = 0;
                        foreach (var element in elements)
                        {
                            try
                            {
                                int r = int.Parse(element.Attribute("R").Value);
                                int g = int.Parse(element.Attribute("G").Value);
                                int b = int.Parse(element.Attribute("B").Value);
                                Color color = Color.FromArgb(r, g, b);

                                string name = "";
                                if (xmlFileName == "Terrain")
                                {
                                    name = $"{element.Attribute("Name")?.Value} (ID: {element.Attribute("ID")?.Value})";
                                }
                                else if (xmlFileName == "Altitude")
                                {
                                    name = $"{element.Attribute("Type")?.Value} {element.Attribute("Altitude")?.Value} (Key: {element.Attribute("Key")?.Value})";
                                }

                                graphics.FillRectangle(new SolidBrush(color), 0, yPos, colorWidth, rowHeight);
                                graphics.DrawRectangle(borderPen, 0, yPos, colorWidth, rowHeight);
                                graphics.DrawString(name, labelFont, textBrush, colorWidth + 10, yPos + 10);
                                graphics.DrawString($"RGB: {r},{g},{b}", rgbFont, Brushes.DarkGray, colorWidth + 10, yPos + 35);

                                yPos += rowHeight;
                            }
                            catch
                            {
                                yPos += rowHeight;
                                continue;
                            }
                        }

                        // Create output directory if needed
                        Directory.CreateDirectory(outputFolder);

                        // Save PNG
                        string outputPath = Path.Combine(outputFolder, outputFileName);
                        bmp.Save(outputPath, ImageFormat.Png);

                        // === ADD THIS LINE FOR SOUND ===
                        System.Media.SystemSounds.Exclamation.Play();  // Ding sound when done!
                                                                       // ===============================
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting color swatch: {ex.Message}");
                // You could add a different sound for errors if you want:
                // System.Media.SystemSounds.Hand.Play();
            }
        }
    }
}
