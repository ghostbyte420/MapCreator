using MapCreator.Controls.UserSubmittedUtilities.OrbifyYourFacet;
using MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator;
using MapCreator.Controls.UserSubmittedUtilities.TransitionEditor;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MapCreator.Controls.UserSubmittedUtilities
{
    public partial class userSubmittedUtilityToolbox : UserControl
    {
        public userSubmittedUtilityToolbox()
        {
            InitializeComponent();
        }

        private void userSubmittedUtilityToolbox_panel_menuStrip_menuStripButton_orbifyYourFacet_Click(object sender, EventArgs e)
        {
            var parentForm = this.FindForm() as mapCreatorMain;
            if (parentForm != null)
            {
                // Get or create userSubmittedUtilityUI in UtilityPanel
                var utilityUI = parentForm.UtilityPanel.Controls.OfType<userSubmittedUtilityUI>().FirstOrDefault();
                if (utilityUI == null)
                {
                    utilityUI = new userSubmittedUtilityUI { Dock = DockStyle.Fill };
                    parentForm.UtilityPanel.Controls.Add(utilityUI);
                }

                // Load PRE-BUILT orbifyYourFacet (instant!)
                var prebuiltPlugin = parentForm.GetPrebuiltPlugin("OrbifyYourFacet");
                if (prebuiltPlugin != null)
                {
                    utilityUI.LoadUtility(prebuiltPlugin);
                }
            }
        }

        private void userSubmittedUtilityToolbox_panel_menuStrip_menuStripButton_terrainGenerator_Click(object sender, EventArgs e)
        {
            var parentForm = this.FindForm() as mapCreatorMain;
            if (parentForm != null)
            {
                // Get or create userSubmittedUtilityUI in UtilityPanel
                var utilityUI = parentForm.UtilityPanel.Controls.OfType<userSubmittedUtilityUI>().FirstOrDefault();
                if (utilityUI == null)
                {
                    utilityUI = new userSubmittedUtilityUI { Dock = DockStyle.Fill };
                    parentForm.UtilityPanel.Controls.Add(utilityUI);
                }

                // Load PRE-BUILT terrainGenerator (instant!)
                var prebuiltPlugin = parentForm.GetPrebuiltPlugin("TerrainGenerator");
                if (prebuiltPlugin != null)
                {
                    utilityUI.LoadUtility(prebuiltPlugin);
                }
            }
        }

        private void userSubmittedUtilityToolbox_panel_menuStrip_menuStripButton_transitionEditor_Click(object sender, EventArgs e)
        {
            var parentForm = this.FindForm() as mapCreatorMain;
            if (parentForm != null)
            {
                // Get or create userSubmittedUtilityUI in UtilityPanel
                var utilityUI = parentForm.UtilityPanel.Controls.OfType<userSubmittedUtilityUI>().FirstOrDefault();
                if (utilityUI == null)
                {
                    utilityUI = new userSubmittedUtilityUI { Dock = DockStyle.Fill };
                    parentForm.UtilityPanel.Controls.Add(utilityUI);
                }

                // Load PRE-BUILT transitionEditor (instant!)
                var prebuiltPlugin = parentForm.GetPrebuiltPlugin("TransitionEditor");
                if (prebuiltPlugin != null)
                {
                    utilityUI.LoadUtility(prebuiltPlugin);
                }
            }
        }

        private void userSubmittedUtilityToolbox_panel_menuStrip_menuStripButton_resetAllPlugins_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Reset all plugins? This will clear all unsaved work in:\n\n• Transition Editor\n• Orbify Your Facet\n• Terrain Generator",
                "Reset All Plugins",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                var parentForm = this.FindForm() as mapCreatorMain;
                if (parentForm != null)
                {
                    parentForm.ResetAllPlugins();
                    MessageBox.Show(
                        "All plugins reset successfully!\n\nClick a plugin button to reload it.",
                        "Reset Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
    }
}