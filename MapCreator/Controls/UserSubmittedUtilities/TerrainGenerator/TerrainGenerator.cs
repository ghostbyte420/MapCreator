using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;

namespace MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator
{
    public partial class terrainGenerator : UserControl
    {
        private Dictionary<string, Color> _terrainColors = new Dictionary<string, Color>
        {
            {"Ocean", Color.FromArgb(0, 0, 130)},
            {"BaseLand", Color.FromArgb(0, 100, 0)},
            {"Lake", Color.FromArgb(0, 0, 170)},
            {"Beach", Color.FromArgb(255, 255, 192)},
            {"Desert", Color.FromArgb(227, 191, 51)},
            {"Dirt", Color.FromArgb(135, 125, 90)},
            {"Forest", Color.FromArgb(0, 110, 90)},
            {"Jungle", Color.FromArgb(0, 75, 75)},
            {"Mountain", Color.FromArgb(75, 75, 75)},
            {"Snow", Color.FromArgb(255, 255, 255)},
            {"Lava", Color.FromArgb(255, 0, 0)}
        };
        private const int EdgeBuffer = 20;
        private const int BiomeBuffer = 4;
        private Label thresholdValueLabel;
        private Label roughnessValueLabel;
        private System.Windows.Forms.Timer debounceTimer;
        private float lastThreshold = 0.5f;
        private int lastRoughness = 3;
        private bool isGenerating = false;
        private bool isSaving = false;

        public terrainGenerator()
        {
            // Enable double buffering to reduce flicker
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeComponent();

            // Initialize trackbars with adjusted height
            terrainGenerator_canvasDisplay_trackBar_threshold.Minimum = 0;
            terrainGenerator_canvasDisplay_trackBar_threshold.Maximum = 100;
            terrainGenerator_canvasDisplay_trackBar_threshold.Value = 50;
            terrainGenerator_canvasDisplay_trackBar_threshold.Size = new Size(
                terrainGenerator_canvasDisplay_trackBar_threshold.Width,
                22  // Half of original height
            );

            terrainGenerator_canvasDisplay_trackBar_roughness.Minimum = 1;
            terrainGenerator_canvasDisplay_trackBar_roughness.Maximum = 100;
            terrainGenerator_canvasDisplay_trackBar_roughness.Value = 3;
            terrainGenerator_canvasDisplay_trackBar_roughness.Size = new Size(
                terrainGenerator_canvasDisplay_trackBar_roughness.Width,
                22  // Half of original height
            );

            // Initialize zoom control
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Minimum = 10;
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Maximum = 1000;
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Value = 100;
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.ValueChanged += terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_ValueChanged;

            // Initialize labels AFTER controls are created
            thresholdValueLabel = new Label
            {
                Text = terrainGenerator_canvasDisplay_trackBar_threshold.Value.ToString(),
                ForeColor = Color.White,
                AutoSize = true
            };

            roughnessValueLabel = new Label
            {
                Text = terrainGenerator_canvasDisplay_trackBar_roughness.Value.ToString(),
                ForeColor = Color.White,
                AutoSize = true
            };

            // Position labels
            thresholdValueLabel.Location = new Point(
                terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.Right + 10,
                terrainGenerator_canvasDisplay_trackBar_threshold_label_threshold.Top
            );

            roughnessValueLabel.Location = new Point(
                terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.Right + 10,
                terrainGenerator_canvasDisplay_trackBar_threshold_label_roughness.Top
            );

            // Add labels to form
            Controls.Add(thresholdValueLabel);
            Controls.Add(roughnessValueLabel);

            // Assign event handlers
            terrainGenerator_menuStrip_menuStripButton_generateLand.Click += terrainGenerator_menuStrip_menuStripButton_generateLand_Click;
            terrainGenerator_canvasDisplay_trackBar_threshold.ValueChanged += terrainGenerator_canvasDisplay_trackBar_threshold_ValueChanged;
            terrainGenerator_canvasDisplay_trackBar_roughness.ValueChanged += terrainGenerator_canvasDisplay_trackBar_roughness_ValueChanged;

            // Setup debounce timer
            debounceTimer = new System.Windows.Forms.Timer();
            debounceTimer.Interval = 500;
            debounceTimer.Tick += DebounceTimer_Tick;
        }

