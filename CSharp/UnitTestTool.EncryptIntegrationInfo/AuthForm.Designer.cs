namespace UnitTestTool.EncryptIntegrationInfo
{
    partial class AuthForm
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
            this.webBrowserAuth = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowserAuth
            // 
            this.webBrowserAuth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserAuth.Location = new System.Drawing.Point(0, 0);
            this.webBrowserAuth.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserAuth.Name = "webBrowserAuth";
            this.webBrowserAuth.Size = new System.Drawing.Size(724, 601);
            this.webBrowserAuth.TabIndex = 0;
            this.webBrowserAuth.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserAuth_DocumentCompleted);
            // 
            // AuthForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 601);
            this.Controls.Add(this.webBrowserAuth);
            this.MinimumSize = new System.Drawing.Size(256, 256);
            this.Name = "AuthForm";
            this.Text = "AuthForm";
            this.Load += new System.EventHandler(this.AuthForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowserAuth;
    }
}