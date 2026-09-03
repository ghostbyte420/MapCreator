using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using MapCreator.Engine.Compiler;

namespace MapCreator.Engine.Plugin.FacetDesigner
{
    public partial class facetDesigner : Form
    {
        private List<TerrainInfo> terrainList = new List<TerrainInfo>();
        private List<AltitudeInfo> altitudeList = new List<AltitudeInfo>();
        private Color[] currentPalette = null;
        private enum PaletteMode { None, Terrain, Altitude }
        private PaletteMode currentPaletteMode = PaletteMode.None;
        private Bitmap currentImage = null;
        private float currentZoom = 1.0f;
        private readonly float minZoom = 0.13f;
        private readonly float maxZoom = 200.0f;
        private Point? highlightedTile = null;
        private bool isPanning = false;
        private Point panStartPoint;
        private Point scrollStartPoint;
        private Point pictureBoxStartLocation;
        private List<Bitmap> undoStack = new List<Bitmap>();
        private List<Bitmap> redoStack = new List<Bitmap>();
        private int maxUndoSteps = 20;
        private Color selectedColor = Color.Black;
        private int selectedColorIndex = 0;
        private bool isPainting = false;
        private Point lastPaintPoint;
        private List<SwatchItem> swatchItems = new List<SwatchItem>();
        private string customNamesFilePath;

