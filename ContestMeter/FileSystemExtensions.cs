using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace ContestMeter
{
    public static class FileSystemExtensions
    {
        public static T LoadJson<T>(this IFileSystem fs, string url)
        {
            T result = default(T);
            using (var ms = new MemoryStream())
            {
                fs.Download(url, ms);

                ms.Flush();
                ms.Position = 0;

                result = (T)new DataContractSerializer(typeof(T)).ReadObject(ms);
            }

            return result;
        }

        public static void Download(this IFileSystem fs, string source, string destination)
        {
            using (var file = File.Open(destination, FileMode.Create))
            {
                fs.Download(source, file);
            }
        }

        public static bool Upload(this IFileSystem fs, string source, string destination)
        {
            using (var sourceStream = File.OpenRead(source))
            {
                return fs.Upload(sourceStream, destination);
            }
        }

        public static bool SaveJson<T>(this IFileSystem fs, string url, T mock)
        {
            using (var ms = new MemoryStream())
            {
                var jSer = new DataContractSerializer(typeof(T));
                jSer.WriteObject(ms, mock);
                ms.Flush();
                ms.Position = 0;

                return fs.Upload(ms, url);
            }
        }

        public static void Copy(Stream source, Stream destination)
        {
            if (!source.CanRead)
            {
                throw new ArgumentException("SourceFileName stream should be readable.");
            }
            if (!destination.CanWrite)
            {
                throw new ArgumentException("Destination stream should be writable.");
            }

            int len = (int)1024 * 1024;
            var buffer = new byte[len];

            while (len > 0)
            {
                len = source.Read(buffer, 0, len);
                if (len > 0)
                {
                    destination.Write(buffer, 0, len);
                }
            }
        }
    }
}
