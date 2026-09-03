using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MapCreator.Engine.Compiler;
using System.Xml;
using MapCreator.Engine.UltimaSDK;

namespace MapCreator.Controls.UserSubmittedUtilities.TransitionEditor
{
    public partial class transitionEditor : UserControl
    {
        private ClsTerrainTable terrainTable;
        private TransitionTable transitionTable;
        private bool isLoading = false;
        private bool showTerrainIDsMode = true; // Default to ID view
        private Transition currentTransition = null; // Track current for refresh

        public transitionEditor()
        {
            // Enable double buffering to reduce flicker
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeComponent();

            this.Load += transitionEditor_Load;
        }

        private void transitionEditor_Load(object sender, EventArgs e)
        {
            isLoading = true;

            terrainTable = new ClsTerrainTable();
            terrainTable.Load();

            transitionTable = new TransitionTable();
            transitionTable.MassLoad(string.Empty);

            // **ADD THIS:**
            InitializeArtFiles();

            LoadTerrainComboBoxes();

            transitionEditor_rbtn_2wayTransition.Checked = true;

            transitionEditor_lbl_terrainC.Visible = false;
            transitionEditor_cbox_terrainC.Visible = false;

            isLoading = false;
        }

        private void LoadTerrainComboBoxes()
        {
            // Use built-in Display method
            terrainTable.Display(transitionEditor_cbox_terrainA);
            terrainTable.Display(transitionEditor_cbox_terrainB);
            terrainTable.Display(transitionEditor_cbox_terrainC);

            // Set display member to show terrain name
            transitionEditor_cbox_terrainA.DisplayMember = "Name";
            transitionEditor_cbox_terrainB.DisplayMember = "Name";
            transitionEditor_cbox_terrainC.DisplayMember = "Name";
        }

        /// <summary>
        /// Initializes the UltimaSDK art file system from the ClientFileData directory
        /// </summary>
        private void InitializeArtFiles()
        {
            try
            {
                // Art files should be in ClientFileData folder next to the .exe
                string clientDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientFileData");

                if (!Directory.Exists(clientDataPath))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠ ClientFileData directory not found: {clientDataPath}");
                    System.Diagnostics.Debug.WriteLine("  Terrain art will not be available (ID placeholders will be used)");
                    return;
                }

                string artPath = Path.Combine(clientDataPath, "art.mul");
                string artIdxPath = Path.Combine(clientDataPath, "artidx.mul");

                if (!File.Exists(artPath))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠ art.mul not found in: {clientDataPath}");
                    System.Diagnostics.Debug.WriteLine("  Terrain art will not be available (ID placeholders will be used)");
                    return;
                }

                if (!File.Exists(artIdxPath))
                {
                    System.Diagnostics.Debug.WriteLine($"⚠ artidx.mul not found in: {clientDataPath}");
                    System.Diagnostics.Debug.WriteLine("  Terrain art will not be available (ID placeholders will be used)");
                    return;
                }

                // Set the path for the Files system
                MapCreator.Engine.UltimaSDK.Files.SetMulPath(clientDataPath);

