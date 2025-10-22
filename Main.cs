using MapCreator.Controls.ConfigureColorTables;
using System.Windows.Forms;


namespace MapCreator
{
    public partial class mapCreatorMain : Form
    {
        public mapCreatorMain()
        {
            InitializeComponent();

            this.MaximizeBox = false;  // disables and hides maximize button (no gray box)

            mapCreatorMain_splitContainerPanel1_button_configureColorTables.Click += mapCreatorMain_splitContainerPanel1_button_configureColorTables_Click;           
        }

        private void ShowControlInsidePanel2 (UserControl control)
        {
            mapCreatorMain_splitContainer.Panel2.Controls.Clear();
            control.Dock = DockStyle.Fill;
            mapCreatorMain_splitContainer.Panel2.Controls.Add(control);

        }



        private void mapCreatorMain_splitContainerPanel1_button_configureColorTables_Click(object sender, EventArgs e)
        {
            ShowControlInsidePanel2(new configureColorTables());
        }
    }
}
