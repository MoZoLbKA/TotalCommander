using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace TotalCommander
{
    public partial class MainForm : Form, IFormData
    {
        private FileManager fileManager;
        private DialogBox dlgBox;
        private SearchDialogBox searchDlgBox;

        public MainForm()
        {
            InitializeComponent();
            dlgBox = new DialogBox("Введите имя", "Отмена", "Подтвердить");

            listView1.GotFocus += LeftListViewGotFocus;
            listView2.GotFocus += RightListViewGotFocus;

            listView1.ItemActivate += ListViewDoubleClick;
            listView2.ItemActivate += ListViewDoubleClick;
            
            fileManager = new FileManager(listView1, listView2, imageList1, imageList2, imageList3, imageList4);

            textBox1.Text = fileManager.LeftDirectory.FullName;
            textBox2.Text = fileManager.RightDirectory.FullName;

            comboBox1.Items.AddRange(fileManager.Drives.Disks.ToArray());
            comboBox2.Items.AddRange(fileManager.Drives.Disks.ToArray());

            comboBox1.SelectedIndex = comboBox2.SelectedIndex = 0;

            comboBox1.SelectedIndexChanged += ComboBox1SelectedValueChanged;
            comboBox2.SelectedIndexChanged += ComboBox2SelectedValueChanged;
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            textBox1.Text = fileManager.LeftDirectory.FullName;
            textBox2.Text = fileManager.RightDirectory.FullName;

            label1.Text = $"{fileManager.Drives.Disks[comboBox1.SelectedIndex].AvailableFreeSpace / Math.Pow(1024, 3): 0.00} ГБ / " +
                          $"{fileManager.Drives.Disks[comboBox1.SelectedIndex].TotalSize / Math.Pow(1024, 3): 0.00} ГБ";

            label2.Text = $"{fileManager.Drives.Disks[comboBox2.SelectedIndex].AvailableFreeSpace / Math.Pow(1024, 3): 0.00} ГБ / " +
                          $"{fileManager.Drives.Disks[comboBox2.SelectedIndex].TotalSize / Math.Pow(1024, 3): 0.00} ГБ";
        }

        private void ListViewDoubleClick(object sender, EventArgs e)
        {
            fileManager.ItemDoubleClick(sender);
            UpdateLabels();
        }

        private void LeftListViewGotFocus(object sender, EventArgs e)
        {
            fileManager.Section = Section.Left;
            Directory.SetCurrentDirectory(fileManager.LeftDirectory.FullName);
        }

        private void RightListViewGotFocus(object sender, EventArgs e)
        {
            fileManager.Section = Section.Right;
            Directory.SetCurrentDirectory(fileManager.RightDirectory.FullName);
        }

        private void ChangeViewMode(object sender, EventArgs e)
        {
            fileManager.ChangeViewMode(((sender as ToolStripMenuItem).Tag as string));
        }

        private void OpenNotepad(object sender, EventArgs e)
        {
            Process.Start("notepad.exe");
        }

        private void RefreshFiles(object sender, EventArgs e)
        {
            if (fileManager.LeftDirectory.FullName == fileManager.RightDirectory.FullName)
            {
                fileManager.SetUpListView(Section.Left);
                fileManager.SetUpListView(Section.Right);
            }
            else
            {
                fileManager.SetUpListView(fileManager.Section);
            }
        }

        private void ComboBox1SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                fileManager.ChangeDirectory(fileManager.Drives.Disks[comboBox1.SelectedIndex].Name, Section.Left);
                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBox2SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                fileManager.ChangeDirectory(fileManager.Drives.Disks[comboBox2.SelectedIndex].Name, Section.Right);
                UpdateLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateFolder(object sender, EventArgs e)
        {
            if (!(dlgBox.ShowDialog() == DialogResult.OK))
                return;

            try
            {
                fileManager.CreateFolder(dlgBox.TextBox);
                RefreshFiles(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateFile(object sender, EventArgs e)
        {
            if (!(dlgBox.ShowDialog() == DialogResult.OK))
                return;

            try
            {
                fileManager.CreateFile(dlgBox.TextBox);
                RefreshFiles(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DirectoryInfoDialogBox dirInfoDlgBox;
        private FileInfoDialogBox fileInfoDlgBox;
        private void DeleteFiles(object sender, EventArgs e)
        {
            if (!(MessageBox.Show("Вы действительно хотите удалить эти объекты?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes))
                return;

            try
            {
                fileManager.DeleteFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyFiles(object sender, EventArgs e)
        {
            fileManager.CopyFiles();
        }


        private void SearchFile(object sender, EventArgs e)
        {
            try
            {
                searchDlgBox = new SearchDialogBox(this);
                searchDlgBox.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ChangeDirectory(string newPath)
        {
            fileManager.ChangeDirectory(newPath, fileManager.Section);
        }

        private void InfoDialogBox(object sender, EventArgs e)
        {
            var tmp = fileManager.GetSelectedItem();

            if (tmp is FileInfo)
            {
                fileInfoDlgBox = new FileInfoDialogBox(tmp as FileInfo);
                fileInfoDlgBox.Show();

            }
            else if (tmp is DirectoryInfo)
            {
                dirInfoDlgBox = new DirectoryInfoDialogBox(tmp as DirectoryInfo);
                dirInfoDlgBox.Show();               
            }
        }

        private void ItemDragEvent(object sender, ItemDragEventArgs e)
        {
            var path = fileManager.GetSelectedItemsPath();
            (sender as ListView).DoDragDrop(path, DragDropEffects.Copy);
        }

        private void DragEnterEvent(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void DragDropEvent(object sender, DragEventArgs e)
        {
            string[] path;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                path = e.Data.GetData(DataFormats.FileDrop) as String[];              
            }
            else
            {
                path = e.Data.GetData(typeof(string[])) as String[];
            }

            if (!(MessageBox.Show($"Вы действительно хотите скопировать {path.Length} элементов?", "Копирование", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                return;

            fileManager.SetFilesToBuffer(path);

            if ((sender as ListView).Name == "listView1")
                fileManager.PasteFiles(Section.Left);
            else if ((sender as ListView).Name == "listView2")
                fileManager.PasteFiles(Section.Right);

            fileManager.SetUpListView(Section.Right);
            fileManager.SetUpListView(Section.Left);
        }
        private void CutFiles(object sender, EventArgs e)
        {
            fileManager.CutFiles();
        }

        private void PasteFiles(object sender, EventArgs e)
        {
            fileManager.PasteFiles(fileManager.Section);
            fileManager.SetUpListView(Section.Right);
            fileManager.SetUpListView(Section.Left);
        }
    }
}
