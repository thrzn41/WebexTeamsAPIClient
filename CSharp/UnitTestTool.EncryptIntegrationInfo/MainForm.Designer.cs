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
            this.labelOauthUrl = new System.Windows.Forms.Label();
            this.textBoxOAuthUrl = new System.Windows.Forms.TextBox();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.labelClientId = new System.Windows.Forms.Label();
            this.textBoxClientId = new System.Windows.Forms.TextBox();
            this.labelClientSecret = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labelRedirectUri = new System.Windows.Forms.Label();
            this.textBoxRedirectUri = new System.Windows.Forms.TextBox();
            this.labelExportPath = new System.Windows.Forms.Label();
            this.textBoxExportPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelOauthUrl
            // 
            this.labelOauthUrl.AutoSize = true;
            this.labelOauthUrl.Location = new System.Drawing.Point(12, 79);
            this.labelOauthUrl.Name = "labelOauthUrl";
            this.labelOauthUrl.Size = new System.Drawing.Size(136, 12);
            this.labelOauthUrl.TabIndex = 0;
            this.labelOauthUrl.Text = "OAuth Authorization URL:";
            // 
            // textBoxOAuthUrl
            // 
            this.textBoxOAuthUrl.Location = new System.Drawing.Point(154, 72);
            this.textBoxOAuthUrl.Name = "textBoxOAuthUrl";
            this.textBoxOAuthUrl.Size = new System.Drawing.Size(634, 19);
            this.textBoxOAuthUrl.TabIndex = 1;
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(11, 167);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 2;
            this.buttonCreate.Text = "CREATE";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // labelClientId
            // 
            this.labelClientId.AutoSize = true;
            this.labelClientId.Location = new System.Drawing.Point(12, 27);
            this.labelClientId.Name = "labelClientId";
            this.labelClientId.Size = new System.Drawing.Size(52, 12);
            this.labelClientId.TabIndex = 3;
            this.labelClientId.Text = "Client ID:";
            // 
            // textBoxClientId
            // 
            this.textBoxClientId.Location = new System.Drawing.Point(154, 20);
            this.textBoxClientId.Name = "textBoxClientId";
            this.textBoxClientId.Size = new System.Drawing.Size(634, 19);
            this.textBoxClientId.TabIndex = 4;
            // 
            // labelClientSecret
            // 
            this.labelClientSecret.AutoSize = true;
            this.labelClientSecret.Location = new System.Drawing.Point(12, 54);
            this.labelClientSecret.Name = "labelClientSecret";
            this.labelClientSecret.Size = new System.Drawing.Size(74, 12);
            this.labelClientSecret.TabIndex = 5;
            this.labelClientSecret.Text = "Client Secret:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(154, 47);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(634, 19);
            this.textBox1.TabIndex = 6;
            this.textBox1.UseSystemPasswordChar = true;
            // 
            // labelRedirectUri
            // 
            this.labelRedirectUri.AutoSize = true;
            this.labelRedirectUri.Location = new System.Drawing.Point(13, 104);
            this.labelRedirectUri.Name = "labelRedirectUri";
            this.labelRedirectUri.Size = new System.Drawing.Size(73, 12);
            this.labelRedirectUri.TabIndex = 7;
            this.labelRedirectUri.Text = "Redirect URI:";
            // 
            // textBoxRedirectUri
            // 
            this.textBoxRedirectUri.Location = new System.Drawing.Point(154, 97);
            this.textBoxRedirectUri.Name = "textBoxRedirectUri";
            this.textBoxRedirectUri.Size = new System.Drawing.Size(634, 19);
            this.textBoxRedirectUri.TabIndex = 8;
            this.textBoxRedirectUri.Text = "urn:ietf:wg:oauth:2.0:oob";
            // 
            // labelExportPath
            // 
            this.labelExportPath.AutoSize = true;
            this.labelExportPath.Location = new System.Drawing.Point(15, 134);
            this.labelExportPath.Name = "labelExportPath";
            this.labelExportPath.Size = new System.Drawing.Size(67, 12);
            this.labelExportPath.TabIndex = 9;
            this.labelExportPath.Text = "Export Path:";
            // 
            // textBoxExportPath
            // 
            this.textBoxExportPath.Location = new System.Drawing.Point(154, 123);
            this.textBoxExportPath.Name = "textBoxExportPath";
            this.textBoxExportPath.ReadOnly = true;
            this.textBoxExportPath.Size = new System.Drawing.Size(634, 19);
            this.textBoxExportPath.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxExportPath);
            this.Controls.Add(this.labelExportPath);
            this.Controls.Add(this.textBoxRedirectUri);
            this.Controls.Add(this.labelRedirectUri);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.labelClientSecret);
            this.Controls.Add(this.textBoxClientId);
            this.Controls.Add(this.labelClientId);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.textBoxOAuthUrl);
            this.Controls.Add(this.labelOauthUrl);
            this.Name = "MainForm";
            this.Text = "Encrypt Integration Info";
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
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label labelRedirectUri;
        private System.Windows.Forms.TextBox textBoxRedirectUri;
        private System.Windows.Forms.Label labelExportPath;
        private System.Windows.Forms.TextBox textBoxExportPath;
    }
}