        private void terrainGenerator_canvasDisplay_trackBar_threshold_ValueChanged(object sender, EventArgs e)
        {
            if (thresholdValueLabel != null)
            {
                thresholdValueLabel.Text = terrainGenerator_canvasDisplay_trackBar_threshold.Value.ToString();
                debounceTimer.Stop();
                debounceTimer.Start();
            }
        }

        private void terrainGenerator_canvasDisplay_trackBar_roughness_ValueChanged(object sender, EventArgs e)
        {
            if (roughnessValueLabel != null)
            {
                roughnessValueLabel.Text = terrainGenerator_canvasDisplay_trackBar_roughness.Value.ToString();
                debounceTimer.Stop();
                debounceTimer.Start();
            }
        }

        private void terrainGenerator_canvasDisplay_numericUpDown_canvasZoom_ValueChanged(object sender, EventArgs e)
        {
            float zoomFactor = (float)terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Value / 100f;
            terrainGenerator_canvasDisplay.Zoom = zoomFactor;
        }

        public void UpdateCanvasZoom()
        {
            int zoomPercentage = (int)(terrainGenerator_canvasDisplay.Zoom * 100);
            terrainGenerator_canvasDisplay_numericUpDown_canvasZoom.Value = zoomPercentage;
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();
            if (!isGenerating)
            {
                float threshold = terrainGenerator_canvasDisplay_trackBar_threshold.Value / 100f;
                int roughness = terrainGenerator_canvasDisplay_trackBar_roughness.Value;
                if (threshold != lastThreshold || roughness != lastRoughness)
                {
                    lastThreshold = threshold;
                    lastRoughness = roughness;
                    GeneratePreview(threshold, roughness);
                }
            }
        }

        private void terrainGenerator_menuStrip_menuStripButton_generateLand_Click(object sender, EventArgs e)
        {
            float threshold = terrainGenerator_canvasDisplay_trackBar_threshold.Value / 100f;
            int roughness = terrainGenerator_canvasDisplay_trackBar_roughness.Value;
            GenerateAndDisplayMap(threshold, roughness, 5120, 4096);
        }

        private void GeneratePreview(float threshold, int roughness)
        {
            GenerateAndDisplayMap(threshold, roughness, 1024, 768);
        }

        private void GenerateAndDisplayMap(float threshold, int roughness, int width, int height)
        {
            if (isGenerating) return;
            isGenerating = true;
            terrainGenerator_canvasDisplay.Visible = false;
            Cursor = Cursors.WaitCursor;

            Task.Run(() =>
            {
                Bitmap map = GenerateMap(width, height, threshold, roughness, 0.01f);
                map = ApplyBiomes(map);

                this.Invoke((MethodInvoker)delegate
                {
                    terrainGenerator_canvasDisplay.MapImage = map;
                    terrainGenerator_canvasDisplay.Zoom = 1.0f;
                    terrainGenerator_canvasDisplay._panOffset = Point.Empty;
                    terrainGenerator_canvasDisplay.Invalidate();
                    terrainGenerator_canvasDisplay.Visible = true;
                    Cursor = Cursors.Default;
                    isGenerating = false;
                });
            });
        }

