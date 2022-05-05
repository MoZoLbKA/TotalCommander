using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TotalCommander
{
    public class Searcher
    {
        public List<string> Results { get; private set; }

        public Searcher()
        {
            Results = new List<string>();
        }

        public void Clear()
        {
            Results.Clear();
        }

        public List<string> GetResults()
        {
            return Results;
        }

        public void Search(DirectoryInfo dir, string fileName)
        {
            try
            {
                ArrayList tmpFiles = new ArrayList();
                tmpFiles.AddRange(dir.GetDirectories());
                tmpFiles.AddRange(dir.GetFiles());

                foreach (var file in tmpFiles)
                {
                    if (file is DirectoryInfo)
                    {
                        if (Regex.IsMatch((file as DirectoryInfo).Name, fileName))
                            Results.Add((file as DirectoryInfo).FullName);

                        Search(file as DirectoryInfo, fileName);
                    }
                    else
                    {
                        if (Regex.IsMatch((file as FileInfo).Name, fileName))
                            Results.Add((file as FileInfo).FullName);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
