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
using Thrzn41.Util;

namespace UnitTestTool.EncryptTeamsInfo
{
    public partial class MainForm : Form
    {
        private DirectoryInfo dirInfo;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            this.dirInfo = new DirectoryInfo(userDir);

            this.textBoxPath.Text = String.Format("{0}{1}.thrzn41{1}unittest{1}teams", this.dirInfo.FullName, Path.DirectorySeparatorChar);
        }

        private async Task exportEncryptedToken()
        {
            DirectoryInfo exportDir = this.dirInfo.CreateSubdirectory(".thrzn41").CreateSubdirectory("unittest").CreateSubdirectory("teams");

            LocalProtectedString ps = LocalProtectedString.FromString(this.textBoxToken.Text);

            using (var fs = new FileStream(String.Format("{0}{1}teamstoken.dat", exportDir.FullName, Path.DirectorySeparatorChar), FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await fs.WriteAsync(ps.EncryptedData, 0, ps.EncryptedData.Length);
            }

            using (var fs = new FileStream(String.Format("{0}{1}tokenentropy.dat", exportDir.FullName, Path.DirectorySeparatorChar), FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await fs.WriteAsync(ps.Entropy, 0, ps.Entropy.Length);
            }


        }

        private async void buttonExportToken_Click(object sender, EventArgs e)
        {
            try
            {
                this.buttonExportToken.Enabled = false;

                await exportEncryptedToken();

                MessageBox.Show(ResourceMessage.AppMessages.TeamsInfoExported);
            }
            finally
            {
                this.buttonExportToken.Enabled = true;
            }
        }
    }
}
