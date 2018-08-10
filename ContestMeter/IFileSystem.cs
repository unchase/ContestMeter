using System;
using System.IO;
using System.Collections.Generic;
namespace ContestMeter
{
    public interface IFileSystem
    {
        string Combine(string path, string file);
        string GetFileName(string url);
        string GetFolderName(string url);
        bool Exists(string url);
        IEnumerable<string> List(string url);

        bool CreateFolder(string url);
        bool DeleteFolder(string url);
        bool DeleteFile(string url);

        void Download(string source, Stream destination);

        bool Upload(Stream source, string destination);
    }
}