                System.Diagnostics.Debug.WriteLine($"✓ Art files initialized successfully:");
                System.Diagnostics.Debug.WriteLine($"  Path: {clientDataPath}");
                System.Diagnostics.Debug.WriteLine($"  art.mul: {new FileInfo(artPath).Length / 1024 / 1024:F1} MB");
                System.Diagnostics.Debug.WriteLine($"  artidx.mul: {new FileInfo(artIdxPath).Length / 1024:F1} MB");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error initializing art files: {ex.Message}");
                System.Diagnostics.Debug.WriteLine("  Terrain art will not be available (ID placeholders will be used)");
            }
        }

        private void transitionEditor_rbtn_2wayTransition_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoading) return;  // Don't respond during initial load

            if (transitionEditor_rbtn_2wayTransition.Checked)
            {
                // Hide Terrain C for 2-way transitions
                transitionEditor_lbl_terrainC.Visible = false;
                transitionEditor_cbox_terrainC.Visible = false;

                // Update the transitions list to show 2-way matches
                UpdateAvailableTransitions();
            }
        }

        private void transitionEditor_rbtn_3wayTransition_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoading) return;  // Don't respond during initial load

            if (transitionEditor_rbtn_3wayTransition.Checked)
            {
                // Show Terrain C for 3-way transitions
                transitionEditor_lbl_terrainC.Visible = true;
                transitionEditor_cbox_terrainC.Visible = true;

                // Populate Terrain C if not already populated
                if (transitionEditor_cbox_terrainC.Items.Count == 0)
                {
                    terrainTable.Display(transitionEditor_cbox_terrainC);
                    transitionEditor_cbox_terrainC.DisplayMember = "Name";
                }

                // Update the transitions list to show 3-way matches
                UpdateAvailableTransitions();
            }
        }

        private void transitionEditor_cbox_terrainA_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAvailableTransitions();
        }

        private void transitionEditor_cbox_terrainB_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAvailableTransitions();
        }

        private void transitionEditor_cbox_terrainC_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAvailableTransitions();
        }

        private void UpdateAvailableTransitions()
        {
            if (isLoading) return;  // Don't update during initial load

            // Clear the list
            transitionEditor_lbox_transitionList.Items.Clear();

            // Make sure we have at least Terrain A and B selected
            if (transitionEditor_cbox_terrainA.SelectedItem == null ||
                transitionEditor_cbox_terrainB.SelectedItem == null)
            {
                return;  // Not enough terrains selected yet
            }

            // For 3-way mode, also check Terrain C
            if (transitionEditor_rbtn_3wayTransition.Checked &&
                transitionEditor_cbox_terrainC.SelectedItem == null)
            {
                return;  // 3-way mode needs all three terrains
            }

            // Get selected terrain IDs
            ClsTerrain terrainA = (ClsTerrain)transitionEditor_cbox_terrainA.SelectedItem;
            ClsTerrain terrainB = (ClsTerrain)transitionEditor_cbox_terrainB.SelectedItem;
            ClsTerrain terrainC = transitionEditor_rbtn_3wayTransition.Checked ? (ClsTerrain)transitionEditor_cbox_terrainC.SelectedItem : null;

            // Loop through all transitions and find matches
            foreach (Transition trans in transitionTable.GetTransitionTable.Values)
            {
                bool matches = false;

                if (transitionEditor_rbtn_2wayTransition.Checked)
                {
                    // 2-way: check if transition uses these two terrains
                    matches = TransitionMatchesTwoWay(trans, terrainA.GroupID, terrainB.GroupID);
                }
                else
                {
                    // 3-way: check if transition uses all three terrains
                    matches = TransitionMatchesThreeWay(trans, terrainA.GroupID, terrainB.GroupID, terrainC.GroupID);
                }

                if (matches)
                {
                    transitionEditor_lbox_transitionList.Items.Add(trans);
                }
            }
        }

        private bool TransitionMatchesTwoWay(Transition trans, int terrainA, int terrainB)
        {
            // For a 2-way transition, check if the HashKey uses only these two terrain types
            // The HashKey has 9 positions (0-8), each containing a terrain byte

            for (int i = 0; i <= 8; i++)
            {
                byte key = trans.GetKey(i);
                if (key != terrainA && key != terrainB)
                {
                    return false;  // Found a terrain that doesn't match
                }
            }
            return true;  // All positions use only terrainA or terrainB
        }

        private bool TransitionMatchesThreeWay(Transition trans, int terrainA, int terrainB, int terrainC)
        {
            // For a 3-way transition, check if the HashKey uses only these three terrain types

            for (int i = 0; i <= 8; i++)
            {
                byte key = trans.GetKey(i);
                if (key != terrainA && key != terrainB && key != terrainC)
                {
                    return false;  // Found a terrain that doesn't match
                }
            }
            return true;  // All positions use only terrainA, terrainB, or terrainC
        }

        private void transitionEditor_lbox_transitionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;  // Don't respond during initial load

            // Make sure something is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                return;
            }

            // Get the selected transition
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;

            // Display the transition details
            DisplayTransitionDetails(selectedTransition);
        }

        private void DisplayTransitionDetails(Transition transition)
        {
            // Display the 3x3 HashKey grid (positions 0-8)
            transitionEditor_lbl_hashKey0.Text = transition.GetKey(0).ToString("X2");
            transitionEditor_lbl_hashKey1.Text = transition.GetKey(1).ToString("X2");
            transitionEditor_lbl_hashKey2.Text = transition.GetKey(2).ToString("X2");
            transitionEditor_lbl_hashKey3.Text = transition.GetKey(3).ToString("X2");
            transitionEditor_lbl_hashKey4.Text = transition.GetKey(4).ToString("X2");
            transitionEditor_lbl_hashKey5.Text = transition.GetKey(5).ToString("X2");
            transitionEditor_lbl_hashKey6.Text = transition.GetKey(6).ToString("X2");
            transitionEditor_lbl_hashKey7.Text = transition.GetKey(7).ToString("X2");
            transitionEditor_lbl_hashKey8.Text = transition.GetKey(8).ToString("X2");

            // Display Map Tiles
            transitionEditor_lbox_mapTiles.Items.Clear();
            foreach (var mapTile in transition.GetMapTiles)
            {
                transitionEditor_lbox_mapTiles.Items.Add(mapTile);
            }

            // Display Static Tiles
            transitionEditor_lbox_staticTiles.Items.Clear();
            foreach (var staticTile in transition.GetStaticTiles)
            {
                transitionEditor_lbox_staticTiles.Items.Add(staticTile);
            }

            // Display Description
            transitionEditor_tbox_description.Text = transition.Description ?? string.Empty;

            // Display Random Statics File and Chance
            if (!string.IsNullOrEmpty(transition.File))
            {
                // Transition has a RandomStatics file reference
                transitionEditor_chkbox_useRandomStatics.Checked = true;
                transitionEditor_tbox_randomStaticsFile.Text = transition.File;

                // Try to load the RandomStatics file to get the Chance value
                try
                {
                    RandomStatics randomStatics = new RandomStatics(transition.File);
                    transitionEditor_numupdwn_globalChance.Value = randomStatics.Freq;
                }
                catch
                {
                    // If file can't be loaded, default to 0
                    transitionEditor_numupdwn_globalChance.Value = 0;
                }
            }
            else
            {
                // No RandomStatics file
                transitionEditor_chkbox_useRandomStatics.Checked = false;
                transitionEditor_tbox_randomStaticsFile.Text = string.Empty;
                transitionEditor_numupdwn_globalChance.Value = 0;
            }

            // Load visual terrain art into the PictureBoxes
            LoadHashKeyVisualEditor(transition);
        }

        /// <summary>
        /// Gets the PictureBox control for a specific hash key position (0-8)
        /// </summary>
        private PictureBox GetHashKeyPictureBox(int position)
        {
            return position switch
            {
                0 => transitionEditor_pbox_hashKey0,
                1 => transitionEditor_pbox_hashKey1,
                2 => transitionEditor_pbox_hashKey2,
                3 => transitionEditor_pbox_hashKey3,
                4 => transitionEditor_pbox_hashKey4,
                5 => transitionEditor_pbox_hashKey5,
                6 => transitionEditor_pbox_hashKey6,
                7 => transitionEditor_pbox_hashKey7,
                8 => transitionEditor_pbox_hashKey8,
                _ => null
            };
        }

        /// <summary>
        /// Loads terrain textures into the 3x3 PictureBox grid based on the transition's HashKey
        /// </summary>
        private void LoadHashKeyVisualEditor(Transition transition)
        {
            // Store current transition for toggle refresh
            currentTransition = transition;

            for (int position = 0; position < 9; position++)
            {
                byte terrainId = transition.GetKey(position);
                PictureBox pictureBox = GetHashKeyPictureBox(position);

                if (pictureBox != null)
                {
                    try
                    {
                        // Get the texture ID for this terrain
                        int textureId = TerrainData.LandTileToTextureId(terrainId);

                        // Check toggle state
                        if (showTerrainIDsMode)
                        {
                            // Show terrain ID placeholder
                            pictureBox.Image = CreatePlaceholderImage(terrainId, textureId);
                            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox.BackColor = Color.Black;
                        }
                        else
                        {
                            // Try to load real terrain art
                            Bitmap texture = TextureLoader.GetLandTexture(textureId);

                            if (texture != null)
                            {
                                pictureBox.Image = texture;
                                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox.BackColor = Color.Black;
                            }
                            else
                            {
                                // Fallback to placeholder if art fails
                                pictureBox.Image = CreatePlaceholderImage(terrainId, textureId);
                                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox.BackColor = Color.DarkGray;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Show error placeholder
                        pictureBox.Image = CreateErrorImage(terrainId, ex.Message);
                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox.BackColor = Color.DarkRed;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a placeholder image when terrain art cannot be loaded
        /// </summary>
        private Bitmap CreatePlaceholderImage(byte terrainId, int tileId)
        {
            Bitmap bmp = new Bitmap(44, 44);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Color based on terrain ID
                Color bgColor = Color.FromArgb(terrainId * 3, terrainId * 2, terrainId);
                g.Clear(bgColor);

                // Draw terrain ID text
                using (Font font = new Font("Arial", 8, FontStyle.Bold))
                {
                    string text = $"T:{terrainId}\nID:{tileId}";
                    SizeF textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, Brushes.White,
                        (44 - textSize.Width) / 2,
                        (44 - textSize.Height) / 2);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Creates an error indicator image
        /// </summary>
        private Bitmap CreateErrorImage(byte terrainId, string error)
        {
            Bitmap bmp = new Bitmap(44, 44);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.DarkRed);
                using (Font font = new Font("Arial", 10, FontStyle.Bold))
                {
                    g.DrawString("ERR", font, Brushes.Yellow, 5, 15);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Toggle between terrain art view and terrain ID debug view
        /// </summary>
        private void transitionEditor_pbox_cbox_tileIDToggle_CheckedChanged(object sender, EventArgs e)
        {
            showTerrainIDsMode = transitionEditor_pbox_cbox_tileIDToggle.Checked;

            // Refresh the visual editor if a transition is loaded
            if (currentTransition != null)
            {
                LoadHashKeyVisualEditor(currentTransition);
            }
        }

        private void transitionEditor_gbox_pluginControl_btn_export_Click(object sender, EventArgs e)
        {
            // Make sure a transition is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a transition to export.", "No Transition Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected transition
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;

            // Build the default filename based on selected terrains
            string defaultFileName = GetTransitionFileName();

            // Create UserXMLFiles folder if it doesn't exist
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserXMLFiles");
            Directory.CreateDirectory(defaultPath);

            // Open Save dialog
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                InitialDirectory = defaultPath,
                FileName = defaultFileName,
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Export Transition",
                DefaultExt = "xml"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Save the transition
                    ExportTransition(selectedTransition, saveDialog.FileName);
                    MessageBox.Show($"Transition exported successfully to:\n{saveDialog.FileName}",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting transition:\n{ex.Message}",
                        "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetTransitionFileName()
        {
            // Get terrain names from selected ComboBoxes
            string terrainAName = transitionEditor_cbox_terrainA.SelectedItem != null
                ? ((ClsTerrain)transitionEditor_cbox_terrainA.SelectedItem).Name
                : "Unknown";

            string terrainBName = transitionEditor_cbox_terrainB.SelectedItem != null
                ? ((ClsTerrain)transitionEditor_cbox_terrainB.SelectedItem).Name
                : "Unknown";

            // For 3-way mode, include Terrain C
            if (transitionEditor_rbtn_3wayTransition.Checked &&
                transitionEditor_cbox_terrainC.SelectedItem != null)
            {
                string terrainCName = ((ClsTerrain)transitionEditor_cbox_terrainC.SelectedItem).Name;
                return $"{terrainAName} ↔ {terrainBName} ↔ {terrainCName}.xml";
            }

            // 2-way mode
            return $"{terrainAName} ↔ {terrainBName}.xml";
        }

        private void ExportTransition(Transition transition, string filePath)
        {
            // Create XML document
            XmlTextWriter writer = new XmlTextWriter(filePath, Encoding.UTF8);
            writer.Indentation = 2;
            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();
            writer.WriteStartElement("Trans");

            // Save the transition
            transition.Save(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private void transitionEditor_btn_addMapTile_Click(object sender, EventArgs e)
        {
            // Make sure a transition is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a transition first.", "No Transition Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected transition
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;

            // Prompt for Tile ID
            string tileInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the Map Tile ID (0-65535):",
                "Add Map Tile",
                "0");

            // User cancelled
            if (string.IsNullOrEmpty(tileInput))
                return;

            // Parse Tile ID
            if (!short.TryParse(tileInput, out short tileID) || tileID < 0)
            {
                MessageBox.Show("Invalid Tile ID. Please enter a number between 0 and 65535.",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prompt for Altitude Modifier
            string altInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the Altitude Modifier (-128 to 127):",
                "Add Map Tile",
                "0");

            // User cancelled
            if (string.IsNullOrEmpty(altInput))
                return;

            // Parse Altitude Modifier
            if (!short.TryParse(altInput, out short altMod) || altMod < -128 || altMod > 127)
            {
                MessageBox.Show("Invalid Altitude Modifier. Please enter a number between -128 and 127.",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Add the tile
            selectedTransition.AddMapTile(tileID, altMod);

            // Refresh the display
            DisplayTransitionDetails(selectedTransition);

            MessageBox.Show($"Map Tile {tileID} added successfully!", "Tile Added",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void transitionEditor_btn_removeMapTile_Click(object sender, EventArgs e)
        {
            // Make sure a transition is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a transition first.", "No Transition Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Make sure a map tile is selected
            if (transitionEditor_lbox_mapTiles.SelectedItem == null)
            {
                MessageBox.Show("Please select a map tile to remove.", "No Tile Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected transition and tile
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;
            MapTile selectedTile = (MapTile)transitionEditor_lbox_mapTiles.SelectedItem;

            // Confirm removal
            DialogResult result = MessageBox.Show(
                $"Remove Map Tile {selectedTile}?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Remove the tile
                selectedTransition.RemoveMapTile(selectedTile);

                // Refresh the display
                DisplayTransitionDetails(selectedTransition);

                MessageBox.Show("Map Tile removed successfully!", "Tile Removed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void transitionEditor_btn_addStaticTile_Click(object sender, EventArgs e)
        {
            // Make sure a transition is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a transition first.", "No Transition Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected transition
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;

            // Prompt for Tile ID
            string tileInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the Static Tile ID (0-65535):",
                "Add Static Tile",
                "0");

            if (string.IsNullOrEmpty(tileInput))
                return;

            if (!short.TryParse(tileInput, out short tileID) || tileID < 0)
            {
                MessageBox.Show("Invalid Tile ID. Please enter a number between 0 and 65535.",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prompt for Altitude Modifier
            string altInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the Altitude Modifier (-128 to 127):",
                "Add Static Tile",
                "0");

            if (string.IsNullOrEmpty(altInput))
                return;

            if (!short.TryParse(altInput, out short altMod) || altMod < -128 || altMod > 127)
            {
                MessageBox.Show("Invalid Altitude Modifier. Please enter a number between -128 and 127.",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Add the tile
            selectedTransition.AddStaticTile(tileID, altMod);

            // Refresh the display
            DisplayTransitionDetails(selectedTransition);

            MessageBox.Show($"Static Tile {tileID} added successfully!", "Tile Added",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void transitionEditor_btn_removeStaticTile_Click(object sender, EventArgs e)
        {
            // Make sure a transition is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a transition first.", "No Transition Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Make sure a static tile is selected
            if (transitionEditor_lbox_staticTiles.SelectedItem == null)
            {
                MessageBox.Show("Please select a static tile to remove.", "No Tile Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected transition and tile
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;
            MapCreator.Engine.Compiler.StaticTile selectedTile = (MapCreator.Engine.Compiler.StaticTile)transitionEditor_lbox_staticTiles.SelectedItem;

            // Confirm removal
            DialogResult result = MessageBox.Show(
                $"Remove Static Tile {selectedTile}?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Remove the tile
                selectedTransition.RemoveStaticTile(selectedTile);

                // Refresh the display
                DisplayTransitionDetails(selectedTransition);

                MessageBox.Show("Static Tile removed successfully!", "Tile Removed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void transitionEditor_gbox_pluginControl_btn_save_Click(object sender, EventArgs e)
        {
            // Make sure a transition is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a transition to save.", "No Transition Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected transition
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;

            // Update the description from the TextBox
            selectedTransition.Description = transitionEditor_tbox_description.Text;

            // Update Random Statics File reference
            if (transitionEditor_chkbox_useRandomStatics.Checked)
            {
                // User wants to use Random Statics
                string fileName = transitionEditor_tbox_randomStaticsFile.Text.Trim();

                if (!string.IsNullOrEmpty(fileName))
                {
                    // Set the File attribute (just the reference, not editing the RS file itself)
                    selectedTransition.File = fileName;
                }
                else
                {
                    MessageBox.Show("Please enter a RandomStatics filename or uncheck the checkbox.",
                        "Filename Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                // User does NOT want Random Statics - clear the File attribute
                selectedTransition.File = null;
            }

            // The transition's tiles are already updated in real-time by Add/Remove buttons
            // So we just need to refresh the transition list display
            RefreshTransitionList();

            MessageBox.Show("Changes saved successfully!\n\nNote: To make changes permanent, use Export and embed in Custom folder.",
                "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshTransitionList()
        {
            // Remember which transition was selected
            Transition selectedTransition = transitionEditor_lbox_transitionList.SelectedItem as Transition;

            // Refresh the available transitions list
            UpdateAvailableTransitions();

            // Try to re-select the same transition
            if (selectedTransition != null)
            {
                // Find it in the refreshed list by HashKey
                for (int i = 0; i < transitionEditor_lbox_transitionList.Items.Count; i++)
                {
                    Transition trans = (Transition)transitionEditor_lbox_transitionList.Items[i];
                    if (trans.HashKey == selectedTransition.HashKey)
                    {
                        transitionEditor_lbox_transitionList.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void transitionEditor_gbox_pluginControl_btn_new_Click(object sender, EventArgs e)
        {
            // Make sure terrains are selected
            if (transitionEditor_cbox_terrainA.SelectedItem == null ||
                transitionEditor_cbox_terrainB.SelectedItem == null)
            {
                MessageBox.Show("Please select Terrain A and Terrain B first.", "Terrains Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // For 3-way mode, also check Terrain C
            if (transitionEditor_rbtn_3wayTransition.Checked &&
                transitionEditor_cbox_terrainC.SelectedItem == null)
            {
                MessageBox.Show("Please select Terrain C for 3-way transitions.", "Terrain C Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get terrain information
            ClsTerrain terrainA = (ClsTerrain)transitionEditor_cbox_terrainA.SelectedItem;
            ClsTerrain terrainB = (ClsTerrain)transitionEditor_cbox_terrainB.SelectedItem;
            ClsTerrain terrainC = transitionEditor_rbtn_3wayTransition.Checked ? (ClsTerrain)transitionEditor_cbox_terrainC.SelectedItem : null;

            // Prompt for description
            string terrainNames = terrainC != null
                ? $"{terrainA.Name} to {terrainB.Name} to {terrainC.Name}"
                : $"{terrainA.Name} to {terrainB.Name}";

            string description = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter a description for this transition:",
                "New Transition",
                terrainNames);

            // User cancelled
            if (string.IsNullOrEmpty(description))
                return;

            // Generate default HashKey pattern
            string hashKey = GenerateDefaultHashKey(terrainA.GroupID, terrainB.GroupID,
                terrainC?.GroupID ?? 0, transitionEditor_rbtn_3wayTransition.Checked);

            // Check if this HashKey already exists
            if (transitionTable.GetTransitionTable.ContainsKey(hashKey))
            {
                MessageBox.Show($"A transition with HashKey {hashKey} already exists.\n\nTry modifying the pattern after creation.",
                    "Duplicate HashKey", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create new transition using XML constructor
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlElement transInfo = doc.CreateElement("TransInfo");
                transInfo.SetAttribute("Description", description);
                transInfo.SetAttribute("HashKey", hashKey);

                // Add empty MapTiles node
                XmlElement mapTiles = doc.CreateElement("MapTiles");
                transInfo.AppendChild(mapTiles);

                // Add empty StaticTiles node
                XmlElement staticTiles = doc.CreateElement("StaticTiles");
                transInfo.AppendChild(staticTiles);

                // Create transition from XML
                Transition newTransition = new Transition(transInfo);

                // Add to transition table
                transitionTable.Add(newTransition);

                // Refresh the list
                RefreshTransitionList();

                // Select the new transition
                for (int i = 0; i < transitionEditor_lbox_transitionList.Items.Count; i++)
                {
                    Transition trans = (Transition)transitionEditor_lbox_transitionList.Items[i];
                    if (trans.HashKey == hashKey)
                    {
                        transitionEditor_lbox_transitionList.SelectedIndex = i;
                        break;
                    }
                }

                MessageBox.Show($"New transition created!\n\nHashKey: {hashKey}\n\nYou can now add Map Tiles and Static Tiles.",
                    "Transition Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating transition:\n{ex.Message}",
                    "Creation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateDefaultHashKey(int terrainA, int terrainB, int terrainC, bool is3Way)
        {
            // Generate a default diagonal transition pattern
            // Pattern: Top-left to bottom-right diagonal

            byte idA = (byte)terrainA;
            byte idB = (byte)terrainB;
            byte idC = is3Way ? (byte)terrainC : idB;

            // Create diagonal pattern (telephone keypad 0-8):
            // [0][1][2]     A  A  B
            // [3][4][5]  →  A  B  B
            // [6][7][8]     B  B  B

            string hashKey;

            if (is3Way)
            {
                // 3-way diagonal: A → B → C
                // [0][1][2]     A  A  B
                // [3][4][5]  →  A  B  C
                // [6][7][8]     B  C  C
                hashKey = string.Format("{0:X2}{0:X2}{1:X2}{0:X2}{1:X2}{2:X2}{1:X2}{2:X2}{2:X2}",
                    idA, idB, idC);
            }
            else
            {
                // 2-way diagonal: A → B
                hashKey = string.Format("{0:X2}{0:X2}{1:X2}{0:X2}{1:X2}{1:X2}{1:X2}{1:X2}{1:X2}",
                    idA, idB);
            }

            return hashKey;
        }

        private void transitionEditor_gbox_pluginControl_btn_delete_Click(object sender, EventArgs e)
        {
            // Make sure a transition is selected
            if (transitionEditor_lbox_transitionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a transition to delete.", "No Transition Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get the selected transition
            Transition selectedTransition = (Transition)transitionEditor_lbox_transitionList.SelectedItem;

            // Confirm deletion
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete this transition?\n\n" +
                $"HashKey: {selectedTransition.HashKey}\n" +
                $"Description: {selectedTransition.Description}\n\n" +
                $"This will remove it from the current session only.\n" +
                $"To permanently remove an embedded transition, delete it from the Custom folder and rebuild.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Remove from transition table
                    transitionTable.Remove(selectedTransition);

                    // Clear the detail display
                    ClearTransitionDisplay();

                    // Refresh the list
                    RefreshTransitionList();

                    MessageBox.Show("Transition deleted successfully!\n\nNote: This only affects the current session.",
                        "Deletion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting transition:\n{ex.Message}",
                        "Deletion Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearTransitionDisplay()
        {
            // Clear HashKey grid
            transitionEditor_lbl_hashKey0.Text = "--";
            transitionEditor_lbl_hashKey1.Text = "--";
            transitionEditor_lbl_hashKey2.Text = "--";
            transitionEditor_lbl_hashKey3.Text = "--";
            transitionEditor_lbl_hashKey4.Text = "--";
            transitionEditor_lbl_hashKey5.Text = "--";
            transitionEditor_lbl_hashKey6.Text = "--";
            transitionEditor_lbl_hashKey7.Text = "--";
            transitionEditor_lbl_hashKey8.Text = "--";

            // Clear Map Tiles
            transitionEditor_lbox_mapTiles.Items.Clear();

            // Clear Static Tiles
            transitionEditor_lbox_staticTiles.Items.Clear();

            // Clear Description
            transitionEditor_tbox_description.Text = string.Empty;
        }

        private void transitionEditor_gbox_pluginControl_btn_load_Click(object sender, EventArgs e)
        {
            // Create default path
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserXMLFiles");
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            // Open file dialog
            OpenFileDialog openDialog = new OpenFileDialog
            {
                InitialDirectory = defaultPath,
                Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                Title = "Load Transition XML",
                Multiselect = false
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadTransitionsFromFile(openDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading transitions:\n{ex.Message}",
                        "Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadTransitionsFromFile(string filePath)
        {
            // Load the XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Find all TransInfo nodes
            XmlNodeList transNodes = xmlDoc.SelectNodes("//Trans/TransInfo");

            if (transNodes == null || transNodes.Count == 0)
            {
                MessageBox.Show("No transitions found in this file.",
                    "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int loadedCount = 0;
            int skippedCount = 0;
            List<string> skippedHashKeys = new List<string>();

            // Process each transition
            foreach (XmlNode node in transNodes)
            {
                if (node is XmlElement element)
                {
                    try
                    {
                        // Create transition from XML
                        Transition newTransition = new Transition(element);

                        // Check if this HashKey already exists
                        if (transitionTable.GetTransitionTable.ContainsKey(newTransition.HashKey))
                        {
                            skippedCount++;
                            skippedHashKeys.Add(newTransition.HashKey);
                        }
                        else
                        {
                            // Add to transition table
                            transitionTable.Add(newTransition);
                            loadedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Skip invalid transitions
                        skippedCount++;
                    }
                }
            }

            // Refresh the list
            RefreshTransitionList();

            // Show summary
            string message = $"Loaded {loadedCount} transition(s) successfully!";

            if (skippedCount > 0)
            {
                message += $"\n\nSkipped {skippedCount} transition(s):";
                if (skippedHashKeys.Count > 0)
                {
                    message += "\n- Duplicate HashKeys (already exist in table)";
                }
                else
                {
                    message += "\n- Invalid or corrupted data";
                }
            }

            MessageBox.Show(message, "Load Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void transitionEditor_gbox_pluginControl_btn_exportAll_Click(object sender, EventArgs e)
        {
            // Make sure there are transitions to export
            if (transitionEditor_lbox_transitionList.Items.Count == 0)
            {
                MessageBox.Show("No transitions available to export.", "Nothing to Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Build the default filename based on selected terrains
            string defaultFileName = GetTransitionFileName();

            // Create UserXMLFiles folder if it doesn't exist
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserXMLFiles");
            Directory.CreateDirectory(defaultPath);

            // Open Save dialog
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                InitialDirectory = defaultPath,
                FileName = defaultFileName,
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Export All Transitions",
                DefaultExt = "xml"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Export all visible transitions
                    ExportAllTransitions(saveDialog.FileName);

                    MessageBox.Show(
                        $"Exported {transitionEditor_lbox_transitionList.Items.Count} transition(s) successfully to:\n{saveDialog.FileName}",
                        "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting transitions:\n{ex.Message}",
                        "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportAllTransitions(string filePath)
        {
            // Create XML document
            XmlTextWriter writer = new XmlTextWriter(filePath, Encoding.UTF8);
            writer.Indentation = 2;
            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();
            writer.WriteStartElement("Trans");

            // Save all transitions from the visible list
            foreach (var item in transitionEditor_lbox_transitionList.Items)
            {
                if (item is Transition transition)
                {
                    transition.Save(writer);
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        private void transitionEditor_chkbox_useRandomStatics_CheckedChanged(object sender, EventArgs e)
        {
            // Enable/disable Random Statics controls based on checkbox
            bool isEnabled = transitionEditor_chkbox_useRandomStatics.Checked;

            transitionEditor_lbl_randomStaticsFile.Enabled = isEnabled;
            transitionEditor_tbox_randomStaticsFile.Enabled = isEnabled;
            transitionEditor_lbl_globalChance.Enabled = isEnabled;
            transitionEditor_numupdwn_globalChance.Enabled = isEnabled;
            transitionEditor_btn_editRandomStatics.Enabled = isEnabled;
            transitionEditor_btn_clearRandomStatics.Enabled = isEnabled;
            transitionEditor_btn_newRandomStatics.Enabled = isEnabled;

            // If unchecking, clear the values
            if (!isEnabled)
            {
                transitionEditor_tbox_randomStaticsFile.Text = string.Empty;
                transitionEditor_numupdwn_globalChance.Value = 0;
            }
        }

        private void transitionEditor_btn_clearRandomStatics_Click(object sender, EventArgs e)
        {
            // Uncheck the checkbox (this will trigger the CheckedChanged event)
            // which will clear the textbox and NumericUpDown
            transitionEditor_chkbox_useRandomStatics.Checked = false;

            MessageBox.Show("Random Statics cleared.", "Cleared",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void transitionEditor_btn_editRandomStatics_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RandomStatics editor coming soon!\n\n" +
                            "This will open a full editor for creating/editing RandomStatics XML files.\n\n" +
                            "For now, use an external XML editor to modify files in:\n" +
                            "MapCreator.Engine\\Compiler\\XMLFacetData\\TerrainTypes\\",
                            "Feature Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void transitionEditor_btn_newRandomStatics_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RandomStatics creator coming soon!\n\n" +
                            "This will create a new RandomStatics XML file with collections, statics, hues, etc.\n\n" +
                            "For now, copy an existing file from:\n" +
                            "MapCreator.Engine\\Compiler\\XMLFacetData\\TerrainTypes\\",
                            "Feature Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
