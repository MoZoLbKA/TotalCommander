using System;
using System.Collections.Generic;

namespace TotalCommander
{
    public enum TransferType { Copy, Cut }

    public class Buffer
    {
        private List<string> files;
        private List<string> folders;
        public TransferType Type { get; set; }

        public Buffer()
        {
            files = new List<string>();
            folders = new List<string>();
        }

        public void AddFile(string name)
        {
            files.Add(name);
        }

        public void AddFolder(string name)
        {
            folders.Add(name);
        }

        public List<string> GetFiles()
        {
            return files;
        }

        public List<string> GetFolders()
        {
            return folders;
        }

        public void Clear()
        {
            files.Clear();
            folders.Clear();
        }
    }
}
