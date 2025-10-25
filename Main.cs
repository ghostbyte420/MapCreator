using MapCreator.Controls;
using MapCreator.Controls.ConfigureColorTables;
using MapCreator.Controls.DevelopmentTeamCredits;
using System.Windows.Forms;


namespace MapCreator
{
    public partial class mapCreatorMain : Form
    {
        public mapCreatorMain()
        {
            InitializeComponent();

            this.MaximizeBox = false;
        }

        private void ShowControlInsidePanel2(UserControl control)
        {
            mapCreatorMain_splitContainer.Panel2.Controls.Clear();
            control.Dock = DockStyle.Fill;
            mapCreatorMain_splitContainer.Panel2.Controls.Add(control);
        }

        private void mapCreatorMain_splitContainerPanel1_button_configureColorTables_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new configureColorTables());
        }

        private void mapCreatorMain_splitContainerPanel1_button_createMapTemplate_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new createMapTemplate());
        }

        private void mapCreatorMain_menuStrip_menuStripButton_credits_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new developmentTeamCredits());
        }
    }
}
