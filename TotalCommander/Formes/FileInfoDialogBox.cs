using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TotalCommander
{
    public partial class FileInfoDialogBox : Form
    {
        public string FileName { get => textBox1.Text; }
        private FileInfo fileInfo;

        public FileInfoDialogBox(FileInfo fileInfo)
        {
            InitializeComponent();
            this.fileInfo = fileInfo;

            pictureBox1.BackgroundImage = Icon.ExtractAssociatedIcon(fileInfo.FullName).ToBitmap();
            textBox1.Text = this.fileInfo.Name;
            locationLabel.Text = Path.GetDirectoryName(this.fileInfo.FullName);
            createdLabel.Text = this.fileInfo.CreationTime.ToLongDateString();
            editedLabel.Text = this.fileInfo.LastWriteTime.ToLongDateString();
            openedLabel.Text = this.fileInfo.LastAccessTime.ToLongDateString();
            typeLabel.Text = Path.GetExtension(fileInfo.FullName);
            
            if (this.fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                checkBox1.CheckState = CheckState.Checked;

            if (this.fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly))
                checkBox2.CheckState = CheckState.Checked;
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