        public facetDesigner()
        {
            InitializeComponent();
            facetDesigner_panel_pictureBox_facetCanvas.BackColor = Color.Silver;
            SetDoubleBuffered(facetDesigner_panel_pictureBox_facetCanvas);
            customNamesFilePath = Path.Combine(Application.StartupPath, "MapCompiler", "Engine", "customSwatchNames.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(customNamesFilePath));

            // numeric zoom settings
            facetDesigner_toolStrip_numericUpDown_zoomLevel.Minimum = (decimal)minZoom;
            facetDesigner_toolStrip_numericUpDown_zoomLevel.Maximum = (decimal)maxZoom;
            facetDesigner_toolStrip_numericUpDown_zoomLevel.DecimalPlaces = 2;
            facetDesigner_toolStrip_numericUpDown_zoomLevel.Increment = 0.1M;
            facetDesigner_toolStrip_numericUpDown_zoomLevel.Value = (decimal)currentZoom;
            facetDesigner_toolStrip_numericUpDown_zoomLevel.ValueChanged += facetDesigner_toolStrip_numericUpDown_zoomLevel_ValueChanged;

            // menu / toolbar wiring
            facetDesigner_menuStrip_menuStripButton_selectColorPalette_loadTerrain.Click += facetDesigner_menuStrip_menuStripButton_selectColorPalette_loadTerrain_Click;
            facetDesigner_menuStrip_menuStripButton_selectColorPalette_loadAltitude.Click += facetDesigner_menuStrip_menuStripButton_selectColorPalette_loadAltitude_Click;
            facetDesigner_menuStrip_menuStripButton_selectFacetBitmap_importTerrainBitmap.Click += facetDesigner_menuStrip_menuStripButton_selectFacetBitmap_importTerrainBitmap_Click;
            facetDesigner_menuStrip_menuStripButton_selectFacetBitmap_importAltitudeBitmap.Click += facetDesigner_menuStrip_menuStripButton_selectFacetBitmap_importAltitudeBitmap_Click;
            facetDesigner_toolStrip_toolStripButton_undoChanges.Click += facetDesigner_toolStrip_toolStripButton_undoChanges_Click;
            facetDesigner_menuStrip_menuStripButton_saveFacetBitmap.Click += facetDesigner_menuStrip_menuStripButton_saveFacetBitmap_Click;

            // canvas event hookups
            facetDesigner_panel_pictureBox_facetCanvas.Paint += facetDesigner_panel_pictureBox_facetCanvas_Paint;
            facetDesigner_panel_pictureBox_facetCanvas.MouseWheel += FacetCanvas_MouseWheel;
            facetDesigner_panel_pictureBox_facetCanvas.MouseEnter += (s, e) => facetDesigner_panel_pictureBox_facetCanvas.Focus();
            facetDesigner_panel_pictureBox_facetCanvas.MouseMove += FacetCanvas_MouseMove;
            facetDesigner_panel_pictureBox_facetCanvas.MouseLeave += (s, e) =>
            {
                highlightedTile = null;
                facetDesigner_panel_pictureBox_facetCanvas.Invalidate();
            };
            facetDesigner_panel_pictureBox_facetCanvas.MouseDown += FacetCanvas_MouseDown;
            facetDesigner_panel_pictureBox_facetCanvas.MouseUp += FacetCanvas_MouseUp;
            facetDesigner_panel_pictureBox_facetCanvas.Dock = DockStyle.None;
            facetDesigner_panel_pictureBox_facetCanvas.SizeMode = PictureBoxSizeMode.Normal;

            // Swatch panel setup
            facetDesigner_panel_swatchColors.AutoScroll = true;
            facetDesigner_panel_swatchColors.Width = 220;
        }

        public static void SetDoubleBuffered(Control c)
        {
            if (SystemInformation.TerminalServerSession) return;
            var aProp = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

        // ---- Zoom Synchronization ----
        private bool _suppressZoomEvent = false;
        private void SetZoomLevelWithoutEvent(float zoom)
        {
            zoom = Math.Max(minZoom, Math.Min(maxZoom, zoom));
            try
            {
                _suppressZoomEvent = true;
                facetDesigner_toolStrip_numericUpDown_zoomLevel.Value = (decimal)zoom;
                currentZoom = zoom;
                UpdateCanvasSizeAndInvalidate();
            }
            finally
            {
                _suppressZoomEvent = false;
            }
        }

        private void facetDesigner_toolStrip_numericUpDown_zoomLevel_ValueChanged(object sender, EventArgs e)
        {
            if (_suppressZoomEvent) return;
            var v = (float)facetDesigner_toolStrip_numericUpDown_zoomLevel.Value;
            currentZoom = Math.Max(minZoom, Math.Min(maxZoom, v));
            UpdateCanvasSizeAndInvalidate();
        }

        // ---- Palette Loading ----
        private void facetDesigner_menuStrip_menuStripButton_selectColorPalette_loadTerrain_Click(object sender, EventArgs e)
        {
            terrainList = LoadTerrainFromXML("Terrain.xml");

            LoadSwatches(terrainList);
            currentPalette = GetTerrainPalette();
            currentPaletteMode = PaletteMode.Terrain;
        }

        private void facetDesigner_menuStrip_menuStripButton_selectColorPalette_loadAltitude_Click(object sender, EventArgs e)
        {
            altitudeList = LoadAltitudeFromXML("Altitude.xml");

            LoadSwatches(altitudeList);
            currentPalette = GetAltitudePalette();
            currentPaletteMode = PaletteMode.Altitude;
        }

        private Color[] GetTerrainPalette()
        {
            Color[] palette = new Color[256];
            foreach (var item in terrainList)
            {
                palette[item.ID] = item.Color;
            }
            return palette;
        }

        private Color[] GetAltitudePalette()
        {
            Color[] palette = new Color[256];
            foreach (var item in altitudeList)
            {
                palette[item.Key] = item.Color;
            }
            return palette;
        }

        // ---- Swatch Loading ----
        private void LoadSwatches(List<TerrainInfo> terrainList)
        {
            swatchItems.Clear();
            facetDesigner_panel_swatchColors.Controls.Clear();
            swatchItems = terrainList.Select((_, i) => new SwatchItem { Index = i, CustomName = "" }).ToList();
            LoadCustomSwatchNames();
            int yPos = 10;
            foreach (var item in terrainList)
            {
                int index = terrainList.IndexOf(item);
                var swatchItem = swatchItems[index];
                var colorButton = new Button
                {
                    BackColor = item.Color,
                    Width = 32,
                    Height = 32,
                    Left = 10,
                    Top = yPos,
                    Tag = index
                };
                colorButton.Click += TerrainSwatch_Click;
                string displayName = string.IsNullOrEmpty(swatchItem.CustomName) ? item.Name : swatchItem.CustomName;
                var nameLabel = new TextBox
                {
                    Text = displayName,
                    Width = 150,
                    Left = 50,
                    Top = yPos + 4,
                    Tag = index,
                    BorderStyle = BorderStyle.FixedSingle
                };
                nameLabel.TextChanged += (s, e) =>
                {
                    swatchItem.CustomName = nameLabel.Text;
                    SaveCustomSwatchNames();
                };
                facetDesigner_panel_swatchColors.Controls.Add(colorButton);
                facetDesigner_panel_swatchColors.Controls.Add(nameLabel);
                yPos += 40;
            }
        }

        private void LoadSwatches(List<AltitudeInfo> altitudeList)
        {
            swatchItems.Clear();
            facetDesigner_panel_swatchColors.Controls.Clear();
            swatchItems = altitudeList.Select((_, i) => new SwatchItem { Index = i, CustomName = "" }).ToList();
            LoadCustomSwatchNames();
            int yPos = 10;
            foreach (var item in altitudeList)
            {
                int index = altitudeList.IndexOf(item);
                var swatchItem = swatchItems[index];
                var colorButton = new Button
                {
                    BackColor = item.Color,
                    Width = 32,
                    Height = 32,
                    Left = 10,
                    Top = yPos,
                    Tag = index
                };
                colorButton.Click += AltitudeSwatch_Click;
                string displayName = string.IsNullOrEmpty(swatchItem.CustomName)
                    ? $"Key: {item.Key}, Type: {item.Type}, Altitude: {item.Altitude}"
                    : swatchItem.CustomName;
                var nameLabel = new TextBox
                {
                    Text = displayName,
                    Width = 150,
                    Left = 50,
                    Top = yPos + 4,
                    Tag = index,
                    BorderStyle = BorderStyle.FixedSingle
                };
                nameLabel.TextChanged += (s, e) =>
                {
                    swatchItem.CustomName = nameLabel.Text;
                    SaveCustomSwatchNames();
                };
                facetDesigner_panel_swatchColors.Controls.Add(colorButton);
                facetDesigner_panel_swatchColors.Controls.Add(nameLabel);
                yPos += 40;
            }
        }

        // ---- Custom Names Persistence ----
        private void SaveCustomSwatchNames()
        {
            var doc = new XDocument(
                new XElement("CustomNames",
                    swatchItems
                        .Where(item => !string.IsNullOrEmpty(item.CustomName))
                        .Select(item => new XElement("Swatch",
                            new XAttribute("Index", item.Index),
                            new XAttribute("CustomName", item.CustomName)
                        ))
                )
            );
            doc.Save(customNamesFilePath);
        }

        private void LoadCustomSwatchNames()
        {
            if (File.Exists(customNamesFilePath))
            {
                var doc = XDocument.Load(customNamesFilePath);
                foreach (var elem in doc.Descendants("Swatch"))
                {
                    int index = int.Parse(elem.Attribute("Index")?.Value ?? "-1");
                    string customName = elem.Attribute("CustomName")?.Value;
                    if (index >= 0 && index < swatchItems.Count)
                    {
                        swatchItems[index].CustomName = customName;
                    }
                }
            }
        }

        // ---- Bitmap Import ----
        private void facetDesigner_menuStrip_menuStripButton_selectFacetBitmap_importTerrainBitmap_Click(object sender, EventArgs e)
        {
            if (currentPaletteMode != PaletteMode.Terrain || currentPalette == null)
            {
                MessageBox.Show("Load the Terrain swatch first!", "Palette Needed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ImportIndexedBitmap("Terrain.bmp", currentPalette);
        }

        private void facetDesigner_menuStrip_menuStripButton_selectFacetBitmap_importAltitudeBitmap_Click(object sender, EventArgs e)
        {
            if (currentPaletteMode != PaletteMode.Altitude || currentPalette == null)
            {
                MessageBox.Show("Load the Altitude swatch first!", "Palette Needed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ImportIndexedBitmap("Altitude.bmp", currentPalette);
        }

        private unsafe void ImportIndexedBitmap(string defaultFilename, Color[] palette)
        {
            using OpenFileDialog dlg = new OpenFileDialog
            {
                Title = $"Select {defaultFilename}",
                Filter = "Bitmap files (*.bmp)|*.bmp",
                FileName = defaultFilename
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using var bmp = new Bitmap(dlg.FileName);
                if (bmp.PixelFormat != PixelFormat.Format8bppIndexed)
                {
                    MessageBox.Show("Only 8-bit indexed BMP files are supported.", "Wrong Format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Bitmap displayBmp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format8bppIndexed);
                // ColorPalette.Entries is read-only in modern .NET; build a new palette via ColorPalette(Color[]).
                ColorPalette bmpPalette = displayBmp.Palette;
                Color[] bmpEntries = bmpPalette.Entries;
                for (int i = 0; i < palette.Length && i < bmpEntries.Length; i++)
                {
                    bmpEntries[i] = palette[i];
                }
                displayBmp.Palette = new ColorPalette(bmpEntries);
                var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var srcData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);
                var dstData = displayBmp.LockBits(rect, ImageLockMode.WriteOnly, displayBmp.PixelFormat);
                try
                {
                    int srcStride = srcData.Stride;
                    int dstStride = dstData.Stride;
                    byte* srcPtr = (byte*)srcData.Scan0;
                    byte* dstPtr = (byte*)dstData.Scan0;
                    int width = bmp.Width;
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            dstPtr[y * dstStride + x] = srcPtr[y * srcStride + x];
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(srcData);
                    displayBmp.UnlockBits(dstData);
                }
                SetImageAndZoom(displayBmp);
            }
        }

        private void SetImageAndZoom(Bitmap bmp)
        {
            currentImage?.Dispose();
            currentImage = bmp;
            if (currentPalette != null && currentImage.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                ColorPalette palette = currentImage.Palette;
                Color[] entries = palette.Entries;
                for (int i = 0; i < currentPalette.Length && i < entries.Length; i++)
                {
                    entries[i] = currentPalette[i];
                }
                currentImage.Palette = new ColorPalette(entries);
            }
            ZoomToFit();
            facetDesigner_panel_facetCanvas.AutoScrollPosition = new Point(0, 0);
        }

        // ---- Zoom & Canvas Updates ----
        private void ZoomToFit()
        {
            if (currentImage == null) return;
            var panelSize = facetDesigner_panel_facetCanvas.ClientSize;
            float zoomX = (float)panelSize.Width / currentImage.Width;
            float zoomY = (float)panelSize.Height / currentImage.Height;
            float fitZoom = Math.Min(zoomX, zoomY);
            SetZoomLevelWithoutEvent(fitZoom);
        }

        private void ZoomTo100() { SetZoomLevelWithoutEvent(1.0f); }
        private void ZoomTo50() { SetZoomLevelWithoutEvent(0.5f); }

        private void UpdateCanvasSizeAndInvalidate()
        {
            if (currentImage != null)
            {
                int newWidth = (int)(currentImage.Width * currentZoom);
                int newHeight = (int)(currentImage.Height * currentZoom);
                facetDesigner_panel_pictureBox_facetCanvas.Size = new Size(newWidth, newHeight);
                facetDesigner_panel_pictureBox_facetCanvas.Location = new Point(
                    Math.Max(0, (facetDesigner_panel_facetCanvas.ClientSize.Width - newWidth) / 2),
                    Math.Max(0, (facetDesigner_panel_facetCanvas.ClientSize.Height - newHeight) / 2)
                );
            }
            facetDesigner_panel_pictureBox_facetCanvas.Invalidate();
        }

        private void FacetCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            if (currentImage == null || ModifierKeys != Keys.Control) return;
            const float zoomStep = 1.10f;
            float newZoom = e.Delta > 0 ? Math.Min(maxZoom, currentZoom * zoomStep) : Math.Max(minZoom, currentZoom / zoomStep);
            SetZoomLevelWithoutEvent(newZoom);
        }

        // ---- Mouse Panning & Highlight ----
        private Point ClampScroll(Point scroll, Size imageSize)
        {
            int panelW = facetDesigner_panel_facetCanvas.ClientSize.Width;
            int panelH = facetDesigner_panel_facetCanvas.ClientSize.Height;
            int imgW = imageSize.Width;
            int imgH = imageSize.Height;
            int maxX = Math.Max(0, imgW - panelW);
            int maxY = Math.Max(0, imgH - panelH);
            return new Point(
                Math.Max(0, Math.Min(scroll.X, maxX)),
                Math.Max(0, Math.Min(scroll.Y, maxY))
            );
        }

        private void FacetCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && currentImage != null)
            {
                isPainting = true;
                lastPaintPoint = e.Location;
                PaintPixel(e.Location);
            }
            else if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right)
            {
                isPanning = true;
                panStartPoint = e.Location;
                pictureBoxStartLocation = facetDesigner_panel_pictureBox_facetCanvas.Location;
                facetDesigner_panel_pictureBox_facetCanvas.Cursor = Cursors.SizeAll;
            }
        }

        private void FacetCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPainting = false;
                SaveStateForUndo();
            }
            else if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Right)
            {
                isPanning = false;
                facetDesigner_panel_pictureBox_facetCanvas.Cursor = Cursors.Default;
            }
        }

