using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;

using System.Windows.Forms;

namespace TotalCommander
{
    public partial class DirectoryInfoDialogBox : Form
    {
        public string FileName { get=>textBox1.Text; }
        private DirectoryInfo dirInfo;      
        public DirectoryInfoDialogBox(DirectoryInfo dirInfo)
        {
            InitializeComponent();
            this.dirInfo = dirInfo;
            

            textBox1.Text = this.dirInfo.Name;
            locationLabel.Text = Path.GetDirectoryName(this.dirInfo.FullName);
            createdLabel.Text = this.dirInfo.CreationTime.ToLongDateString();

            if (this.dirInfo.Attributes.HasFlag(FileAttributes.Hidden))
                visibleCheckBox.CheckState = CheckState.Checked;

            if (this.dirInfo.Attributes.HasFlag(FileAttributes.ReadOnly))
                readonlyCheckBox.CheckState = CheckState.Checked;
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
