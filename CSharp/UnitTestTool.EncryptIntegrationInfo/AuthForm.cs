﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnitTestTool.EncryptIntegrationInfo
{
    public partial class AuthForm : Form
    {

        private readonly Uri authUri;

        public AuthForm()
        {
            InitializeComponent();
        }

        public AuthForm(Uri uri)
            : this()
        {
            this.authUri = uri;
        }

        private void AuthForm_Load(object sender, EventArgs e)
        {
            this.webBrowserAuth.Navigate(this.authUri);
        }

        private void webBrowserAuth_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string bodyText = this.webBrowserAuth.Document?.Body?.OuterText;

            if(!String.IsNullOrEmpty(bodyText) && bodyText.StartsWith("code="))
            {
                this.Close();
            }
        }
    }
}
