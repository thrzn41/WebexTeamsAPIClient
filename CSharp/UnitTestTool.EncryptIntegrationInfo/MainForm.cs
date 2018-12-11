using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnitTestTool.EncryptIntegrationInfo
{
    public partial class MainForm : Form
    {
        private DirectoryInfo dirInfo;

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            var form = new AuthForm(new Uri(this.textBoxOAuthUrl.Text));

            form.Show();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            this.dirInfo = new DirectoryInfo(userDir);

            this.textBoxExportPath.Text = String.Format("{0}{1}.thrzn41{1}unittest{1}teams", this.dirInfo.FullName, Path.DirectorySeparatorChar);
        }
    }
}