        private Bitmap GenerateMap(int width, int height, float threshold, int roughness, float scale)
        {
            Bitmap bitmap = new Bitmap(width, height);
            var noise = new PerlinNoise();
            float[,] landNoise = new float[width, height];

            // Generate noise map with proper scaling
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    float value = float.MinValue;
                    for (int i = 0; i < 3; i++)
                    {
                        // Use different scales for each octave
                        float freq = (float)Math.Pow(2, i);
                        value = Math.Max(value, noise.FBM(x * scale * freq, y * scale * freq, roughness));
                    }
                    landNoise[x, y] = value;
                }
            });

            // Create bitmap from noise
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            unsafe
            {
                byte* ptr = (byte*)bitmapData.Scan0;
                int stride = bitmapData.Stride;

                Parallel.For(0, height, y =>
                {
                    byte* row = ptr + y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        bool isEdge = x < EdgeBuffer || y < EdgeBuffer ||
                                     x >= width - EdgeBuffer || y >= height - EdgeBuffer;

                        // Normalize noise value from [-1,1] to [0,1]
                        float normalizedValue = (landNoise[x, y] + 1) * 0.5f;

                        // Determine if pixel is land or ocean based on threshold
                        Color color = isEdge ?
                            _terrainColors["Ocean"] :
                            (normalizedValue > threshold ? _terrainColors["BaseLand"] : _terrainColors["Ocean"]);

                        row[x * 4] = color.B;
                        row[x * 4 + 1] = color.G;
                        row[x * 4 + 2] = color.R;
                        row[x * 4 + 3] = 255;
                    }
                });
            }

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        private Bitmap ApplyBiomes(Bitmap baseMap)
        {
            int width = baseMap.Width;
            int height = baseMap.Height;
            Bitmap biomeMap = new Bitmap(width, height);
            var noise = new PerlinNoise();
            float[,] elevationNoise = new float[width, height];
            float[,] moistureNoise = new float[width, height];
            float[,] lakeNoise = new float[width, height];
            float[,] temperatureNoise = new float[width, height];

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    elevationNoise[x, y] = (noise.FBM(x * 0.02f, y * 0.02f, 4) + 1) * 0.5f;
                    moistureNoise[x, y] = (noise.FBM(x * 0.025f, y * 0.025f, 4) + 1) * 0.5f;
                    lakeNoise[x, y] = noise.FBM(x * 0.05f, y * 0.05f, 2);
                    temperatureNoise[x, y] = (noise.FBM(x * 0.03f, y * 0.03f, 3) + 1) * 0.5f;
                }
            });

            bool[,] isLand = new bool[width, height];
            BitmapData baseData = baseMap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );
            BitmapData biomeData = biomeMap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb
            );

            unsafe
            {
                byte* basePtr = (byte*)baseData.Scan0;
                byte* biomePtr = (byte*)biomeData.Scan0;
                int stride = baseData.Stride;

                Parallel.For(0, height, y =>
                {
                    byte* baseRow = basePtr + y * stride;
                    byte* biomeRow = biomePtr + y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        bool isOcean = baseRow[x * 4 + 2] == 0 && baseRow[x * 4 + 1] == 0 && baseRow[x * 4] == 130;
                        isLand[x, y] = !isOcean;

                        if (!isOcean)
                        {
                            bool isNearOcean = false;
                            for (int dy = -BiomeBuffer; dy <= BiomeBuffer && !isNearOcean; dy++)
                            {
                                int ny = y + dy;
                                if (ny < 0 || ny >= height) continue;
                                for (int dx = -BiomeBuffer; dx <= BiomeBuffer && !isNearOcean; dx++)
                                {
                                    int nx = x + dx;
                                    if (nx < 0 || nx >= width) continue;
                                    byte* neighbor = basePtr + ny * stride + nx * 4;
                                    if (neighbor[2] == 0 && neighbor[1] == 0 && neighbor[0] == 130)
                                        isNearOcean = true;
                                }
                            }

                            if (!isNearOcean)
                            {
                                if (elevationNoise[x, y] < 0.4f && moistureNoise[x, y] < 0.3f && lakeNoise[x, y] > 0.7f)
                                {
                                    biomeRow[x * 4] = _terrainColors["Lake"].B;
                                    biomeRow[x * 4 + 1] = _terrainColors["Lake"].G;
                                    biomeRow[x * 4 + 2] = _terrainColors["Lake"].R;
                                }
                                else
                                {
                                    Color biomeColor = GetBiomeColor(elevationNoise[x, y], moistureNoise[x, y], temperatureNoise[x, y]);
                                    biomeRow[x * 4] = biomeColor.B;
                                    biomeRow[x * 4 + 1] = biomeColor.G;
                                    biomeRow[x * 4 + 2] = biomeColor.R;
                                }
                            }
                            else
                            {
                                biomeRow[x * 4] = baseRow[x * 4];
                                biomeRow[x * 4 + 1] = baseRow[x * 4 + 1];
                                biomeRow[x * 4 + 2] = baseRow[x * 4 + 2];
                            }
                            biomeRow[x * 4 + 3] = 255;
                        }
                        else
                        {
                            biomeRow[x * 4] = baseRow[x * 4];
                            biomeRow[x * 4 + 1] = baseRow[x * 4 + 1];
                            biomeRow[x * 4 + 2] = baseRow[x * 4 + 2];
                            biomeRow[x * 4 + 3] = 255;
                        }
                    }
                });
            }

            baseMap.UnlockBits(baseData);
            biomeMap.UnlockBits(biomeData);
            return biomeMap;
        }

        private Color GetBiomeColor(float elevation, float moisture, float temperature)
        {
            if (elevation < 0.1f) return _terrainColors["Ocean"];
            if (elevation < 0.3f) return _terrainColors["Beach"];
            if (elevation < 0.6f)
            {
                if (moisture < 0.2f) return _terrainColors["Desert"];
                if (moisture < 0.5f) return _terrainColors["Dirt"];
                if (moisture < 0.8f) return _terrainColors["Forest"];
                return _terrainColors["Jungle"];
            }
            else if (elevation < 0.8f)
            {
                if (elevation > 0.7f && temperature < 0.3f) return _terrainColors["Snow"];
                return _terrainColors["Mountain"];
            }
            else if (elevation < 0.9f)
            {
                if (temperature < 0.5f) return _terrainColors["Snow"];
                return _terrainColors["Mountain"];
            }
            return _terrainColors["Snow"];
        }

        private void terrainGenerator_menuStrip_menuStripButton_saveImage_Click(object sender, EventArgs e)
        {
            if (isSaving) return;
            isSaving = true;
            try
            {
                SaveMapToFile();
            }
            finally
            {
                isSaving = false;
            }
        }

        private void SaveMapToFile()
        {
            if (terrainGenerator_canvasDisplay.MapImage == null)
            {
                MessageBox.Show("No map to save!");
                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Bitmap Image|*.bmp";
                saveDialog.Title = "Save Map as 8-bit Bitmap";

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                Cursor = Cursors.WaitCursor;
                SystemSounds.Beep.Play();

                try
                {
                    Bitmap indexedBitmap = ConvertTo8Bit(terrainGenerator_canvasDisplay.MapImage);
                    indexedBitmap.Save(saveDialog.FileName, ImageFormat.Bmp);
                    MessageBox.Show("Map saved successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving map: {ex.Message}");
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private Bitmap ConvertTo8Bit(Bitmap bitmap)
        {
            Bitmap indexedBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
            indexedBitmap.Palette = GetTerrainPalette();

            BitmapData srcData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            BitmapData dstData = indexedBitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed
            );

            try
            {
                unsafe
                {
                    byte* srcPtr = (byte*)srcData.Scan0;
                    byte* dstPtr = (byte*)dstData.Scan0;
                    int srcStride = srcData.Stride;
                    int dstStride = dstData.Stride;
                    int height = bitmap.Height;
                    int width = bitmap.Width;

                    Dictionary<Color, byte> colorToIndex = new Dictionary<Color, byte>();
                    int index = 0;
                    foreach (var color in _terrainColors.Values)
                    {
                        if (index >= 256) break;
                        colorToIndex[color] = (byte)index;
                        index++;
                    }

                    for (int y = 0; y < height; y++)
                    {
                        byte* srcRow = srcPtr + y * srcStride;
                        byte* dstRow = dstPtr + y * dstStride;

                        for (int x = 0; x < width; x++)
                        {
                            byte b = srcRow[x * 4];
                            byte g = srcRow[x * 4 + 1];
                            byte r = srcRow[x * 4 + 2];
                            Color pixelColor = Color.FromArgb(r, g, b);

                            if (colorToIndex.TryGetValue(pixelColor, out byte colorIndex))
                            {
                                dstRow[x] = colorIndex;
                            }
                            else
                            {
                                dstRow[x] = 0; // Default to Ocean if color not found
                            }
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(srcData);
                indexedBitmap.UnlockBits(dstData);
            }

            return indexedBitmap;
        }

        private ColorPalette GetTerrainPalette()
        {
            Bitmap paletteBitmap = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            ColorPalette palette = paletteBitmap.Palette;
            int i = 0;
            foreach (var color in _terrainColors.Values)
            {
                if (i >= 256) break;
                palette.Entries[i++] = color;
            }
            for (; i < 256; i++)
                palette.Entries[i] = Color.Black;
            return palette;
        }
    }
}
