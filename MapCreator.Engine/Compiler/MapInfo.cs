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
    public class MapInfo
    {

        #region Getters And Setters

        public string MapName { get; }

        public byte MapNumber { get; }

        public int XSize { get; }

        public int YSize { get; }

        #endregion

        public MapInfo(XmlElement iXml)
        {
            MapName = iXml.GetAttribute("Name");
            MapNumber = XmlConvert.ToByte(iXml.GetAttribute("Num"));
            XSize = XmlConvert.ToInt32(iXml.GetAttribute("XSize"));
            YSize = XmlConvert.ToInt32(iXml.GetAttribute("YSize"));
        }

        public override string ToString()
        {
            return string.Format("{0}", MapName);
        }
    }
}