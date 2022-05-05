using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TotalCommander
{
    public partial class DialogBox : Form
    {
        public string TextBox { get => textBox1.Text; }

        public DialogBox(string text, string button1Name, string button2Name)
        {
            InitializeComponent();
            this.Name = text;
            cancelButton.Text = button1Name;
            acceptButton.Text = button2Name;
        }
    }
}
