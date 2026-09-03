using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using static System.Net.Mime.MediaTypeNames;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using MapCreator.Engine.UltimaSDK;

namespace MapCreator.Engine.Compiler
{
    public class ClsAltitude
    {
        private int m_Key;

        private Color m_AltColor;

        private string m_Type;

        private short m_Alt;

        [Category("ID")]
        public Color AltitudeColor
        {
            get
            {
                return this.m_AltColor;
            }
            set
            {
                this.m_AltColor = value;
            }
        }

        [Category("Method")]
        public short GetAltitude
        {
            get
            {
                return this.m_Alt;
            }
            set
            {
                this.m_Alt = value;
            }
        }

        [Category("Key")]
        public int Key
        {
            get
            {
                return this.m_Key;
            }
            set
            {
                this.m_Key = value;
            }
        }

        [Category("ID")]
        public string Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }

        public ClsAltitude(int iKey, string iType, short iAlt, Color iAltColor)
        {
            this.m_Key = iKey;
            this.m_Type = iType;
            this.m_Alt = iAlt;
            this.m_AltColor = iAltColor;
        }

        public ClsAltitude(XmlElement xmlInfo)
        {
            this.m_Key = XmlConvert.ToInt32(xmlInfo.GetAttribute("Key"));
            this.m_Type = xmlInfo.GetAttribute("Type");
            this.m_Alt = XmlConvert.ToInt16(xmlInfo.GetAttribute("Altitude"));
            this.m_AltColor = Color.FromArgb((int)XmlConvert.ToByte(xmlInfo.GetAttribute("R")), (int)XmlConvert.ToByte(xmlInfo.GetAttribute("G")), (int)XmlConvert.ToByte(xmlInfo.GetAttribute("B")));
        }

        public void Save(XmlTextWriter xmlInfo)
        {
            xmlInfo.WriteStartElement("Altitude");
            xmlInfo.WriteAttributeString("Key", StringType.FromInteger(this.m_Key));
            xmlInfo.WriteAttributeString("Type", this.m_Type);
            xmlInfo.WriteAttributeString("Altitude", StringType.FromInteger(this.m_Alt));
            xmlInfo.WriteAttributeString("R", StringType.FromByte(this.m_AltColor.R));
            xmlInfo.WriteAttributeString("G", StringType.FromByte(this.m_AltColor.G));
            xmlInfo.WriteAttributeString("B", StringType.FromByte(this.m_AltColor.B));
            xmlInfo.WriteEndElement();
        }

        public void SaveACO(BinaryWriter iACTFile)
        {
            byte num = 0;
            iACTFile.Write(this.m_AltColor.R);
            iACTFile.Write(this.m_AltColor.R);
            iACTFile.Write(this.m_AltColor.G);
            iACTFile.Write(this.m_AltColor.G);
            iACTFile.Write(this.m_AltColor.B);
            iACTFile.Write(this.m_AltColor.B);
            iACTFile.Write(num);
            iACTFile.Write(num);
        }

        public void SaveACOText(BinaryWriter iACTFile)
        {
            byte num = 0;
            iACTFile.Write(this.m_AltColor.R);
            iACTFile.Write(this.m_AltColor.R);
            iACTFile.Write(this.m_AltColor.G);
            iACTFile.Write(this.m_AltColor.G);
            iACTFile.Write(this.m_AltColor.B);
            iACTFile.Write(this.m_AltColor.B);
            iACTFile.Write(num);
            iACTFile.Write(num);
            iACTFile.Write(num);
            iACTFile.Write(num);
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding(true, true);
            string str = string.Format("{0} {1}", this.m_Type, this.m_Alt);
            byte[] bytes = unicodeEncoding.GetBytes(str);
            byte num1 = Convert.ToByte(bytes.Length);
            byte num2 = checked((byte)Math.Round((double)num1 / 2 + 1));
            iACTFile.Write(num);
            iACTFile.Write(num2);
            byte[] numArray = bytes;
            for (int i = 0; i < (int)numArray.Length; i++)
            {
                iACTFile.Write(numArray[i]);
            }
            iACTFile.Write(num);
            iACTFile.Write(num);
        }

        public void SaveACT(BinaryWriter iACTFile)
        {
            iACTFile.Write(this.m_AltColor.R);
            iACTFile.Write(this.m_AltColor.G);
            iACTFile.Write(this.m_AltColor.B);
        }