        private void FacetCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPainting && currentImage != null)
            {
                PaintLine(lastPaintPoint, e.Location);
                lastPaintPoint = e.Location;
            }
            else if (isPanning)
            {
                FacetCanvas_MouseDrag(sender, e);
            }
            else
            {
                facetDesigner_panel_pictureBox_facetCanvas_MouseMove(sender, e);
            }
        }

        private void FacetCanvas_MouseDrag(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                int dx = e.X - panStartPoint.X;
                int dy = e.Y - panStartPoint.Y;
                int newX = pictureBoxStartLocation.X + dx;
                int newY = pictureBoxStartLocation.Y + dy;
                if (currentImage != null)
                {
                    int scaledWidth = (int)(currentImage.Width * currentZoom);
                    int scaledHeight = (int)(currentImage.Height * currentZoom);
                    int panelWidth = facetDesigner_panel_facetCanvas.ClientSize.Width;
                    int panelHeight = facetDesigner_panel_facetCanvas.ClientSize.Height;
                    int minX = panelWidth - scaledWidth;
                    int maxX = 0;
                    int minY = panelHeight - scaledHeight;
                    int maxY = 0;
                    if (scaledWidth < panelWidth)
                    {
                        minX = 0;
                        maxX = panelWidth - scaledWidth;
                    }
                    if (scaledHeight < panelHeight)
                    {
                        minY = 0;
                        maxY = panelHeight - scaledHeight;
                    }
                    newX = Math.Max(minX, Math.Min(maxX, newX));
                    newY = Math.Max(minY, Math.Min(maxY, newY));
                }
                facetDesigner_panel_pictureBox_facetCanvas.Location = new Point(newX, newY);
            }
            else
            {
                facetDesigner_panel_pictureBox_facetCanvas_MouseMove(sender, e);
            }
        }

        private void facetDesigner_panel_pictureBox_facetCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentImage == null) { highlightedTile = null; return; }
            int px = (int)Math.Floor((e.X - facetDesigner_panel_pictureBox_facetCanvas.Left) / currentZoom);
            int py = (int)Math.Floor((e.Y - facetDesigner_panel_pictureBox_facetCanvas.Top) / currentZoom);
            this.Text = $"FacetDesigner - ({px}, {py})";
            if (px >= 0 && px < currentImage.Width && py >= 0 && py < currentImage.Height)
            {
                if (highlightedTile == null || highlightedTile.Value.X != px || highlightedTile.Value.Y != py)
                {
                    highlightedTile = new Point(px, py);
                    facetDesigner_panel_pictureBox_facetCanvas.Invalidate();
                }
            }
            else if (highlightedTile != null)
            {
                highlightedTile = null;
                facetDesigner_panel_pictureBox_facetCanvas.Invalidate();
            }
        }

        // ---- OPTIMIZED PAINT HANDLER ----
        private void facetDesigner_panel_pictureBox_facetCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (currentImage == null) return;
            float scale = currentZoom;
            int imgW = currentImage.Width;
            int imgH = currentImage.Height;
            Rectangle destRect = e.ClipRectangle;
            int srcX = (int)(destRect.X / scale);
            int srcY = (int)(destRect.Y / scale);
            int srcW = (int)Math.Ceiling(destRect.Width / scale);
            int srcH = (int)Math.Ceiling(destRect.Height / scale);
            if (srcX < 0) srcX = 0;
            if (srcY < 0) srcY = 0;
            if (srcX + srcW > imgW) srcW = imgW - srcX;
            if (srcY + srcH > imgH) srcH = imgH - srcY;
            if (srcW <= 0 || srcH <= 0) return;
            e.Graphics.Clear(facetDesigner_panel_pictureBox_facetCanvas.BackColor);
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.DrawImage(
                currentImage,
                destRect,
                new Rectangle(srcX, srcY, srcW, srcH),
                GraphicsUnit.Pixel
            );
            if (highlightedTile != null)
            {
                var (tx, ty) = (highlightedTile.Value.X, highlightedTile.Value.Y);
                var rect = new RectangleF(tx * scale, ty * scale, scale, scale);
                using var brush = new SolidBrush(Color.FromArgb(120, Color.Yellow));
                using var pen = new Pen(Color.Red, Math.Max(1f, scale / 8f));
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        // ---- Toolstrip Zoom Buttons ---
        private void facetDesigner_toolStrip_toolStripButton_zoom100_Click(object sender, EventArgs e) => ZoomTo100();
        private void facetDesigner_toolStrip_toolStripButton_zoomFit_Click(object sender, EventArgs e) => ZoomToFit();
        private void facetDesigner_toolStrip_toolStripButton_zoom50_Click(object sender, EventArgs e) => ZoomTo50();

        // ---- XML Loaders ----
        public List<TerrainInfo> LoadTerrainFromXML(string xmlPath)
        {
            var terrains = new List<TerrainInfo>();
            var doc = XDocument.Load(FacetData.OpenRead(xmlPath));

            foreach (var elem in doc.Descendants("Terrain"))
            {
                terrains.Add(new TerrainInfo
                {
                    Name = elem.Attribute("Name")?.Value,
                    ID = int.Parse(elem.Attribute("ID")?.Value ?? "0"),
                    TileID = int.Parse(elem.Attribute("TileID")?.Value ?? "0"),
                    Color = Color.FromArgb(
                        int.Parse(elem.Attribute("R")?.Value ?? "0"),
                        int.Parse(elem.Attribute("G")?.Value ?? "0"),
                        int.Parse(elem.Attribute("B")?.Value ?? "0")),
                    Base = int.Parse(elem.Attribute("Base")?.Value ?? "0"),
                    Random = bool.Parse(elem.Attribute("Random")?.Value ?? "False")
                });
            }
            return terrains;
        }

        public List<AltitudeInfo> LoadAltitudeFromXML(string xmlPath)
        {
            var altitudes = new List<AltitudeInfo>();
            var doc = XDocument.Load(FacetData.OpenRead(xmlPath));

            foreach (var elem in doc.Descendants("Altitude"))
            {
                altitudes.Add(new AltitudeInfo
                {
                    Key = int.Parse(elem.Attribute("Key")?.Value ?? "0"),
                    Type = elem.Attribute("Type")?.Value,
                    Altitude = int.Parse(elem.Attribute("Altitude")?.Value ?? "0"),
                    Color = Color.FromArgb(
                        int.Parse(elem.Attribute("R")?.Value ?? "0"),
                        int.Parse(elem.Attribute("G")?.Value ?? "0"),
                        int.Parse(elem.Attribute("B")?.Value ?? "0"))
                });
            }
            return altitudes;
        }

        private void TerrainSwatch_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                if (index >= 0 && index < terrainList.Count)
                {
                    selectedColor = terrainList[index].Color;
                    selectedColorIndex = terrainList[index].ID;
                    MessageBox.Show($"Selected terrain color index: {selectedColorIndex}");
                }
                else
                {
                    MessageBox.Show($"Invalid index: {index}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AltitudeSwatch_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                if (index >= 0 && index < altitudeList.Count)
                {
                    var altitude = altitudeList[index];
                    selectedColor = altitude.Color;
                    selectedColorIndex = altitude.Key;
                    MessageBox.Show($"Selected altitude key: {selectedColorIndex}");
                }
                else
                {
                    MessageBox.Show($"Invalid index: {index}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void facetDesigner_menuStrip_menuStripButton_saveFacetBitmap_Click(object sender, EventArgs e)
        {
            if (currentImage == null)
            {
                MessageBox.Show("No image to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using var dlg = new SaveFileDialog
            {
                Title = "Save Facet Bitmap",
                Filter = "Bitmap files (*.bmp)|*.bmp",
                FileName = "FacetOutput.bmp"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using Bitmap saveBmp = new Bitmap(currentImage.Width, currentImage.Height, PixelFormat.Format8bppIndexed);
                    ColorPalette palette = saveBmp.Palette;
                    Color[] entries = palette.Entries;
                    for (int i = 0; i < currentPalette.Length && i < entries.Length; i++)
                    {
                        entries[i] = currentPalette[i];
                    }
                    saveBmp.Palette = new ColorPalette(entries);
                    var rect = new Rectangle(0, 0, currentImage.Width, currentImage.Height);
                    var srcData = currentImage.LockBits(rect, ImageLockMode.ReadOnly, currentImage.PixelFormat);
                    var dstData = saveBmp.LockBits(rect, ImageLockMode.WriteOnly, saveBmp.PixelFormat);
                    unsafe
                    {
                        try
                        {
                            int srcStride = srcData.Stride;
                            int dstStride = dstData.Stride;
                            byte* srcPtr = (byte*)srcData.Scan0;
                            byte* dstPtr = (byte*)dstData.Scan0;
                            int width = currentImage.Width;
                            for (int y = 0; y < currentImage.Height; y++)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    dstPtr[y * dstStride + x] = srcPtr[y * srcStride + x];
                                }
                            }
                        }
                        finally
                        {
                            currentImage.UnlockBits(srcData);
                            saveBmp.UnlockBits(dstData);
                        }
                    }
                    saveBmp.Save(dlg.FileName, ImageFormat.Bmp);
                    MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ---- Painting Logic ----
        private unsafe void PaintPixel(Point point)
        {
            if (currentImage == null || currentPalette == null)
                return;
            int px = (int)Math.Floor((point.X - facetDesigner_panel_pictureBox_facetCanvas.Left) / currentZoom);
            int py = (int)Math.Floor((point.Y - facetDesigner_panel_pictureBox_facetCanvas.Top) / currentZoom);
            if (px >= 0 && px < currentImage.Width && py >= 0 && py < currentImage.Height)
            {
                var rect = new Rectangle(0, 0, currentImage.Width, currentImage.Height);
                var bmpData = currentImage.LockBits(rect, ImageLockMode.ReadWrite, currentImage.PixelFormat);
                try
                {
                    byte* ptr = (byte*)bmpData.Scan0;
                    int stride = bmpData.Stride;
                    ptr += py * stride + px;
                    *ptr = (byte)selectedColorIndex;
                }
                finally
                {
                    currentImage.UnlockBits(bmpData);
                }
                facetDesigner_panel_pictureBox_facetCanvas.Invalidate(
                    new Rectangle(
                        (int)(px * currentZoom),
                        (int)(py * currentZoom),
                        (int)Math.Ceiling(currentZoom),
                        (int)Math.Ceiling(currentZoom)
                    )
                );
            }
        }

        private void PaintLine(Point start, Point end)
        {
            int dx = Math.Abs(end.X - start.X);
            int dy = Math.Abs(end.Y - start.Y);
            int sx = start.X < end.X ? 1 : -1;
            int sy = start.Y < end.Y ? 1 : -1;
            int err = dx - dy;
            Point current = start;
            while (true)
            {
                PaintPixel(current);
                if (current == end) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; current.X += sx; }
                if (e2 < dx) { err += dx; current.Y += sy; }
            }
        }

        // ---- Undo/Redo ----
        private void SaveStateForUndo()
        {
            if (currentImage == null)
                return;
            var clone = (Bitmap)currentImage.Clone();
            undoStack.Add(clone);
            if (undoStack.Count > maxUndoSteps)
            {
                var oldest = undoStack[0];
                oldest.Dispose();
                undoStack.RemoveAt(0);
            }
            foreach (var redoImage in redoStack)
            {
                redoImage.Dispose();
            }
            redoStack.Clear();
        }

        private void Undo()
        {
            if (undoStack.Count == 0)
                return;
            if (currentImage != null)
            {
                var clone = (Bitmap)currentImage.Clone();
                redoStack.Add(clone);
            }
            var previous = undoStack[undoStack.Count - 1];
            undoStack.RemoveAt(undoStack.Count - 1);
            currentImage?.Dispose();
            currentImage = previous;
            facetDesigner_panel_pictureBox_facetCanvas.Invalidate();
        }

        private void Redo()
        {
            if (redoStack.Count == 0)
                return;
            if (currentImage != null)
            {
                var clone = (Bitmap)currentImage.Clone();
                undoStack.Add(clone);
            }
            var next = redoStack[redoStack.Count - 1];
            redoStack.RemoveAt(redoStack.Count - 1);
            currentImage?.Dispose();
            currentImage = next;
            facetDesigner_panel_pictureBox_facetCanvas.Invalidate();
        }

        private void facetDesigner_toolStrip_toolStripButton_undoChanges_Click(object sender, EventArgs e)
        {
            Undo();
        }

        // ---- Data Classes ----
        public class TerrainInfo
        {
            public string Name;
            public int ID;
            public int TileID;
            public Color Color;
            public int Base;
            public bool Random;
        }

        public class AltitudeInfo
        {
            public int Key;
            public string Type;
            public int Altitude;
            public Color Color;
        }

        public class SwatchItem
        {
            public int Index { get; set; }
            public string CustomName { get; set; }
        }
    }
}
