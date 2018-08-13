using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;

namespace ContestMeter.Common
{
    public class FtpUtilities : IFileSystem
    {
        private const string User = @"olympics";
        //private const string User = @"WIN-8I8OQT9881E\olympics";
        private const string Pwd = "olympics";
        private static NetworkCredential Credentials
        {
            get
            {
                return new NetworkCredential(User, Pwd);
            }
        }

        public static IPAddress GetLocalIP4()
        {
            var host = Dns.GetHostEntry(String.Empty);
            return host.AddressList.Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                               && x != IPAddress.Any && x != IPAddress.Broadcast &&
                                               x != IPAddress.Loopback && !IPAddress.IsLoopback(x)).FirstOrDefault();
        }

        public bool Upload(Stream source, string destination)
        {
            // Get the object used to communicate with the server.
            var request = (FtpWebRequest)WebRequest.Create(destination);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = Credentials;

            // Copy the contents of the file to the request stream.
            request.ContentLength = source.Length;
            using (var requestStream = request.GetRequestStream())
            {
                FileSystemExtensions.Copy(source, requestStream);
            }

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                return FtpStatusCode.ClosingData == response.StatusCode;
            }
        }

        public void Download(string source, Stream destination)
        {
            var request2 = (FtpWebRequest)WebRequest.Create(source);
            request2.Method = WebRequestMethods.Ftp.DownloadFile;
            request2.Credentials = Credentials;

            using (var response = (FtpWebResponse)request2.GetResponse())
            {
                var stream = response.GetResponseStream();
                FileSystemExtensions.Copy(stream, destination);
            }
        }

        public bool DeleteFolder(string url)
        {
            var request2 = (FtpWebRequest)WebRequest.Create(url);
            request2.Method = WebRequestMethods.Ftp.RemoveDirectory;
            request2.Credentials = Credentials;

            using (var response = (FtpWebResponse)request2.GetResponse())
            {
                return FtpStatusCode.FileActionOK == response.StatusCode;
            }
        }

        public bool DeleteFile(string url)
        {
            var request2 = (FtpWebRequest)WebRequest.Create(url);
            request2.Method = WebRequestMethods.Ftp.DeleteFile;
            request2.Credentials = Credentials;

            using (var response = (FtpWebResponse)request2.GetResponse())
            {
                return FtpStatusCode.FileActionOK == response.StatusCode;
            }
        }

        public bool CreateFolder(string url)
        {
            // Get the object used to communicate with the server.
            var request1 = (FtpWebRequest)WebRequest.Create(url);
            request1.Method = WebRequestMethods.Ftp.MakeDirectory;

            // This example assumes the FTP site uses anonymous logon.
            request1.Credentials = Credentials;

            using (var response = (FtpWebResponse)request1.GetResponse())
            {
                return FtpStatusCode.PathnameCreated == response.StatusCode;
            }
        }

        public IEnumerable<string> List(string url)
        {
            // Get the object used to communicate with the server.
            var request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = Credentials;

            var result = new List<string>();

            using (var response = (FtpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                while (!reader.EndOfStream)
                {
                    result.Add(reader.ReadLine().Trim());
                }
            }

            return result;
        }

        public bool Exists(string url)
        {
            Check.NotNull(url, "url");

            if (url == "/")
            {
                return true;
            }

            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            var folder = GetFolderName(url);
            var file = GetFileName(url);

            return Exists(folder, file);
        }

        private bool Exists(string url, string file)
        {
            return List(url).Count(x => x.ToLowerInvariant() == file.ToLowerInvariant()) > 0;
        }

        public string Combine(string path, string file)
        {
            if (!path.EndsWith("/"))
            {
                path += "/";
            }
            return new Uri(new Uri(path), file).ToString().ToLowerInvariant();
        }

        public string GetFileName(string url)
        {
            return Path.GetFileName(url);
        }

        public string GetFolderName(string url)
        {
            if (url.StartsWith("ftp://"))
            {
                url = url.Substring(5);
            }
            return "ftp:/" + Path.GetDirectoryName(url).Replace('\\', '/');
        }
    }
}
