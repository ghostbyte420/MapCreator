using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MapCreator.Controls.UserSubmittedUtilities.OrbifyYourFacet
{
    public partial class orbifyYourFacet : UserControl
    {
        private int[] textures = new int[1];
        private float rotationAngle = 0f;
        private float rotationSpeed = 1f;
        private float cameraDistance = 2.5f;
        private Bitmap planetTexture;
        private System.Windows.Forms.Timer animationTimer;
        private int shaderProgram;
        private int vao;
        private int vbo;
        private int ebo;
        private Matrix4 projectionMatrix;
        private Matrix4 viewMatrix;
        private bool isGLInitialized = false;
        private int indexCount;
        private bool wireframeMode = false;
        private bool showPoles = false;      // Controls whether the poles are drawn
        private bool polesVertical = true;   // Controls spin axis (true: vertical, false: horizontal)

        public orbifyYourFacet()
        {
            // Enable double buffering to reduce flicker
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint, true);
            this.UpdateStyles();

            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            orbifyYourFacet_trackBar_speedControl.Minimum = 0;
            orbifyYourFacet_trackBar_speedControl.Maximum = 100;
            orbifyYourFacet_trackBar_speedControl.Value = 10;
            orbifyYourFacet_trackBar_speedControl.ValueChanged += orbifyYourFacet_trackBar_speedControl_ValueChanged;
            orbifyYourFacet_trackBar_zoomControl.Minimum = 1;

            #region Change The Facet Maximum Zoom Perspective

            orbifyYourFacet_trackBar_zoomControl.Maximum = 134;

            #endregion

            orbifyYourFacet_trackBar_zoomControl.Value = 100;
            orbifyYourFacet_trackBar_zoomControl.ValueChanged += orbifyYourFacet_trackBar_zoomControl_ValueChanged;
            orbifyYourFacet_trackBar_zoomControl_numericUpDown.Minimum = 1;
            orbifyYourFacet_trackBar_zoomControl_numericUpDown.Maximum = 200;
            orbifyYourFacet_trackBar_zoomControl_numericUpDown.Value = 100;
            orbifyYourFacet_trackBar_zoomControl_numericUpDown.ValueChanged += orbifyYourFacet_trackBar_zoomControl_numericUpDown_ValueChanged;
            orbifyYourFacet_checkBox_orbSpinDirection.CheckedChanged += orbifyYourFacet_checkBox_orbSpinDirection_CheckedChanged;
            orbifyYourFacet_menuStrip_options_dropDownMenu_wireFrame_on.Click += orbifyYourFacet_menuStrip_options_dropDownMenu_wireFrame_on_Click;
            orbifyYourFacet_menuStrip_options_dropDownMenu_wireFrame_off.Click += orbifyYourFacet_menuStrip_options_dropDownMenu_wireFrame_off_Click;
            orbifyYourFacet_menuStrip_options_dropDownMenu_viewPoles_on.Click += orbifyYourFacet_menuStrip_options_dropDownMenu_viewPoles_on_Click;
            orbifyYourFacet_menuStrip_options_dropDownMenu_viewPoles_off.Click += orbifyYourFacet_menuStrip_options_dropDownMenu_viewPoles_off_Click;
            animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
            orbifyYourFacet_trackBar_speedControl.AutoSize = false;
            orbifyYourFacet_trackBar_speedControl.Size = new Size(269, 30);
            orbifyYourFacet_trackBar_zoomControl.AutoSize = false;
            orbifyYourFacet_trackBar_zoomControl.Size = new Size(269, 30);
        }

        private void orbifyYourFacet_menuStrip_menuStripButton_loadFacet_Click(object sender, EventArgs e)
        {
            if (!isGLInitialized) return;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Image files (*.png;*.bmp)|*.png;*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    planetTexture?.Dispose();
                    planetTexture = new Bitmap(dlg.FileName);
                    LoadTexture(planetTexture, 0);
                    orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invalidate();
                }
            }
        }

        private void orbifyYourFacet_menuStrip_options_dropDownMenu_wireFrame_on_Click(object sender, EventArgs e)
        {
            wireframeMode = true;
            orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invalidate();
        }

        private void orbifyYourFacet_menuStrip_options_dropDownMenu_wireFrame_off_Click(object sender, EventArgs e)
        {
            wireframeMode = false;
            orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invalidate();
        }

        private void orbifyYourFacet_menuStrip_options_dropDownMenu_viewPoles_on_Click(object sender, EventArgs e)
        {
            polesVertical = false;
            showPoles = true;
            orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invalidate();
        }

        private void orbifyYourFacet_menuStrip_options_dropDownMenu_viewPoles_off_Click(object sender, EventArgs e)
        {
            polesVertical = true;
            showPoles = false;
            orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invalidate();
        }

        private void orbifyYourFacet_panel_canvas_glControl_orbDisplay_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.GenTextures(1, textures);
            LoadShaders();
            SetupBuffers();
            isGLInitialized = true;
        }

        private void orbifyYourFacet_panel_canvas_glControl_orbDisplay_Paint(object sender, PaintEventArgs e)
        {
            if (!isGLInitialized) return;
            if (!orbifyYourFacet_panel_canvas_glControl_orbDisplay.Context.IsCurrent)
            {
                orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invoke((MethodInvoker)delegate
                {
                    orbifyYourFacet_panel_canvas_glControl_orbDisplay_Paint(sender, e);
                });
                return;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (wireframeMode)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.LineWidth(1.5f);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            if (planetTexture != null)
            {
                GL.UseProgram(shaderProgram);
                viewMatrix = Matrix4.LookAt(new Vector3(0, 0, cameraDistance), Vector3.Zero, Vector3.UnitY);
                var modelMatrix = Matrix4.Identity;
                if (polesVertical)
                {
                    modelMatrix *= Matrix4.CreateRotationY(rotationAngle); // Vertical spin
                }
                else
                {
                    modelMatrix *= Matrix4.CreateRotationX(rotationAngle); // Horizontal spin
                }
                int projectionLocation = GL.GetUniformLocation(shaderProgram, "uProjection");
                GL.UniformMatrix4(projectionLocation, false, ref projectionMatrix);
                int viewLocation = GL.GetUniformLocation(shaderProgram, "uView");
                GL.UniformMatrix4(viewLocation, false, ref viewMatrix);
                int modelLocation = GL.GetUniformLocation(shaderProgram, "uModel");
                GL.UniformMatrix4(modelLocation, false, ref modelMatrix);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textures[0]);
                GL.Uniform1(GL.GetUniformLocation(shaderProgram, "uTexture"), 0);
                GL.BindVertexArray(vao);
                GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);

                if (showPoles)
                {
                    GL.UseProgram(0);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(Color.Red);
                    if (polesVertical)
                    {
                        // Vertical poles (top and bottom)
                        GL.Vertex3(0.0f, -1.2f, 0.0f);
                        GL.Vertex3(0.0f, 1.2f, 0.0f);
                    }
                    else
                    {
                        // Horizontal poles (left and right)
                        GL.Vertex3(-1.2f, 0.0f, 0.0f);
                        GL.Vertex3(1.2f, 0.0f, 0.0f);
                    }
                    GL.End();
                }
            }

            orbifyYourFacet_panel_canvas_glControl_orbDisplay.SwapBuffers();
            CheckGLError();
        }

        private void orbifyYourFacet_panel_canvas_glControl_orbDisplay_Resize(object sender, EventArgs e)
        {
            if (!isGLInitialized) return;
            GL.Viewport(0, 0, orbifyYourFacet_panel_canvas_glControl_orbDisplay.Width, orbifyYourFacet_panel_canvas_glControl_orbDisplay.Height);
            float aspectRatio = (float)orbifyYourFacet_panel_canvas_glControl_orbDisplay.Width / orbifyYourFacet_panel_canvas_glControl_orbDisplay.Height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3, aspectRatio, 0.1f, 100.0f);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (isGLInitialized)
            {
                rotationAngle += rotationSpeed * 0.02f;
                orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invalidate();
            }
        }

        private void UpdateCameraDistance()
        {
            float minDistance = 1.04f;
            float maxDistance = 5.0f;
            cameraDistance = maxDistance - ((orbifyYourFacet_trackBar_zoomControl.Value - 1) * (maxDistance - minDistance) / 199f);
            orbifyYourFacet_panel_canvas_glControl_orbDisplay.Invalidate();
        }

        private void LoadShaders()
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, @"
                #version 330 core
                layout(location = 0) in vec3 aPosition;
                layout(location = 1) in vec2 aTexCoord;
                uniform mat4 uProjection;
                uniform mat4 uView;
                uniform mat4 uModel;
                out vec2 vTexCoord;
                void main() { gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0); vTexCoord = aTexCoord; }
            ");
            GL.CompileShader(vertexShader);
            CheckShaderError(vertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, @"
                #version 330 core
                in vec2 vTexCoord;
                out vec4 FragColor;
                uniform sampler2D uTexture;
                void main() { FragColor = texture(uTexture, vTexCoord); }
            ");
            GL.CompileShader(fragmentShader);
            CheckShaderError(fragmentShader);
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            CheckProgramError(shaderProgram);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        private void SetupBuffers()
        {
            int stacks = 30;
            int sectors = 120; // Increased sectors for smoother texture mapping
            float radius = 1.0f;
            List<float> vertices = new List<float>();
            List<uint> indicesList = new List<uint>();
            float sectorStep = 2 * (float)Math.PI / sectors;
            float stackStep = (float)Math.PI / stacks;
            float sectorAngle, stackAngle;
            for (int i = 0; i <= stacks; ++i)
            {
                stackAngle = (float)Math.PI / 2 - i * stackStep;
                float xy = radius * (float)Math.Cos(stackAngle);
                float z = radius * (float)Math.Sin(stackAngle);
                for (int j = 0; j <= sectors; ++j)
                {
                    sectorAngle = j * sectorStep;
                    float x = xy * (float)Math.Cos(sectorAngle);
                    float y = z;
                    float zz = xy * (float)Math.Sin(sectorAngle);
                    vertices.Add(x);
                    vertices.Add(y);
                    vertices.Add(zz);
                    float s = (float)j / sectors;
                    // Overlap edges slightly to hide the seam
                    if (j == sectors) s = 1.0f - (1.0f / sectors);
                    else if (j == 0) s = 1.0f / sectors;
                    float t = (float)i / stacks;
                    vertices.Add(s);
                    vertices.Add(t);
                }
            }
            for (int i = 0; i < stacks; ++i)
            {
                for (int j = 0; j < sectors; ++j)
                {
                    int first = (i * (sectors + 1)) + j;
                    int second = first + sectors + 1;
                    indicesList.Add((uint)first);
                    indicesList.Add((uint)second);
                    indicesList.Add((uint)(first + 1));
                    indicesList.Add((uint)second);
                    indicesList.Add((uint)(second + 1));
                    indicesList.Add((uint)(first + 1));
                }
            }
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Count * sizeof(float)), vertices.ToArray(), BufferUsageHint.StaticDraw);
            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(indicesList.Count * sizeof(uint)), indicesList.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            indexCount = indicesList.Count;
        }

        private void LoadTexture(Bitmap bitmap, int textureId)
        {
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, textures[textureId]);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                bitmap.Width,
                bitmap.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D); // Generate mipmaps
            bitmap.UnlockBits(data);
        }

        private void orbifyYourFacet_checkBox_orbSpinDirection_CheckedChanged(object sender, EventArgs e)
        {
            rotationSpeed = orbifyYourFacet_checkBox_orbSpinDirection.Checked ? -Math.Abs(rotationSpeed) : Math.Abs(rotationSpeed);
        }

        private void orbifyYourFacet_trackBar_speedControl_ValueChanged(object sender, EventArgs e)
        {
            rotationSpeed = orbifyYourFacet_trackBar_speedControl.Value * 0.1f;
        }

        private void orbifyYourFacet_trackBar_zoomControl_ValueChanged(object sender, EventArgs e)
        {
            orbifyYourFacet_trackBar_zoomControl_numericUpDown.Value = orbifyYourFacet_trackBar_zoomControl.Value;
            UpdateCameraDistance();
        }

        private void orbifyYourFacet_trackBar_zoomControl_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            orbifyYourFacet_trackBar_zoomControl.Value = (int)orbifyYourFacet_trackBar_zoomControl_numericUpDown.Value;
            UpdateCameraDistance();
        }

        private void CheckGLError()
        {
            ErrorCode err;
            while ((err = GL.GetError()) != ErrorCode.NoError)
            {
                Console.WriteLine($"OpenGL Error: {err}");
            }
        }

        private void CheckProgramError(int program)
        {
            int status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"Program linking failed: {infoLog}");
            }
        }

        private void CheckShaderError(int shader)
        {
            int status;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"Shader compilation failed: {infoLog}");
            }
        }

        public void Dispose()
        {
            if (isGLInitialized)
            {
                GL.DeleteTextures(1, textures);
                GL.DeleteBuffer(vbo);
                GL.DeleteBuffer(ebo);
                GL.DeleteVertexArray(vao);
                GL.DeleteProgram(shaderProgram);
            }
            planetTexture?.Dispose();
            animationTimer?.Dispose();
        }
    }
}
