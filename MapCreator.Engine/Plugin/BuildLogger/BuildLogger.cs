using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MapCreator.Engine.Plugin.BuildLogger
{
    public partial class buildLogger : Form
    {
        private string m_LogName;
        private DateTime m_Task_Start;
        private DateTime m_Task_End;
        private readonly PrintDocument document = new PrintDocument();
        private readonly PrintDialog dialog = new PrintDialog();

        public buildLogger()
        {
            var buildLogger = this;
            base.Load += new EventHandler(buildLogger.buildLogger_Load);
            document.PrintPage += new PrintPageEventHandler(document_PrintPage);
            InitializeComponent();
        }

        private void buildLogger_Load(object sender, EventArgs e)
        {
            var log = new StringBuilder();
            log.AppendLine("MapCreator: Logger                      |");
            log.AppendLine(DateTime.UtcNow.ToString("dd.MM.yyyy - hh:mm:ss tt"));
            log.AppendLine("========================================");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.EntryPoint != null)
                {
                    m_LogName = assembly.EntryPoint.DeclaringType.Name;
                    var name = assembly.GetName();
                    log.AppendLine($"{name.Name} version: {name.Version}");

                    foreach (var refAssembly in assembly.GetReferencedAssemblies())
                    {
                        log.AppendLine($"{refAssembly.Name} version: {refAssembly.Version}");
                    }
                }
            }

            buildLogger_textBox_logDisplay.Text = log.ToString();
        }

        public void StartTask()
        {
            m_Task_Start = DateTime.UtcNow;
        }

        public void LogMessage(string message)
        {
            if (buildLogger_textBox_logDisplay.InvokeRequired)
            {
                buildLogger_textBox_logDisplay.Invoke(new Action(() =>
                {
                    buildLogger_textBox_logDisplay.Text += message + "\r\n";
                    Refresh();
                }));
            }
            else
            {
                buildLogger_textBox_logDisplay.Text += message + "\r\n";
                Refresh();
            }
        }

        public void LogTimeStamp()
        {
            if (buildLogger_textBox_logDisplay.InvokeRequired)
            {
                buildLogger_textBox_logDisplay.Invoke(new Action(() =>
                {
                    var elapsed = (m_Task_End - m_Task_Start).TotalSeconds;
                    buildLogger_textBox_logDisplay.Text += $"  Task: {m_Task_Start:dd/MMM/yyyy hh:mm:ss} ===> {m_Task_End:hh:mm:ss}  Total: {elapsed} seconds\r\n";
                    Refresh();
                }));
            }
            else
            {
                var elapsed = (m_Task_End - m_Task_Start).TotalSeconds;
                buildLogger_textBox_logDisplay.Text += $"  Task: {m_Task_Start:dd/MMM/yyyy hh:mm:ss} ===> {m_Task_End:hh:mm:ss}  Total: {elapsed} seconds\r\n";
                Refresh();
            }
        }

        public void EndTask()
        {
            m_Task_End = DateTime.UtcNow;
        }

        private void buildLogger_menuStrip_menuStripButton_saveAs_Click(object sender, EventArgs e)
        {
            var log = new SaveFileDialog
            {
                FileName = "DefaultLog.txt",
                Filter = "ProgressLog | (*.txt)"
            };
            if (log.ShowDialog() == DialogResult.OK)
            {
                using var sw = new StreamWriter(log.FileName, true);
                sw.Write(buildLogger_textBox_logDisplay.Text);
                sw.Flush();
            }
        }

        private void document_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(buildLogger_textBox_logDisplay.Text, new Font("Arial", 20, FontStyle.Regular), Brushes.Black, 20, 20);
        }

        private void buildLogger_menuStrip_menuStripButton_printLog_Click(object sender, EventArgs e)
        {
            dialog.Document = document;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                document.Print();
            }
        }
    }
}
