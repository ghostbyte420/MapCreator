using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;

using Microsoft.VisualBasic.CompilerServices;

using MapCreator.Engine.Compiler;
using MapCreator.Engine.Plugin.BuildLogger;

namespace MapCreator.Controls
{
    public partial class createMapTemplate : UserControl
    {
        private Bitmap i_Terrain;
        private readonly ClsTerrainTable iTerrain;
        private Bitmap i_Altitude;
        private readonly ClsAltitudeTable iAltitude;
        private bool i_RandomStatic;
        private readonly buildLogger iLogger;

        public createMapTemplate()
        {
            InitializeComponent();

            iTerrain = new ClsTerrainTable();
            iAltitude = new ClsAltitudeTable();
            i_RandomStatic = true;
            iLogger = new buildLogger();
        }

        private void createMapTemplate_Load(object sender, EventArgs e)
        {
            IEnumerator enumerator = null;

            iLogger.Show();

            var x = iLogger.Location.X + 100;
            var location = iLogger.Location;
            var point = new Point(x, location.Y + 100);
            Location = point;

            iTerrain.Load();
            iAltitude.Load();

            #region Data Directory Modification

            var str = string.Format("{0}\\MapCompiler\\Engine\\{1}", Directory.GetCurrentDirectory(), "MapInfo.xml");

            #endregion

            createMapTemplate_textBox_projectLocation.Text = Directory.GetCurrentDirectory();
            iTerrain.Display(createMapTemplate_comboBox_baseTerrainSelection);

            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(str);
                try
                {
                    createMapTemplate_comboBox_facetSizeSelection.Items.Clear();
                    try
                    {
                        enumerator = xmlDocument.SelectNodes("//Maps/Map").GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            var mapInfo = new MapInfo((XmlElement)enumerator.Current);
                            _ = createMapTemplate_comboBox_facetSizeSelection.Items.Add(mapInfo);
                        }
                    }
                    finally
                    {
                        if (enumerator is IDisposable)
                        {
                            ((IDisposable)enumerator).Dispose();
                        }
                    }
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    var exception = exception1;
                    iLogger.LogMessage(string.Format("XML Error:{0}", exception.Message));
                    ProjectData.ClearProjectError();
                }
            }
            catch (Exception exception2)
            {
                ProjectData.SetProjectError(exception2);
                iLogger.LogMessage(string.Format("Unable to find:{0}", str));
                ProjectData.ClearProjectError();
            }
        }

        private void createMapTemplate_menuStrip_menuStripButton_loadProjectLocation_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog()
            {
                SelectedPath = createMapTemplate_textBox_projectLocation.Text
            };

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                createMapTemplate_textBox_projectLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void createMapTemplate_menuStrip_menuStripButton_generateTemplate_Click(object sender, EventArgs e)
        {
            sbyte altID;
            byte groupID;

            var selectedItem = (MapInfo)createMapTemplate_comboBox_facetSizeSelection.SelectedItem;

            if (selectedItem == null)
            {
                iLogger.LogMessage("Error: Select a Map Type.");
            }
            else if (StringType.StrCmp(createMapTemplate_textBox_facetNameEntry.Text, string.Empty, false) != 0)
            {
                var str = string.Format("{0}/{1}/Map{2}", createMapTemplate_textBox_projectLocation.Text, createMapTemplate_textBox_facetNameEntry.Text, selectedItem.MapNumber);

                if (!Directory.Exists(str))
                {
                    _ = Directory.CreateDirectory(str);
                }

                if (createMapTemplate_comboBox_baseTerrainSelection.SelectedItem != null)
                {
                    var clsTerrain = (ClsTerrain)createMapTemplate_comboBox_baseTerrainSelection.SelectedItem;
                    groupID = clsTerrain.GroupID;
                    altID = clsTerrain.AltID;
                }
                else
                {
                    groupID = 9;
                    altID = 66;
                }

                iLogger.LogMessage("Creating Terrain Image.");
                iLogger.StartTask();

                try
                {
                    var str1 = string.Format("{0}/{1}", str, createMapTemplate_textBox_terrainBitmap.Text);
                    var palette = MakeTerrainBitmapFile(selectedItem.XSize, selectedItem.YSize, groupID, createMapTemplate_checkBox_dungeonAreaSelection.Checked);
                    palette.Palette = iTerrain.GetPalette();
                    palette.Save(str1, ImageFormat.Bmp);
                    palette.Dispose();
                }
                catch (Exception exception)
                {
                    ProjectData.SetProjectError(exception);
                    iLogger.LogMessage("Error: Problem creating Terrain Image.");
                    ProjectData.ClearProjectError();
                }

                //this.iLogger.EndTask();
                iLogger.LogTimeStamp();
                iLogger.LogMessage("Creating Altitude Image.");
                iLogger.StartTask();

                try
                {
                    var str2 = string.Format("{0}/{1}", str, createMapTemplate_textBox_altitudeBitmap.Text);
                    var altPalette = MakeAltitudeBitmapFile(selectedItem.XSize, selectedItem.YSize, altID, createMapTemplate_checkBox_dungeonAreaSelection.Checked);
                    altPalette.Palette = iAltitude.GetAltPalette();
                    altPalette.Save(str2, ImageFormat.Bmp);
                    altPalette.Dispose();
                }
                catch (Exception exception2)
                {
                    ProjectData.SetProjectError(exception2);
                    var exception1 = exception2;
                    iLogger.LogMessage("Error: Problem creating Altitude Image.");
                    iLogger.LogMessage(exception1.Message);
                    ProjectData.ClearProjectError();
                }

                //this.iLogger.EndTask();
                iLogger.LogTimeStamp();
                iLogger.LogMessage("Done.");
            }
            else
            {
                iLogger.LogMessage("Error: Enter a project Name.");
            }
        }

        private void createMapTemplate_menuStrip_menuStripButton_editFacetSizes_Click(object sender, EventArgs e)
        {
            // Build the full path relative to your application's startup directory
            string xmlPath = Path.Combine(Application.StartupPath, "MapCompiler", "Engine", "MapInfo.xml");

            if (File.Exists(xmlPath))
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = xmlPath,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not open the file: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("MapInfo.xml not found at: " + xmlPath);
            }
        }

        public Bitmap MakeTerrainBitmapFile(int xSize, int ySize, byte DefaultTerrain, bool Dungeon)
        {
            var bitmap = new Bitmap(xSize, ySize, PixelFormat.Format8bppIndexed)
            {
                Palette = iTerrain.GetPalette()
            };

            var rectangle = new Rectangle(0, 0, xSize, ySize);
            var bitmapDatum = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            var scan0 = bitmapDatum.Scan0;
            var width = checked(bitmapDatum.Width * bitmapDatum.Height);
            var defaultTerrain = new byte[checked(checked(width - 1) + 1)];

            Marshal.Copy(scan0, defaultTerrain, 0, width);

            if (!Dungeon)
            {
                var num = checked(xSize - 1);

                for (var i = 0; i <= num; i++)
                {
                    var num1 = checked(ySize - 1);

                    for (var j = 0; j <= num1; j++)
                    {
                        defaultTerrain[checked(checked(j * xSize) + i)] = DefaultTerrain;
                    }
                }
            }
            else
            {
                var num2 = checked(xSize - 1);

                for (var k = 0; k <= num2; k++)
                {
                    var num3 = checked(ySize - 1);

                    for (var l = 0; l <= num3; l++)
                    {
                        if (k <= 5119)
                        {
                            defaultTerrain[checked(checked(l * xSize) + k)] = DefaultTerrain;
                        }
                        else
                        {
                            defaultTerrain[checked(checked(l * xSize) + k)] = 19;
                        }
                    }
                }
            }

            Marshal.Copy(defaultTerrain, 0, scan0, width);
            bitmap.UnlockBits(bitmapDatum);

            return bitmap;
        }

        public Bitmap MakeAltitudeBitmapFile(int xSize, int ySize, sbyte DefaultAlt, bool Dungeon)
        {
            var bitmap = new Bitmap(xSize, ySize, PixelFormat.Format8bppIndexed)
            {
                Palette = iAltitude.GetAltPalette()
            };
            var rectangle = new Rectangle(0, 0, xSize, ySize);
            var bitmapDatum = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            var scan0 = bitmapDatum.Scan0;
            var width = checked(bitmapDatum.Width * bitmapDatum.Height);
            var defaultAlt = new byte[width];
            Marshal.Copy(scan0, defaultAlt, 0, width);
            if (!Dungeon)
            {
                var num = xSize - 1;
                for (var i = 0; i <= num; i++)
                {
                    var num1 = ySize - 1;
                    for (var j = 0; j <= num1; j++)
                    {
                        defaultAlt[(j * xSize) + i] = (byte)DefaultAlt;
                    }
                }
            }
            else
            {
                var num2 = xSize - 1;
                for (var k = 0; k <= num2; k++)
                {
                    var num3 = ySize - 1;
                    for (var l = 0; l <= num3; l++)
                    {
                        if (k <= 5119)
                        {
                            defaultAlt[(l * xSize) + k] = (byte)DefaultAlt;
                        }
                        else
                        {
                            defaultAlt[(l * xSize) + k] = 72;
                        }
                    }
                }
            }

            Marshal.Copy(defaultAlt, 0, scan0, width);
            bitmap.UnlockBits(bitmapDatum);
            return bitmap;
        }

        /// This Prevents Multiple Instances of the iLogger from Launching When The Create Map Template Button is Pressed Multiple Times
        /// This Closes The iLogger Whenever the UserControl is Changed
        private void createMapTemplate_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                iLogger.Dispose();
            }
        }
    }
}
