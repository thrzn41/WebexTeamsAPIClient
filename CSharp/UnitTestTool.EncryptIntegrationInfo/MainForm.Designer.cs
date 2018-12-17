namespace UnitTestTool.EncryptIntegrationInfo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labelOauthUrl = new System.Windows.Forms.Label();
            this.textBoxOAuthUrl = new System.Windows.Forms.TextBox();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.labelClientId = new System.Windows.Forms.Label();
            this.textBoxClientId = new System.Windows.Forms.TextBox();
            this.labelClientSecret = new System.Windows.Forms.Label();
            this.textBoxClientSecret = new System.Windows.Forms.TextBox();
            this.labelRedirectUri = new System.Windows.Forms.Label();
            this.textBoxRedirectUri = new System.Windows.Forms.TextBox();
            this.labelExportPath = new System.Windows.Forms.Label();
            this.textBoxExportPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelOauthUrl
            // 
            resources.ApplyResources(this.labelOauthUrl, "labelOauthUrl");
            this.labelOauthUrl.Name = "labelOauthUrl";
            // 
            // textBoxOAuthUrl
            // 
            resources.ApplyResources(this.textBoxOAuthUrl, "textBoxOAuthUrl");
            this.textBoxOAuthUrl.Name = "textBoxOAuthUrl";
            this.textBoxOAuthUrl.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonCreate
            // 
            resources.ApplyResources(this.buttonCreate, "buttonCreate");
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // labelClientId
            // 
            resources.ApplyResources(this.labelClientId, "labelClientId");
            this.labelClientId.Name = "labelClientId";
            // 
            // textBoxClientId
            // 
            resources.ApplyResources(this.textBoxClientId, "textBoxClientId");
            this.textBoxClientId.Name = "textBoxClientId";
            this.textBoxClientId.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelClientSecret
            // 
            resources.ApplyResources(this.labelClientSecret, "labelClientSecret");
            this.labelClientSecret.Name = "labelClientSecret";
            // 
            // textBoxClientSecret
            // 
            resources.ApplyResources(this.textBoxClientSecret, "textBoxClientSecret");
            this.textBoxClientSecret.Name = "textBoxClientSecret";
            this.textBoxClientSecret.UseSystemPasswordChar = true;
            this.textBoxClientSecret.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelRedirectUri
            // 
            resources.ApplyResources(this.labelRedirectUri, "labelRedirectUri");
            this.labelRedirectUri.Name = "labelRedirectUri";
            // 
            // textBoxRedirectUri
            // 
            resources.ApplyResources(this.textBoxRedirectUri, "textBoxRedirectUri");
            this.textBoxRedirectUri.Name = "textBoxRedirectUri";
            this.textBoxRedirectUri.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // labelExportPath
            // 
            resources.ApplyResources(this.labelExportPath, "labelExportPath");
            this.labelExportPath.Name = "labelExportPath";
            // 
            // textBoxExportPath
            // 
            resources.ApplyResources(this.textBoxExportPath, "textBoxExportPath");
            this.textBoxExportPath.Name = "textBoxExportPath";
            this.textBoxExportPath.ReadOnly = true;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxExportPath);
            this.Controls.Add(this.labelExportPath);
            this.Controls.Add(this.textBoxRedirectUri);
            this.Controls.Add(this.labelRedirectUri);
            this.Controls.Add(this.textBoxClientSecret);
            this.Controls.Add(this.labelClientSecret);
            this.Controls.Add(this.textBoxClientId);
            this.Controls.Add(this.labelClientId);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.textBoxOAuthUrl);
            this.Controls.Add(this.labelOauthUrl);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelOauthUrl;
        private System.Windows.Forms.TextBox textBoxOAuthUrl;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Label labelClientId;
        private System.Windows.Forms.TextBox textBoxClientId;
        private System.Windows.Forms.Label labelClientSecret;
        private System.Windows.Forms.TextBox textBoxClientSecret;
        private System.Windows.Forms.Label labelRedirectUri;
        private System.Windows.Forms.TextBox textBoxRedirectUri;
        private System.Windows.Forms.Label labelExportPath;
        private System.Windows.Forms.TextBox textBoxExportPath;
    }
}