        public override string ToString()
        {
            string str = string.Format("[{0:X3}] {1} {2}", this.m_Key, this.m_Type, this.m_Alt);
            return str;
        }
    }

    public class ClsAltitudeTable
    {
        private Hashtable i_AltitudeTable;

        public Hashtable AltitudeHash
        {
            get
            {
                return this.i_AltitudeTable;
            }
        }

        public void SetAltitude(int Index, ClsAltitude Value)
        {
            this.i_AltitudeTable[Index] = Value;
        }

        public ClsAltitude GetAltitude(int Index)
        {
            return (ClsAltitude)this.i_AltitudeTable[Index];
        }

        public ClsAltitudeTable()
        {
            this.i_AltitudeTable = new Hashtable();
        }

        public void Display(ListBox iList)
        {
            IEnumerator enumerator = null;
            iList.Items.Clear();
            try
            {
                enumerator = this.i_AltitudeTable.Values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ClsAltitude current = (ClsAltitude)enumerator.Current;
                    iList.Items.Add(current);
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

        public ColorPalette GetAltPalette()
        {
            IEnumerator enumerator = null;
            // ColorPalette.Entries is read-only in modern .NET (System.Drawing.Common 10+):
            // build entries into a local array and construct the palette via ColorPalette(Color[]).
            Color[] entries = new Color[256];
            try
            {
                enumerator = this.i_AltitudeTable.Values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ClsAltitude current = (ClsAltitude)enumerator.Current;
                    entries[current.Key] = current.AltitudeColor;
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }
            return new ColorPalette(entries);
        }

        public void Load()
        {
            IEnumerator enumerator = null;
            IEnumerator enumerator1 = null;

            #region Directory Modification

            // string str = string.Format("{0}Data/System/Altitude.xml", AppDomain.CurrentDomain.BaseDirectory);

            string str = string.Format("{0}MapCompiler/Engine/Altitude.xml", AppDomain.CurrentDomain.BaseDirectory);

            #endregion

            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument = FacetData.LoadXml("Altitude.xml");
                this.i_AltitudeTable.Clear();
                try
                {
                    enumerator1 = xmlDocument.SelectNodes("Altitudes").GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        XmlElement current = (XmlElement)enumerator1.Current;
                        try
                        {
                            enumerator = current.SelectNodes("Altitude").GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                ClsAltitude clsAltitude = new ClsAltitude((XmlElement)enumerator.Current);
                                this.i_AltitudeTable.Add(clsAltitude.Key, clsAltitude);
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
                }
                finally
                {
                    if (enumerator1 is IDisposable)
                    {
                        ((IDisposable)enumerator1).Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                ProjectData.SetProjectError(exception);
                Interaction.MsgBox(string.Format("XMLFile:{0}", str), MsgBoxStyle.OkOnly, null);
                ProjectData.ClearProjectError();
            }
        }

        public void Save()
        {
            IEnumerator enumerator = null;

            #region Directory Modification

            // string str = string.Format("{0}Data\\System\\Altitude.xml", AppDomain.CurrentDomain.BaseDirectory);

            string str = string.Format("{0}MapCompiler\\Engine\\Altitude.xml", AppDomain.CurrentDomain.BaseDirectory);

            #endregion

            XmlTextWriter xmlTextWriter = new XmlTextWriter(str, Encoding.UTF8)
            {
                Indentation = 2,
                Formatting = Formatting.Indented
            };
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteStartElement("Altitudes");
            try
            {
                enumerator = this.i_AltitudeTable.Values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ((ClsAltitude)enumerator.Current).Save(xmlTextWriter);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    ((IDisposable)enumerator).Dispose();
                }
            }
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.Close();
        }

        public void SaveACO()
        {
            byte num = Convert.ToByte(this.i_AltitudeTable.Count);

            #region Directory Modification

            // string str = string.Format("{0}/Data/Photoshop/Altitude.ACO", Directory.GetCurrentDirectory());

            string str = string.Format("{0}/Development/DrawingTools/AdobePhotoshop/ColorTables/ACO/Altitude.aco", Directory.GetCurrentDirectory());

            #endregion

            FileStream fileStream = new FileStream(str, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)1);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(num);
            int num1 = 0;
            do
            {
                if (this.i_AltitudeTable[num1] != null)
                {
                    binaryWriter.Write((byte)0);
                    binaryWriter.Write((byte)0);
                    ((ClsAltitude)this.i_AltitudeTable[num1]).SaveACO(binaryWriter);
                }
                num1++;
            }
            while (num1 <= 255);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)2);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(num);
            int num2 = 0;
            do
            {
                if (this.i_AltitudeTable[num2] != null)
                {
                    binaryWriter.Write((byte)0);
                    binaryWriter.Write((byte)0);
                    ((ClsAltitude)this.i_AltitudeTable[num2]).SaveACOText(binaryWriter);
                }
                num2++;
            }
            while (num2 <= 255);
            binaryWriter.Close();
            fileStream.Close();
            Interaction.MsgBox("Altitude.aco Saved", MsgBoxStyle.OkOnly, null);
        }

        public void SaveACT()
        {

            #region Directory Modification

            // string str = string.Format("{0}/Data/Photoshop/Altitude.ACT", Directory.GetCurrentDirectory());

            string str = string.Format("{0}/Development/DrawingTools/AdobePhotoshop/ColorTables/ACT/Altitude.act", Directory.GetCurrentDirectory());

            #endregion

            FileStream fileStream = new FileStream(str, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            byte num = 0;
            int num1 = 0;
            do
            {
                if (this.i_AltitudeTable[num1] != null)
                {
                    ((ClsAltitude)this.i_AltitudeTable[num1]).SaveACT(binaryWriter);
                }
                else
                {
                    binaryWriter.Write(num);
                    binaryWriter.Write(num);
                    binaryWriter.Write(num);
                }
                num1++;
            }
            while (num1 <= 255);
            binaryWriter.Close();
            fileStream.Close();
            Interaction.MsgBox("Altitude.act Saved", MsgBoxStyle.OkOnly, null);
        }
    }

    public static class SyncAltitudeBitmapHelper
    {
        public static void CreateAltitudeImage(string projectPath, string terrainFile, string altitudeFile, ClsAltitudeTable iAltitude, ClsTerrainTable iTerrain, IProgress<int> Progress, IProgress<string> Logger)
        {
            Bitmap bitmap = null;
            Bitmap bitmap1 = null;
            try
            {
                Logger.Report("Load Terrain Image Map.");
                bitmap1 = new Bitmap(string.Format("{0}\\{1}", projectPath, terrainFile));
                bitmap = new Bitmap(bitmap1.Width, bitmap1.Height, PixelFormat.Format8bppIndexed);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                Logger.Report("Error in loading image maps.\r\n");
                Logger.Report(exception.Message);
                ProjectData.ClearProjectError();
            }
            bitmap.Palette = iAltitude.GetAltPalette();
            int width = bitmap1.Width;
            Rectangle rectangle = new Rectangle(0, 0, width, bitmap1.Height);
            BitmapData bitmapDatum = bitmap1.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr scan0 = bitmapDatum.Scan0;
            int num = checked(bitmapDatum.Width * bitmapDatum.Height);
            byte[] numArray = new byte[checked(checked(num - 1) + 1)];
            Marshal.Copy(scan0, numArray, 0, num);
            BitmapData bitmapDatum1 = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            IntPtr intPtr = bitmapDatum1.Scan0;
            int width1 = checked(bitmapDatum1.Width * bitmapDatum1.Height);
            byte[] numArray1 = new byte[checked(checked(width1 - 1) + 1)];
            Marshal.Copy(intPtr, numArray1, 0, width1);
            Logger.Report("Creating Altitude Image Map.");
            int num1 = checked(num - 1);
            for (int i = 0; i <= num1; i++)
            {
                if ((i % 1000) == 0) Progress.Report(i * 100 / width1);
                byte altID = iTerrain.TerrianGroup(numArray[i]).AltID;
                numArray1[i] = altID;
            }
            Marshal.Copy(numArray1, 0, intPtr, width1);
            bitmap.UnlockBits(bitmapDatum1);
            try
            {
                string str = string.Format("{0}\\{1}", projectPath, altitudeFile);
                Logger.Report("Saving Altitude Image Map.\r\n");
                Logger.Report(str);
                bitmap.Save(str, ImageFormat.Bmp);
            }
            catch (Exception exception3)
            {
                ProjectData.SetProjectError(exception3);
                Exception exception2 = exception3;
                Logger.Report("Error in saving image.\r\n");
                Logger.Report(exception2.Message);
                ProjectData.ClearProjectError();
            }
            bitmap.Dispose();
            bitmap1.Dispose();
            Logger.Report("Done.");
        }
    }
}