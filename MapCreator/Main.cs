using MapCreator.Controls;
using MapCreator.Controls.CompileYourNewMap;
using MapCreator.Controls.ConfigureColorTables;
using MapCreator.Controls.DevelopmentTeamCredits;
using MapCreator.Controls.EncodeAltitudeBitmap;
using MapCreator.Controls.UserSubmittedUtilities;
using MapCreator.Engine.Plugin.FacetDesigner;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace MapCreator
{
    public partial class mapCreatorMain : Form
    {
        private List<Control> originalPanel1Controls = new List<Control>();
        public Panel UtilityPanel { get; private set; }

        // Pre-built plugins (created once at startup, reused forever)
        private UserControl prebuiltTransitionEditor;
        private UserControl prebuiltOrbifyYourFacet;
        private UserControl prebuiltTerrainGenerator;

        public mapCreatorMain()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            mapCreatorMain_statusStrip_statusStripButton_back.Image = Properties.Resources.btn_0006;
            this.mapCreatorMain_statusStrip_statusStripButton_back.ButtonClick += mapCreatorMain_statusStrip_statusStripButton_back_ButtonClick;
            this.mapCreatorMain_statusStrip_statusStripButton_back.MouseEnter += BackButton_MouseEnter;
            this.mapCreatorMain_statusStrip_statusStripButton_back.MouseLeave += BackButton_MouseLeave;
            this.mapCreatorMain_statusStrip_statusStripButton_back.MouseDown += BackButton_MouseDown;
            this.mapCreatorMain_statusStrip_statusStripButton_back.MouseUp += BackButton_MouseUp;
            mapCreatorMain_statusStrip_statusStripButton_back.Visible = false;

            foreach (Control control in mapCreatorMain_splitContainer.Panel1.Controls)
                originalPanel1Controls.Add(control);

            mapCreatorMain_splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            if (mapCreatorMain_splitContainer.Panel2.BackgroundImage == null)
                mapCreatorMain_splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);

            UtilityPanel = new Panel { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.Transparent };
            EnableDoubleBuffering(UtilityPanel);

            mapCreatorMain_splitContainer.Panel2.Controls.Add(UtilityPanel);
            EnableDoubleBuffering(mapCreatorMain_splitContainer.Panel2);

            // Pre-create all plugins ONCE at startup (they load in the background)
            PreCreatePlugins();
        }

        private void EnableDoubleBuffering(Control control)
        {
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                control,
                new object[] { true }
            );
        }
           
        private void PreCreatePlugins()
        {
            // Build all three plugins during startup
            // They sit in memory, fully initialized but invisible
            prebuiltTransitionEditor = new MapCreator.Controls.UserSubmittedUtilities.TransitionEditor.transitionEditor();
            prebuiltOrbifyYourFacet = new MapCreator.Controls.UserSubmittedUtilities.OrbifyYourFacet.orbifyYourFacet();
            prebuiltTerrainGenerator = new MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator.terrainGenerator();
        }
  
        public UserControl GetPrebuiltPlugin(string pluginName)
        {
            // Return the matching pre-built plugin
            switch (pluginName)
            {
                case "TransitionEditor":
                    return prebuiltTransitionEditor;
                case "OrbifyYourFacet":
                    return prebuiltOrbifyYourFacet;
                case "TerrainGenerator":
                    return prebuiltTerrainGenerator;
                default:
                    return null;
            }
        }

        public void ResetAllPlugins()
        {
            // First, clear the UI so it's not holding references to old plugins
            var utilityUI = UtilityPanel.Controls.OfType<MapCreator.Controls.UserSubmittedUtilities.userSubmittedUtilityUI>().FirstOrDefault();
            if (utilityUI != null)
            {
                utilityUI.Controls.Clear();
            }

            // Dispose all old plugins
            prebuiltTransitionEditor?.Dispose();
            prebuiltOrbifyYourFacet?.Dispose();
            prebuiltTerrainGenerator?.Dispose();

            // Create fresh new ones
            prebuiltTransitionEditor = new MapCreator.Controls.UserSubmittedUtilities.TransitionEditor.transitionEditor();
            prebuiltOrbifyYourFacet = new MapCreator.Controls.UserSubmittedUtilities.OrbifyYourFacet.orbifyYourFacet();
            prebuiltTerrainGenerator = new MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator.terrainGenerator();
        }

        private void ShowControlInsidePanel1(UserControl control)
        {
            mapCreatorMain_splitContainer.Panel1.Controls.Clear();
            control.Dock = DockStyle.Fill;
            mapCreatorMain_splitContainer.Panel1.Controls.Add(control);
            mapCreatorMain_statusStrip_statusStripButton_back.Visible = true;
        }

        private void ShowControlInsidePanel2(UserControl control)
        {
            // Dispose and clear existing controls in UtilityPanel
            foreach (Control c in UtilityPanel.Controls)
            {
                c.Dispose();
            }
            UtilityPanel.Controls.Clear();

            // Add the new control to UtilityPanel
            control.Dock = DockStyle.Fill;
            UtilityPanel.Controls.Add(control);

            // Show the Back button
            mapCreatorMain_statusStrip_statusStripButton_back.Visible = true;
        }

        private void RestorePanel1()
        {
            mapCreatorMain_splitContainer.Panel1.Controls.Clear();
            foreach (Control control in originalPanel1Controls)
                mapCreatorMain_splitContainer.Panel1.Controls.Add(control);
        }

        private void mapCreatorMain_menuStrip_menuStripButton_credits_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new developmentTeamCredits());
        }

        private void mapCreatorMain_splitContainerPanel1_button_configureColorTables_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new configureColorTables());
        }

        private void mapCreatorMain_splitContainerPanel1_button_createMapTemplate_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new createMapTemplate());
        }

        private void mapCreatorMain_splitContainerPanel1_button_drawACustomFacet_Click(object sender, EventArgs e)
        {
            facetDesigner drawACustomFacet = new facetDesigner();
            drawACustomFacet.Show();
        }

        private void mapCreatorMain_splitContainerPanel1_button_syncAltitudeBitmap_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new SyncAltitudeBitmap());
        }

        private void mapCreatorMain_splitContainerPanel1_button_compileYourNewMap_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new compileYourNewMap());
        }

        private void mapCreatorMain_splitContainerPanel1_button_userSubmittedUtilities_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel1(new userSubmittedUtilityToolbox());
            ShowControlInsidePanel2(new userSubmittedUtilityUI());
        }

        private void mapCreatorMain_statusStrip_statusStripButton_back_Click(object sender, EventArgs e) { }

        private void mapCreatorMain_statusStrip_statusStripButton_back_ButtonClick(object sender, EventArgs e)
        {
            RestorePanel1(); // Restore Panel1

            // Just REMOVE controls from UtilityPanel (DON'T dispose pre-built plugins!)
            UtilityPanel.Controls.Clear();

            // Hide the Back button
            mapCreatorMain_statusStrip_statusStripButton_back.Visible = false;
        }

        private void BackButton_MouseEnter(object sender, EventArgs e)
        {
            mapCreatorMain_statusStrip_statusStripButton_back.Image = Properties.Resources.btn_0006a;
        }

        private void BackButton_MouseLeave(object sender, EventArgs e)
        {
            mapCreatorMain_statusStrip_statusStripButton_back.Image = Properties.Resources.btn_0006;
        }

        private void BackButton_MouseDown(object sender, MouseEventArgs e)
        {
            mapCreatorMain_statusStrip_statusStripButton_back.Image = Properties.Resources.btn_0006a;
        }

        private void BackButton_MouseUp(object sender, MouseEventArgs e)
        {
            mapCreatorMain_statusStrip_statusStripButton_back.Image = Properties.Resources.btn_0006a;
        }
    }
}
