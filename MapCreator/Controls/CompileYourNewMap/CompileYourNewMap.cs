using MapCreator.Engine.Compiler;
using MapCreator.Engine.Plugin.BuildLogger;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapCreator.Controls.CompileYourNewMap
{
    public partial class compileYourNewMap : UserControl
    {
        private Bitmap i_Terrain;
        private readonly ClsTerrainTable iTerrain;
        private Bitmap i_Altitude;
        private readonly ClsAltitudeTable iAltitude;
        private readonly buildLogger iLogger;
        private bool i_RandomStatic;

        public compileYourNewMap()
        {
            InitializeComponent();

            var cYNM = this;
            base.Load += new EventHandler(cYNM.compileYourNewMap_Load);

            iTerrain = new ClsTerrainTable();
            iAltitude = new ClsAltitudeTable();
            iLogger = new buildLogger();
            i_RandomStatic = true;
        }

        private void compileYourNewMap_Load(object sender, EventArgs e)
        {
            this.iLogger.Show();
            int x = checked(this.iLogger.Location.X + 100);
            Point location = this.iLogger.Location;
            Point point = new Point(x, checked(location.Y + 100));
            this.Location = point;
            this.compileYourNewMap_textBox_projectLocation.Text = AppDomain.CurrentDomain.BaseDirectory;
        }

        private void compileYourNewMap_menuStrip_menuStripButton_loadProjectLocation_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog()
            {
                SelectedPath = compileYourNewMap_textBox_projectLocation.Text
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                compileYourNewMap_textBox_projectLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void compileYourNewMap_menuStrip__menuStripButton_generateFacets_Click(object sender, EventArgs e)
        {
            if (Interaction.MsgBox("You are about to create the Mul Files\r\nAre you sure ?", MsgBoxStyle.YesNo, "Make UO Map") == MsgBoxResult.Yes)
            {
                new Thread(new ThreadStart(compileGameFacetFiles)).Start();
            }
        }

        private void compileYourNewMap_radioButtton_staticToggleOn_CheckedChanged(object sender, EventArgs e)
        {
            this.i_RandomStatic = true;
            System.Media.SystemSounds.Beep.Play();
        }

        private void compileYourNewMap_radioButtton_staticToggleOff_CheckedChanged(object sender, EventArgs e)
        {
            this.i_RandomStatic = false;
            System.Media.SystemSounds.Beep.Play();
        }

        private void compileGameFacetFiles()
        {
            short altID = 0;
            string str;
            byte num = 0;
            int num1;
            int num2;
            int num3;
            int num4;

            IEnumerator enumerator = null;
            TransitionTable transitionTable = new TransitionTable();
            DateTime now = DateTime.Now;

            SafeUpdate(() => iLogger.StartTask());
            SafeUpdate(() => iLogger.LogMessage("Loading Terrain Image."));

            try
            {
                str = string.Format("{0}\\{1}", this.compileYourNewMap_textBox_projectLocation.Text, this.compileYourNewMap_textBox_terrainBitmap.Text);

                SafeUpdate(() => iLogger.LogMessage(str));

                this.i_Terrain = new Bitmap(str);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;

                SafeUpdate(() => iLogger.LogMessage("Problem with Loading Terrain Image."));
                SafeUpdate(() => iLogger.LogMessage(exception.Message));

                ProjectData.ClearProjectError();
                return;
            }

            SafeUpdate(() => iLogger.LogMessage("Loading Altitude Image."));

            try
            {
                str = string.Format("{0}\\{1}", this.compileYourNewMap_textBox_projectLocation.Text, this.compileYourNewMap_textBox_altitudeBitmap.Text);

                SafeUpdate(() => iLogger.LogMessage(str));

                this.i_Altitude = new Bitmap(str);
            }
            catch (Exception exception3)
            {
                ProjectData.SetProjectError(exception3);
                Exception exception2 = exception3;

                SafeUpdate(() => iLogger.LogMessage("Problem with Loading Altitude Image."));
                SafeUpdate(() => iLogger.LogMessage(exception2.Message));

                ProjectData.ClearProjectError();
                return;
            }

            SafeUpdate(() => iLogger.LogTimeStamp());
            SafeUpdate(() => iLogger.LogMessage("Preparing Image Files."));
            SafeUpdate(() => iLogger.StartTask());

            int width = this.i_Terrain.Width;
            int height = this.i_Terrain.Height;
            Rectangle rectangle = new Rectangle(0, 0, width, height);
            BitmapData bitmapDatum = this.i_Terrain.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr scan0 = bitmapDatum.Scan0;
            int width1 = checked(bitmapDatum.Width * bitmapDatum.Height);
            byte[] numArray = new byte[checked(checked(width1 - 1) + 1)];
            Marshal.Copy(scan0, numArray, 0, width1);
            BitmapData bitmapDatum1 = this.i_Altitude.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr intPtr = bitmapDatum1.Scan0;
            int width2 = checked(bitmapDatum1.Width * bitmapDatum1.Height);
            byte[] numArray1 = new byte[checked(checked(width2 - 1) + 1)];
            Marshal.Copy(intPtr, numArray1, 0, width2);

            SafeUpdate(() => iLogger.LogTimeStamp());
            SafeUpdate(() => iLogger.LogMessage("Creating Master Terrain Table."));
            SafeUpdate(() => iLogger.StartTask());

            MapCell[,] mapCell = new MapCell[checked(width + 1), checked(height + 1)];
            ClsAltitudeTable clsAltitudeTable = new ClsAltitudeTable();
            clsAltitudeTable.Load();

            try
            {
                int num5 = checked(width - 1);

                for (int i = 0; i <= num5; i++)
                {
                    int num6 = checked(height - 1);

                    for (int j = 0; j <= num6; j++)
                    {
                        int num7 = checked(checked(j * width) + i);

                        //ClsAltitude getAltitude = clsAltitudeTable[numArray1[num7]];
                        ClsAltitude getAltitude = clsAltitudeTable.GetAltitude(numArray1[num7]);

                        mapCell[i, j] = new MapCell(numArray[num7], getAltitude.GetAltitude);
                    }
                }
            }
            catch (Exception exception4)
            {
                ProjectData.SetProjectError(exception4);

                SafeUpdate(() => iLogger.LogMessage("Altitude image needs to be rebuilt"));

                ProjectData.ClearProjectError();
                return;
            }

            this.i_Terrain.Dispose();
            this.i_Altitude.Dispose();

            SafeUpdate(() => iLogger.LogTimeStamp());

            width--;
            height--;
            int num8 = checked((int)Math.Round((double)width / 8 - 1));
            int num9 = checked((int)Math.Round((double)height / 8 - 1));

            SafeUpdate(() => iLogger.LogMessage("Load Transition Tables."));
            SafeUpdate(() => iLogger.StartTask());
       
            if (true) // Transitions now load from embedded FacetData, not disk
            {
                // Transitions now load from embedded FacetData, not disk
                transitionTable.MassLoad(string.Empty);

                SafeUpdate(() => iLogger.LogTimeStamp());
                SafeUpdate(() => iLogger.LogMessage("Preparing Static Tables"));

                Collection[,] collections = new Collection[checked(num8 + 1), checked(num9 + 1)];
                int num10 = num8;

                for (int k = 0; k <= num10; k++)
                {
                    int num11 = num9;

                    for (int l = 0; l <= num11; l++)
                    {
                        collections[k, l] = new Collection();
                    }
                }

                SafeUpdate(() => iLogger.LogMessage("Applying Transition Tables."));
                SafeUpdate(() => iLogger.StartTask());
                SafeUpdate(() => this.compileYourNewMap_progressBar.Maximum = width);

                ClsTerrainTable clsTerrainTable = new ClsTerrainTable();
                clsTerrainTable.Load();
                MapTile mapTile = new MapTile();

                #region Transition Issues Patched

                // Transition.Transition mytransition = new Transition.Transition();
                // Transition.Transition transition = new Transition.Transition();

                Transition transition = new Transition();

                #endregion

                short[] numArray2 = new short[16];
                short num12 = checked((short)width);
                for (short m = 0; m <= num12; m = checked((short)(m + 1)))
                {
                    num1 = (m != 0 ? checked(m - 1) : width);
                    num2 = (m != width ? checked(m + 1) : 0);
                    short num13 = checked((short)height);
                    for (short n = 0; n <= num13; n = checked((short)(n + 1)))
                    {
                        num4 = (n != 0 ? checked(n - 1) : height);
                        num3 = (n != height ? checked(n + 1) : 0);
                        object[] groupID = new object[] { mapCell[num1, num4].GroupID, mapCell[m, num4].GroupID, mapCell[num2, num4].GroupID, mapCell[num1, n].GroupID, mapCell[m, n].GroupID, mapCell[num2, n].GroupID, mapCell[num1, num3].GroupID, mapCell[m, num3].GroupID, mapCell[num2, num3].GroupID };
                        string str1 = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}{8:X2}", groupID);

                        try
                        {
                            #region Transition Issues Patched

                            // transition = transitionTable[str1];
                            // transitionTable.MassLoad(str1);
                            // transition = (transitionTable.MassLoad(str1));
                            // transition = transitionTable.MassLoad(str1);                  
                            // transition = transitionTable.Transition(str1);
                            // transition = (Transition.Transition)(transitionTable.GetTransitionTable[str1]);

                            transition = (Transition)transitionTable.GetTransitionTable[str1];

                            #endregion

                            if (transition == null)
                            {
                                //ClsTerrain terrianGroup = clsTerrainTable[mapCell[m, n].GroupID];
                                ClsTerrain terrianGroup = clsTerrainTable.TerrianGroup(mapCell[m, n].GroupID);

                                mapCell[m, n].TileID = terrianGroup.TileID;
                                mapCell[m, n].AltID = altID;
                                terrianGroup = null;
                            }
                            else
                            {
                                altID = mapCell[m, n].AltID;
                                mapTile = transition.GetRandomMapTile();
                                if (mapTile == null)
                                {
                                    //ClsTerrain clsTerrain = clsTerrainTable[mapCell[m, n].GroupID];
                                    ClsTerrain clsTerrain = clsTerrainTable.TerrianGroup(mapCell[m, n].GroupID);
                                    mapCell[m, n].TileID = clsTerrain.TileID;
                                    mapCell[m, n].ChangeAltID((short)clsTerrain.AltID);
                                    clsTerrain = null;
                                }
                                else
                                {
                                    MapTile mapTile1 = mapTile;
                                    mapCell[m, n].TileID = mapTile1.TileID;
                                    mapCell[m, n].ChangeAltID(mapTile1.AltIDMod);
                                    mapTile1 = null;
                                }
                                transition.GetRandomStaticTiles(m, n, altID, collections, this.i_RandomStatic);
                            }
                            if (mapCell[m, n].GroupID == 254)
                            {
                                mapCell[m, n].TileID = 1078;
                                mapCell[m, n].AltID = 0;
                            }
                        }
                        catch (Exception exception6)
                        {
                            ProjectData.SetProjectError(exception6);
                            Exception exception5 = exception6;
                            buildLogger iLogger = this.iLogger;
                            groupID = new object[] { m, n, altID, str1 };

                            SafeUpdate(() => iLogger.LogMessage(string.Format("\r\nLocation: X:{0}, Y:{1}, Z:{2} Hkey:{3}", m, n, altID, str1)));
                            SafeUpdate(() => iLogger.LogMessage(exception5.ToString()));

                            ProjectData.ClearProjectError();
                            return;
                        }
                    }

                    SafeUpdate(() => this.compileYourNewMap_progressBar.Value = m);
                }

                SafeUpdate(() => iLogger.LogTimeStamp());
                SafeUpdate(() => iLogger.LogMessage("Second Pass."));
                SafeUpdate(() => iLogger.StartTask());

                short[] altID1 = new short[9];
                RoughEdge roughEdge = new RoughEdge();
                short num14 = checked((short)width);

                for (short o = 0; o <= num14; o = checked((short)(o + 1)))
                {
                    num1 = (o != 0 ? checked(o - 1) : width);
                    num2 = (o != width ? checked(o + 1) : 0);
                    short num15 = checked((short)height);

                    for (short p = 0; p <= num15; p = checked((short)(p + 1)))
                    {
                        num4 = (p != 0 ? checked(p - 1) : height);
                        num3 = (p != height ? checked(p + 1) : 0);

                        mapCell[o, p].ChangeAltID(roughEdge.CheckCorner(mapCell[num1, num4].TileID));
                        mapCell[o, p].ChangeAltID(roughEdge.CheckLeft(mapCell[num1, p].TileID));
                        mapCell[o, p].ChangeAltID(roughEdge.CheckTop(mapCell[o, num4].TileID));

                        if (mapCell[o, p].GroupID == 20)
                        {
                            altID1[0] = mapCell[num1, num4].AltID;
                            altID1[1] = mapCell[o, num4].AltID;
                            altID1[2] = mapCell[num2, num4].AltID;
                            altID1[3] = mapCell[num1, p].AltID;
                            altID1[4] = mapCell[o, p].AltID;
                            altID1[5] = mapCell[num2, p].AltID;
                            altID1[6] = mapCell[num1, num3].AltID;
                            altID1[7] = mapCell[o, num3].AltID;
                            altID1[8] = mapCell[num2, num3].AltID;

                            Array.Sort(altID1);
                            float single = 10f * VBMath.Rnd();

                            if (single == 0f)
                            {
                                mapCell[o, p].AltID = checked((short)(checked(altID1[8] - 4)));
                            }
                            else if (single >= 1f && single <= 2f)
                            {
                                mapCell[o, p].AltID = checked((short)(checked(altID1[8] - 2)));
                            }
                            else if (single >= 3f && single <= 7f)
                            {
                                mapCell[o, p].AltID = altID1[8];
                            }
                            else if (single >= 8f && single <= 9f)
                            {
                                mapCell[o, p].AltID = checked((short)(checked(altID1[8] + 2)));
                            }
                            else if (single == 10f)
                            {
                                mapCell[o, p].AltID = checked((short)(checked(altID1[8] + 4)));
                            }
                        }

                        //if (clsTerrainTable[mapCell[o, p].GroupID].RandAlt)
                        if (clsTerrainTable.TerrianGroup(mapCell[o, p].GroupID).RandAlt)
                        {
                            float single1 = 10f * VBMath.Rnd();

                            if (single1 == 0f)
                            {
                                mapCell[o, p].ChangeAltID(-4);
                            }
                            else if (single1 >= 1f && single1 <= 2f)
                            {
                                mapCell[o, p].ChangeAltID(-2);
                            }
                            else if (single1 >= 8f && single1 <= 9f)
                            {
                                mapCell[o, p].ChangeAltID(2);
                            }
                            else if (single1 == 10f)
                            {
                                mapCell[o, p].ChangeAltID(4);
                            }
                        }
                    }

                    SafeUpdate(() => this.compileYourNewMap_progressBar.Value = o);
                }

                SafeUpdate(() => iLogger.LogTimeStamp());

                int num16 = 1;
                int num17 = width;

                if (num17 == 6143)
                {
                    num = 0;
                }
                else if (num17 == 2303)
                {
                    num = 2;
                }
                else if (num17 == 2559)
                {
                    num = 3;
                }

                SafeUpdate(() => iLogger.LogMessage("\r\n"));
                SafeUpdate(() => iLogger.LogMessage("Load . . . . . Import Tiles."));
                SafeUpdate(() => iLogger.StartTask());

                ImportTiles importTile = new ImportTiles(collections, this.compileYourNewMap_textBox_projectLocation.Text);

                SafeUpdate(() => iLogger.LogTimeStamp());
                SafeUpdate(() => iLogger.LogMessage("\r\n"));
                SafeUpdate(() => iLogger.LogMessage("Write .mul Files."));
                SafeUpdate(() => iLogger.StartTask());

                str = string.Format("{0}/Map{1}.mul", this.compileYourNewMap_textBox_projectLocation.Text, num);

                SafeUpdate(() => iLogger.LogMessage(str));

                FileStream fileStream = new FileStream(str, FileMode.Create);
                BinaryWriter binaryWriter = new BinaryWriter(fileStream);

                int num18 = width;

                for (int q = 0; q <= num18; q = checked(q + 8))
                {
                    int num19 = height;

                    for (int r = 0; r <= num19; r = checked(r + 8))
                    {
                        binaryWriter.Write(num16);
                        int num20 = 0;

                        do
                        {
                            int num21 = 0;

                            do
                            {
                                mapCell[checked(q + num21), checked(r + num20)].WriteMapMul(binaryWriter);
                                num21++;
                            }
                            while (num21 <= 7);
                            num20++;
                        }
                        while (num20 <= 7);
                    }
                }

                binaryWriter.Close();
                fileStream.Close();
                str = string.Format("{0}/StaIdx{1}.mul", this.compileYourNewMap_textBox_projectLocation.Text, num);
                FileStream fileStream1 = new FileStream(str, FileMode.Create);

                SafeUpdate(() => iLogger.LogMessage(str));

                str = string.Format("{0}/Statics{1}.mul", this.compileYourNewMap_textBox_projectLocation.Text, num);
                FileStream fileStream2 = new FileStream(str, FileMode.Create);

                SafeUpdate(() => iLogger.LogMessage(str));

                BinaryWriter binaryWriter1 = new BinaryWriter(fileStream1);
                BinaryWriter binaryWriter2 = new BinaryWriter(fileStream2);

                int num22 = num8;

                for (int s = 0; s <= num22; s++)
                {
                    int num23 = num9;

                    for (int t = 0; t <= num23; t++)
                    {
                        int num24 = 0;
                        int position = checked((int)binaryWriter2.BaseStream.Position);

                        try
                        {
                            enumerator = collections[s, t].GetEnumerator();

                            while (enumerator.MoveNext())
                            {
                                ((StaticCell)enumerator.Current).Write(binaryWriter2);

                                num24 = checked(num24 + 7);
                            }
                        }
                        finally
                        {
                            if (enumerator is IDisposable)
                            {
                                ((IDisposable)enumerator).Dispose();
                            }
                        }

                        if (num24 == 0)
                        {
                            position = -1;
                        }

                        binaryWriter1.Write(position);
                        binaryWriter1.Write(num24);
                        binaryWriter1.Write(num16);
                    }
                }

                binaryWriter2.Close();
                binaryWriter1.Close();
                fileStream2.Close();
                fileStream1.Close();

                SafeUpdate(() => iLogger.LogTimeStamp());
                SafeUpdate(() => iLogger.LogMessage("Done."));
            }
        }

        private void SafeUpdate(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void compileYourNewMap_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                iLogger.Dispose();
            }
        }
    }
}
