using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator
{
    public class CanvasDisplay : Panel
    {
        private Bitmap _mapImage;
        public Point _panOffset = Point.Empty;
        private float _zoom = 1.0f;
        private Point _lastMousePos;
        private bool _isPanning = false;
        private bool _canPan = true;
        private float _fitToScreenScale = 1.0f;
        private Rectangle _selectionRect = Rectangle.Empty;
        private bool _isSelecting = false;
        private Point _selectionStart;
        private Point _lastZoomPoint = Point.Empty;  // Track last zoom point

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Bitmap MapImage
        {
            get => _mapImage;
            set
            {
                _mapImage = value;
                if (_mapImage != null)
                {
                    _panOffset = Point.Empty;
                    _zoom = 1.0f;
                    _lastZoomPoint = Point.Empty;
                    CalculateFitToScreenScale();
                    CenterImage();
                }
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Zoom
        {
            get => _zoom;
            set
            {
                float oldZoom = _zoom;
                _zoom = Math.Max(0.1f, Math.Min(value, 10f));
                _canPan = true;

                if (_mapImage != null && oldZoom != _zoom)
                {
                    // Don't recenter when zooming, maintain current view
                    if (_lastZoomPoint != Point.Empty)
                    {
                        // Calculate new pan offset to keep the zoom point centered
                        float oldScale = GetEffectiveScale(oldZoom);
                        float newScale = GetEffectiveScale();

                        _panOffset = new Point(
                            (int)(_lastZoomPoint.X - (_lastZoomPoint.X - _panOffset.X) * (newScale / oldScale)),
                            (int)(_lastZoomPoint.Y - (_lastZoomPoint.Y - _panOffset.Y) * (newScale / oldScale))
                        );
                    }
                }

                if (Parent is terrainGenerator generator)
                {
                    generator.UpdateCanvasZoom();
                }

                Invalidate();
            }
        }

        public CanvasDisplay()
        {
            DoubleBuffered = true;
            AutoScroll = false;
            BackColor = Color.Black;
            this.SetStyle(ControlStyles.Selectable, true);
            this.TabStop = true;
            this.MouseDown += CanvasDisplay_MouseDown;
            this.MouseMove += CanvasDisplay_MouseMove;
            this.MouseUp += CanvasDisplay_MouseUp;
            this.MouseWheel += CanvasDisplay_MouseWheel;
            this.Paint += CanvasDisplay_Paint;
            this.Resize += CanvasDisplay_Resize;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                CopySelectionToClipboard();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CalculateFitToScreenScale()
        {
            if (_mapImage == null || Width <= 0 || Height <= 0) return;
            float scaleX = (float)Width / _mapImage.Width;
            float scaleY = (float)Height / _mapImage.Height;
            _fitToScreenScale = Math.Min(scaleX, scaleY);
            _fitToScreenScale = Math.Max(_fitToScreenScale, 1.0f);
        }

        private void CenterImage()
        {
            if (_mapImage == null) return;
            float scale = GetEffectiveScale();
            int scaledWidth = (int)(_mapImage.Width * scale);
            int scaledHeight = (int)(_mapImage.Height * scale);
            _panOffset.X = (Width - scaledWidth) / 2;
            _panOffset.Y = (Height - scaledHeight) / 2;
        }

        private float GetEffectiveScale()
        {
            return _zoom == 1.0f ? _fitToScreenScale : _zoom;
        }

        private float GetEffectiveScale(float zoom)
        {
            return zoom == 1.0f ? _fitToScreenScale : zoom;
        }

        private void CanvasDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
            if (e.Button == MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    _isPanning = true;
                    _lastMousePos = e.Location;
                    Cursor = Cursors.Hand;
                    _isSelecting = false;
                    _selectionRect = Rectangle.Empty;
                }
                else
                {
                    _isSelecting = true;
                    _selectionStart = e.Location;
                    _selectionRect = new Rectangle(e.Location, new Size(0, 0));
                }
                Invalidate();
            }
        }

        private void CanvasDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                int deltaX = e.X - _lastMousePos.X;
                int deltaY = e.Y - _lastMousePos.Y;
                _panOffset.X += deltaX;
                _panOffset.Y += deltaY;
                _lastMousePos = e.Location;
                Invalidate();
            }
            else if (_isSelecting)
            {
                _selectionRect = new Rectangle(
                    Math.Min(_selectionStart.X, e.X),
                    Math.Min(_selectionStart.Y, e.Y),
                    Math.Abs(_selectionStart.X - e.X),
                    Math.Abs(_selectionStart.Y - e.Y)
                );
                Invalidate();
            }
        }

        private void CanvasDisplay_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                Cursor = Cursors.Default;
            }
            else if (_isSelecting)
            {
                _isSelecting = false;
                Invalidate();
            }
        }

        private void CanvasDisplay_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_mapImage == null) return;

            // Store the mouse position for zoom centering
            _lastZoomPoint = e.Location;

            float oldZoom = _zoom;
            float zoomFactor = e.Delta > 0 ? 1.1f : 0.9f;
            float newZoom = _zoom * zoomFactor;
            newZoom = Math.Max(0.1f, Math.Min(newZoom, 10f));

            if (newZoom != _zoom)
            {
                Zoom = newZoom;  // This will handle the zoom centering
            }
        }

        private void CanvasDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (_mapImage == null) return;
            e.Graphics.Clear(BackColor);

            float scale = GetEffectiveScale();
            int destWidth = (int)(_mapImage.Width * scale);
            int destHeight = (int)(_mapImage.Height * scale);

            // Draw the image
            e.Graphics.DrawImage(_mapImage, _panOffset.X, _panOffset.Y, destWidth, destHeight);

            // Draw selection rectangle if needed
            if (_isSelecting || (!_selectionRect.IsEmpty && !_isPanning))
            {
                using (Pen pen = new Pen(Color.White, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, _selectionRect);
                }
            }
        }

        private void CanvasDisplay_Resize(object sender, EventArgs e)
        {
            CalculateFitToScreenScale();
            if (_mapImage != null)
            {
                CenterImage();
            }
            Invalidate();
        }

        public void CopySelectionToClipboard()
        {
            if (_selectionRect.IsEmpty || _mapImage == null)
            {
                MessageBox.Show("No selection or map to copy!");
                return;
            }

            Rectangle srcRect = new Rectangle(
                (int)((_selectionRect.X - _panOffset.X) / GetEffectiveScale()),
                (int)((_selectionRect.Y - _panOffset.Y) / GetEffectiveScale()),
                (int)(_selectionRect.Width / GetEffectiveScale()),
                (int)(_selectionRect.Height / GetEffectiveScale())
            );

            srcRect = Rectangle.Intersect(srcRect, new Rectangle(0, 0, _mapImage.Width, _mapImage.Height));

            if (srcRect.Width > 0 && srcRect.Height > 0)
            {
                Bitmap selected = new Bitmap(srcRect.Width, srcRect.Height);
                using (Graphics g = Graphics.FromImage(selected))
                {
                    g.DrawImage(_mapImage, 0, 0, srcRect, GraphicsUnit.Pixel);
                }
                Clipboard.SetImage(selected);
                selected.Dispose();
                SystemSounds.Beep.Play();
                MessageBox.Show("Selection copied to clipboard!");
            }
            else
            {
                MessageBox.Show("No valid selection to copy!");
            }
        }

        public void ClearSelection()
        {
            _selectionRect = Rectangle.Empty;
            Invalidate();
        }
    }
}
