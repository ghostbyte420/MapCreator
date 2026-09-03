using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapCreator.Engine.Compiler
{
    public static class ControlExtensions
    {
        public static void UI(this Control c, Action a)
        {
            if (c.IsDisposed) return;
            if (!c.IsHandleCreated) c.CreateControl(); // ensure handle
            if (c.InvokeRequired) c.BeginInvoke(a); else a();
        }
    }
}