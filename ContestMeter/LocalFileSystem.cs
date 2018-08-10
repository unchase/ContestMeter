using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ContestMeter.Common
{
    public class LocalFileSystem : IFileSystem
    {
        public string Combine(string path, string file)
        {
            return Path.Combine(path, file);
        }

        public bool CreateFolder(string url)
        {
            try
            {
                Directory.CreateDirectory(url);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DeleteFile(string url)
        {
            try
            {
                File.Delete(url);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DeleteFolder(string url)
        {
            try
            {
                Directory.Delete(url);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SafeDeleteFolder(string url)
        {
            try
            {
                foreach (var file in Directory.GetFiles(url))
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                Directory.Delete(url);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Download(string source, Stream destination)
        {
            using (var sourceStream = File.OpenRead(source))
            {
                FileSystemExtensions.Copy(sourceStream, destination);
            }
        }

        public bool Exists(string url)
        {
            return File.Exists(url);
        }

        public string GetFileName(string url)
        {
            return Path.GetFileName(url);
        }

        public string GetFolderName(string url)
        {
            return Path.GetDirectoryName(url);
        }

        public IEnumerable<string> List(string url)
        {
            return Directory.GetDirectories(url).Union(Directory.GetFiles(url)).Select(x => Path.GetFileName(x));
        }

        public bool Upload(Stream source, string destination)
        {
            using (var destinationStream = File.Open(destination,FileMode.Create))
            {
                FileSystemExtensions.Copy(source, destinationStream);
            }

            return true;
        }
    }
}
