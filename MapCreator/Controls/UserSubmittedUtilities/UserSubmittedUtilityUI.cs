using System;
using System.Windows.Forms;

namespace MapCreator.Controls.UserSubmittedUtilities
{
    public partial class userSubmittedUtilityUI : UserControl
    {
        public userSubmittedUtilityUI()
        {
            InitializeComponent();

            // Give THIS control a solid background (prevents background image flashing during plugin loads)
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
        }

        // Method to load any user control into UtilityPanel
        public void LoadUtility(UserControl utilityControl)
        {
            // PAUSE DRAWING to prevent flicker
            this.SuspendLayout();

            try
            {
                // Clear only THIS control's children (not UtilityPanel!)
                this.Controls.Clear();

                // If the plugin already has a parent, remove it first
                if (utilityControl.Parent != null)
                {
                    utilityControl.Parent.Controls.Remove(utilityControl);
                }

                // Add the new utility control to THIS userSubmittedUtilityUI
                utilityControl.Dock = DockStyle.Fill;
                this.Controls.Add(utilityControl);
            }
            finally
            {
                // RESUME DRAWING and refresh everything at once
                this.ResumeLayout(true);
            }
        }
    }
}
