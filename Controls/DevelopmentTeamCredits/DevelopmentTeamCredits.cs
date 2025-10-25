using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapCreator.Controls.DevelopmentTeamCredits
{
    public partial class developmentTeamCredits : UserControl
    {
        public developmentTeamCredits()
        {
            InitializeComponent();
        }

        private void developmentTeamCredits_pictureBoxLink_uoAvox_Click(object sender, EventArgs e)
        {
            ProcessStartInfo getsponsor = new ProcessStartInfo
            {
                FileName = "https://uoavox.studio",
                UseShellExecute = true
            };

            Process.Start(getsponsor);
        }
    }
}
