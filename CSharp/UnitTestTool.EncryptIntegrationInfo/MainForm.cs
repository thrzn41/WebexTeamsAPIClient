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
using Thrzn41.WebexTeams;
using Thrzn41.WebexTeams.Version1;
using UnitTestTool.EncryptIntegrationInfo.ResourceMessage;

namespace UnitTestTool.EncryptIntegrationInfo
{
    public partial class MainForm : Form
    {
        private DirectoryInfo dirInfo;

        public MainForm()
        {
            InitializeComponent();
        }

        public bool checkTextBoxes()
        {
            string oauthUrl    = this.textBoxOAuthUrl.Text.Trim();

            return ( !String.IsNullOrEmpty(this.textBoxClientId.Text) &&
                     !String.IsNullOrEmpty(this.textBoxClientSecret.Text) &&
                     !String.IsNullOrEmpty(oauthUrl) &&
                     oauthUrl.StartsWith("https://api.ciscospark.com/v1/authorize") &&
                     !String.IsNullOrEmpty(this.textBoxRedirectUri.Text.Trim())) ;
        }

        private async void buttonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSucceeded = false;

                this.textBoxClientId.Enabled     = false;
                this.textBoxClientSecret.Enabled = false;
                this.textBoxOAuthUrl.Enabled     = false;
                this.textBoxRedirectUri.Enabled  = false;

                this.buttonCreate.Enabled = false;

                var form = new AuthForm(new Uri(this.textBoxOAuthUrl.Text));

                var result = form.ShowDialog();

                if(result == DialogResult.OK)
                {
                    var oauth2 = TeamsAPI.CreateVersion1OAuth2Client(
                        this.textBoxClientSecret.Text,
                        this.textBoxClientId.Text,
                        new TeamsRetryOnErrorHandler(4, TimeSpan.FromSeconds(15.0f)));

                    var r = await oauth2.GetTokenInfoAsync(
                        form.Code,
                        this.textBoxRedirectUri.Text.Trim());

                    if(r.IsSuccessStatus)
                    {
                        var info = new TeamsIntegrationInfo();

                        info.ClientId     = this.textBoxClientId.Text;
                        info.ClientSecret = this.textBoxClientSecret.Text;
                        info.OAuthUrl     = this.textBoxOAuthUrl.Text;
                        info.RedirectUri  = this.textBoxRedirectUri.Text;

                        info.TokenInfo = r.Data;


                        DirectoryInfo exportDir = this.dirInfo.CreateSubdirectory(".thrzn41").CreateSubdirectory("unittest").CreateSubdirectory("teams");

                        var s = info.ToJsonString();

                        LocalProtectedString ps = LocalProtectedString.FromString(info.ToJsonString());

                        using (var fs = new FileStream(String.Format("{0}{1}teamsintegration.dat", exportDir.FullName, Path.DirectorySeparatorChar), FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await fs.WriteAsync(ps.EncryptedData, 0, ps.EncryptedData.Length);
                        }

                        using (var fs = new FileStream(String.Format("{0}{1}integrationentropy.dat", exportDir.FullName, Path.DirectorySeparatorChar), FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await fs.WriteAsync(ps.Entropy, 0, ps.Entropy.Length);
                        }


                        isSucceeded = true;
                    }

                }

                MessageBox.Show((isSucceeded ? AppMessages.CreateSucceeded : AppMessages.CreateFailed));
            }
            finally
            {
                this.buttonCreate.Enabled = true;

                this.textBoxRedirectUri.Enabled  = true;
                this.textBoxOAuthUrl.Enabled     = true;
                this.textBoxClientSecret.Enabled = true;
                this.textBoxClientId.Enabled     = true;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            this.dirInfo = new DirectoryInfo(userDir);

            this.textBoxExportPath.Text = String.Format("{0}{1}.thrzn41{1}unittest{1}teams", this.dirInfo.FullName, Path.DirectorySeparatorChar);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            this.buttonCreate.Enabled = checkTextBoxes();
        }
    }
}
