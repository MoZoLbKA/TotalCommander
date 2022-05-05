
using System.Collections.Generic;
using System.IO;

namespace TotalCommander
{
    public class Drives
    {
        public List<DriveInfo> Disks { get; set; }

        public Drives()
        {
            Disks = new List<DriveInfo>();
            SetDrives();
        }

        public void SetDrives()
        {
            Disks.Clear();
            foreach (var i in DriveInfo.GetDrives())
            {
                if (i.IsReady)
                {
                    Disks.Add(i);
                }
            }
        }
    }
}
