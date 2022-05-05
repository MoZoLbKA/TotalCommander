using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace TotalCommander
{
    public class FileManager
    {
        public Drives Drives { get; set; }
        public Section Section { get; set; }

        public string LeftPath { get; set; }
        public string RightPath { get; set; }

        public DirectoryInfo LeftDirectory { get; set; }
        public DirectoryInfo RightDirectory { get; set; }

        private ListView leftListView;
        private ListView rightListView;

        // icons
        private List<ImageList> leftImagelist;
        private List<ImageList> rightImagelist;

        private Buffer buffer;

        public FileManager(ListView left,ListView right, 
                           ImageList leftImg,ImageList rightImg, 
                           ImageList leftImgLarge,ImageList rightImgLarge)
        {
            buffer = new Buffer();
            Drives = new Drives();
            LeftPath = RightPath = Drives.Disks[0].Name;
            Section = Section.Left;
            Directory.SetCurrentDirectory(LeftPath);

            leftListView = left;
            rightListView = right;


            leftImagelist = new List<ImageList>(2);
            rightImagelist = new List<ImageList>(2);

            leftImagelist.Add(leftImg);
            leftImagelist.Add(leftImgLarge);

            rightImagelist.Add(rightImg);
            rightImagelist.Add(rightImgLarge);

            LeftDirectory = new DirectoryInfo(LeftPath);
            RightDirectory = new DirectoryInfo(RightPath);

            SetUpListView(Section.Left);
            SetUpListView(Section.Right);
        }

        public void SetUpListView(Section section)
        {
            var imgList   = section == Section.Left ? leftImagelist : rightImagelist;
            var listView  = section == Section.Left ? leftListView  : rightListView;
            var directory = section == Section.Left ? LeftDirectory  : RightDirectory;

            imgList[0].Images.Clear();
            imgList[1].Images.Clear();
            listView.Items.Clear();


            int tmp = 0;
            if (directory.Parent != null)
            {
                listView.Items.Add(new ListViewItem("..", tmp++) { Tag = "Folder" });
                imgList[0].Images.Add(Images.arrowUp);
                imgList[1].Images.Add(Images.arrowUp);
            }
            int index = tmp;           
            if (directory.GetDirectories().Length > 0)
            {
                imgList[0].Images.Add(Images.folder);
                imgList[1].Images.Add(Images.folder);

                foreach (var dir in directory.GetDirectories())
                {
                    listView.Items.Add(dir.Name, index);
                    listView.Items[tmp].Tag = "Folder";
                    listView.Items[tmp].SubItems.Add("");
                    listView.Items[tmp].SubItems.Add("<Папка>");
                    listView.Items[tmp++].SubItems.Add(dir.CreationTime.ToShortDateString());
                }
                index++;
            }
           
           
            foreach (var file in directory.GetFiles())
            {
                imgList[0].Images.Add(Icon.ExtractAssociatedIcon(file.FullName).ToBitmap());
                imgList[1].Images.Add(Icon.ExtractAssociatedIcon(file.FullName).ToBitmap());

                listView.Items.Add(file.Name, index++);
                listView.Items[tmp].Tag = "File";
                listView.Items[tmp].SubItems.Add(file.Extension);
                listView.Items[tmp].SubItems.Add(file.Length.ToString());
                listView.Items[tmp++].SubItems.Add(file.CreationTime.ToShortDateString());
            }
        }

        public void ChangeViewMode(string mode)
        {
            var tmp = Section == Section.Left ? leftListView : rightListView;
            tmp.View = (View)Enum.Parse(typeof(View), mode);
        }

        public void ItemDoubleClick(object sender)
        {
            var tmp = sender as ListView;
            try
            {
                if ((string)tmp.FocusedItem.Tag == "File")
                {
                    Process.Start(tmp.FocusedItem.Text);
                }
                else if ((string)tmp.FocusedItem.Tag == "Folder")
                {
                    ChangeDirectory(tmp.FocusedItem.Text, Section);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ChangeDirectory(string newPath, Section s)
        {
            var tmp = new DirectoryInfo(newPath);
            tmp.GetFiles();
            
            if (s == Section.Left)
                LeftDirectory = tmp;
            else
                RightDirectory = tmp;

            Directory.SetCurrentDirectory(newPath);
            SetUpListView(s);
        }

        public void CreateFolder(string name)
        {
            if (name.Length < 1)
                throw new ArgumentException("Слишком короткое название!");
            else if (Directory.Exists(name))
                throw new ArgumentException("Папка с таким именем уже существует!");

            Directory.CreateDirectory(name);
        }

        public void CreateFile(string name)
        {
            if (name.Length < 1)
                throw new ArgumentException("Слишком короткое название!");
            else if (File.Exists(name))
                throw new ArgumentException("Файл с таким именем уже существует!");

            FileStream fs = new FileStream(name, FileMode.CreateNew, FileAccess.Write, FileShare.Inheritable);
            fs.Close();
        }
        
        public void DeleteFiles()
        {
            var listView = Section == Section.Left ? leftListView : rightListView;
            bool samePath = RightDirectory.FullName == LeftDirectory.FullName ? true : false;

            int index;
            foreach (ListViewItem item in listView.SelectedItems)
            {
                if (item.Text == "..")
                    continue;
                else if ((item.Tag as string) == "Folder")
                    Directory.Delete(item.Text, true);
                else if ((item.Tag as string) == "File")
                    File.Delete(item.Text);

                if (samePath)
                {
                    index = leftListView.Items.IndexOf(item);
                    leftListView.Items.RemoveAt(index);
                    rightListView.Items.RemoveAt(index);
                }
                else
                    listView.Items.Remove(item);
            }
        }

        public object GetSelectedItem()
        {
            var listView = Section == Section.Left ? leftListView : rightListView;
            var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

            if (listView.SelectedItems.Count == 0)
                return null;
            var item = listView.SelectedItems[0];

            if ((item.Tag as string) == "Folder")
                return new DirectoryInfo(sourcePath + "\\" + item.Text);
            else if ((item.Tag as string) == "File")
                return new FileInfo(sourcePath + "\\" + item.Text);

            return null;
        }

        public string[] GetSelectedItemsPath()
        {
            var listView = Section == Section.Left ? leftListView : rightListView;
            var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;
            var path = new List<string>();

            foreach (ListViewItem item in listView.SelectedItems)
            {
                if (item.Text == "..")
                    continue;
                path.Add(sourcePath + "\\" + item.Text);
            }

            return path.ToArray();
        }

        public void CopyFiles()
        {
            buffer.Type = TransferType.Copy;
            SetFilesToBuffer();
        }

        public void PasteFiles(Section s)
        {
            var listView = s == Section.Left ? leftListView : rightListView;
            var sourcePath = s == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

            string dir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(sourcePath);

            if (buffer.Type == TransferType.Copy)
            {
                foreach (var item in buffer.GetFiles())
                {
                    try { File.Copy(item, Path.GetFileName(item)); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }

                foreach (var item in buffer.GetFolders())
                {
                    try { CopyFolder(new DirectoryInfo(item), Directory.GetCurrentDirectory()); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    finally { Directory.SetCurrentDirectory(sourcePath); }
                }
            }
            else
            {
                foreach (var item in buffer.GetFiles())
                {
                    try { File.Move(item, Path.GetFileName(item)); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }

                foreach (var item in buffer.GetFolders())
                {
                    try { Directory.Move(item, Path.GetFileName(item)); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
            Directory.SetCurrentDirectory(dir);
        }

        public void CutFiles()
        {
            buffer.Type = TransferType.Cut;
            SetFilesToBuffer();
        }

        public void SetFilesToBuffer(string[] path)
        {
            buffer.Clear();
            buffer.Type = TransferType.Copy;

            foreach (var item in path)
            {
                if (File.Exists(item))
                    buffer.AddFile(item);
                else if (Directory.Exists(item))
                    buffer.AddFolder(item);
            }
        }

        private void SetFilesToBuffer()
        {
            buffer.Clear();
            var listView = Section == Section.Left ? leftListView : rightListView;
            var sourcePath = Section == Section.Left ? LeftDirectory.FullName : RightDirectory.FullName;

            foreach (ListViewItem item in listView.SelectedItems)
            {
                if ((item.Tag as string) == "Folder")
                    buffer.AddFolder(sourcePath + "\\" + item.Text);
                else if ((item.Tag as string) == "File")
                    buffer.AddFile(sourcePath + "\\" + item.Text);
            }
        }

        private void CopyFolder(DirectoryInfo from, string to)
        {
            try
            {
                Directory.SetCurrentDirectory(to);
                Directory.CreateDirectory(from.Name);
                Directory.SetCurrentDirectory(from.Name);

                ArrayList current = new ArrayList();
                current.AddRange(from.GetFiles());
                current.AddRange(from.GetDirectories());

                foreach (var i in current)
                {
                    if (i is FileInfo)
                    {
                        (i as FileInfo).CopyTo($"{Directory.GetCurrentDirectory()}\\{(i as FileInfo).Name}");
                    }
                    else
                    {
                        CopyFolder(i as DirectoryInfo, Directory.GetCurrentDirectory());
                        Directory.SetCurrentDirectory("..");
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
