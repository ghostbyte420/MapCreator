using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Linq;

namespace MapCreator.Controls.DevelopmentTeamCredits
{
    public partial class developmentTeamCredits : UserControl
    {
        private System.Windows.Forms.Timer scrollTimer;
        private const int VerticalSpacing = 35; // Space between lines
        private PictureBox dividerImage;
        private int totalContentHeight = 0; // Track the total height of all controls

        public developmentTeamCredits()
        {
            InitializeComponent();
            developmentTeamCredits_panel.BackColor = Color.Transparent;
            // Enable double buffering for smooth scrolling
            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                developmentTeamCredits_panel,
                new object[] { true }
            );
            SetupCredits();
        }

        private void SetupCredits()
        {
            int y = 50; // Start Y position

            // Developer of UOLandscaper
            AddCenteredLabel(developmentTeamCredits_panel, "Developer of UOLandscaper", y, Color.Black, big: true);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "dknight", y, Color.DarkGreen);
            y += VerticalSpacing;

            // MapCreator Development
            AddCenteredLabel(developmentTeamCredits_panel, "MapCreator Development", y, Color.Black, big: true);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "gametec", y, Color.DarkGreen);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "Otimpire", y, Color.DarkGreen);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "Praxiiz", y, Color.DarkGreen);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "Voxpire", y, Color.DarkGreen);
            y += VerticalSpacing;

            // Development Assistance
            AddCenteredLabel(developmentTeamCredits_panel, "Development Assistance", y, Color.Black, big: true);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "KARASHO'", y, Color.DarkGreen);
            y += VerticalSpacing;

            // AI Used In Development
            AddCenteredLabel(developmentTeamCredits_panel, "AI Used In Development", y, Color.Black, big: true);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "Mistra.AI LeChat", y, Color.DarkGreen);
            y += VerticalSpacing;

            // MapCreator Tester(s)
            AddCenteredLabel(developmentTeamCredits_panel, "MapCreator Tester(s)", y, Color.Black, big: true);
            y += VerticalSpacing;
            AddCenteredLabel(developmentTeamCredits_panel, "Golfin", y, Color.DarkGreen);
            y += VerticalSpacing;

            #region Adjust The Image Divider Position

            // Add the image divider
            int imageYPosition = y + 45;
            AddImageDivider(developmentTeamCredits_panel, imageYPosition);

            #endregion

            // Calculate total content height for seamless looping
            totalContentHeight = developmentTeamCredits_panel.Controls.Cast<Control>().Max(c => c.Top + c.Height);

            // Scroll animation
            scrollTimer = new System.Windows.Forms.Timer();
            scrollTimer.Interval = 16;
            scrollTimer.Tick += ScrollAllLabelsUp;
            scrollTimer.Start();
        }

        // Centers a label in the panel at the given Y position
        private void AddCenteredLabel(Panel panel, string text, int y, Color color, bool big = false)
        {
            Label label = new Label
            {
                Text = text,
                ForeColor = color,

                #region Change Name and Role Font Sizes
                // roles : names (numeric format)

                Font = new Font("Arial", big ? 13 : 12, big ? FontStyle.Bold : FontStyle.Regular), // Names are now size 14

                #endregion

                AutoSize = true,
                BackColor = Color.Transparent,
                Top = y,
            };
            label.Left = (panel.Width - label.PreferredWidth) / 2;
            panel.Controls.Add(label);
        }

        // Adds an image divider from resources at the specified Y position
        private void AddImageDivider(Panel panel, int y)
        {
            dividerImage = new PictureBox
            {
                Image = Properties.Resources.img_0017a,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Top = y,
                BackColor = Color.Transparent
            };
            dividerImage.Left = (panel.Width - dividerImage.Width) / 2;
            panel.Controls.Add(dividerImage);
        }

        // Scroll all labels and the image up smoothly
        private void ScrollAllLabelsUp(object sender, EventArgs e)
        {
            foreach (Control control in developmentTeamCredits_panel.Controls)
            {
                control.Top -= 1;
                if (control.Top + control.Height < 0) // Fully scrolled off the top
                    control.Top = totalContentHeight; // Reset to the bottom of the total content
            }
        }
    }
}
