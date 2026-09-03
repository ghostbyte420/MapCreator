using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MapCreator.Engine.Compiler;
using MapCreator.Engine.Plugin.BuildLogger;

namespace MapCreator.Controls.EncodeAltitudeBitmap
{
    public partial class SyncAltitudeBitmap : UserControl
    {
        private Bitmap i_Terrain;
        private readonly ClsTerrainTable iTerrain;
        private Bitmap i_Altitude;
        private readonly ClsAltitudeTable iAltitude;
        private readonly buildLogger iLogger;
        private bool i_RandomStatic;

        public SyncAltitudeBitmap()
        {
            InitializeComponent();

            var eAB = this;

            base.Load += new EventHandler(eAB.encodeAltitudeBitmap_Load);

            iTerrain = new ClsTerrainTable();
            iAltitude = new ClsAltitudeTable();
            iLogger = new buildLogger();
            i_RandomStatic = true;
        }

        private void encodeAltitudeBitmap_Load(object sender, EventArgs e)
        {
            encodeAltitudeBitmap_textBox_projectLocation.Text = Directory.GetCurrentDirectory();

            iTerrain.Load();
            iAltitude.Load();
        }

        private void encodeAltitudeBitmap_menuStrip_menuStripButton_loadProjectLocation_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog()
            {
                SelectedPath = encodeAltitudeBitmap_textBox_projectLocation.Text
            };
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                encodeAltitudeBitmap_textBox_projectLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void encodeAltitudeBitmap_menuStrip_menuStripButton_encodeAltitudeBitmap_Click(object sender, EventArgs e)
        {
            var progress = new Progress<int>(i => { encodeAltitudeBitmap_progressBar.Value = Math.Abs(i); }); // TODO: temporary fix, i didn't get why it put -73
            var logger = new Progress<string>(iLogger.LogMessage);
            var resetProgress = new Task(() =>
            {
                Thread.Sleep(1000);
                ((IProgress<int>)progress).Report(0);
            });
            await Task.Run(() => SyncAltitudeBitmapHelper.CreateAltitudeImage(encodeAltitudeBitmap_textBox_projectLocation.Text, encodeAltitudeBitmap_textBox_terrainBitmap.Text, encodeAltitudeBitmap_textBox_altitudeBitmap.Text, iAltitude, iTerrain, progress, logger)).ContinueWith(c => resetProgress.Start());

            /// await Task.Run(() => EncodeAltitudeBitmapHelper.MakeAltitudeImage(mainMenu_groupBox01_createYourWorld_panel02_workBench_groupBox01_syncYourAltitudeBitmap_textBox01_projectPath.Text, mainMenu_groupBox01_createYourWorld_panel02_workBench_groupBox01_syncYourAltitudeBitmap_textBox02_terrainBitmap.Text, mainMenu_groupBox01_createYourWorld_panel02_workBench_groupBox01_syncYourAltitudeBitmap_textBox03_altitudeBitmap.Text, iAltitude, iTerrain, progress, logger)).ContinueWith(c => resetProgress.Start());
        }
    }
}
