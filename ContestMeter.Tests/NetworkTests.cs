using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using ContestMeter.Common;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net.Sockets;

namespace ContestMeter.Tests
{
    [TestClass]
    [Ignore]
    public class NetworkTests
    {
        static FtpUtilities FileSystem = new Common.FtpUtilities();
        [TestMethod]
        public void Combine1()
        {
            var site = "ftp://site/address.local/10";
            var file = "file.txt";
            Assert.AreEqual( string.Format("{0}/{1}",site,file), FileSystem.Combine(site,file) );
        }

        [TestMethod]
        public void Combine2()
        {
            var site = "ftp://site/address.local/10/";
            var file = "file.txt";
            Assert.AreEqual(string.Format("{0}{1}", site, file), FileSystem.Combine(site, file));
        }

        [TestMethod]
        public void Combine3()
        {
            var site = "ftp://site/";
            var file = "file";
            Assert.AreEqual(string.Format("{0}{1}", site, file), FileSystem.Combine(site, file));
        }

        [TestMethod]
        public void GetFileName1()
        {
            var site = "ftp://site/1/2/3.txt";
            Assert.AreEqual("3.txt", FileSystem.GetFileName(site));
        }

        [TestMethod]
        public void GetFileName2()
        {
            var site = "ftp://site/1/2/3";
            Assert.AreEqual("3", FileSystem.GetFileName(site));
        }

        [TestMethod]
        public void GetFileName3()
        {
            var site = "ftp://site/1/2/3/";
            Assert.AreEqual("", FileSystem.GetFileName(site));
        }

        [TestMethod]
        public void GetFolderName1()
        {
            var site = "ftp://site/1/2/3.txt";
            Assert.AreEqual("ftp://site/1/2", FileSystem.GetFolderName(site));
        }

        [TestMethod]
        public void GetFolderName2()
        {
            var site = "ftp://site/1/2/3";
            Assert.AreEqual("ftp://site/1/2", FileSystem.GetFolderName(site));
        }

        [TestMethod]
        public void GetFolderName3()
        {
            var site = "ftp://site/1/2/3/";
            Assert.AreEqual("ftp://site/1/2/3", FileSystem.GetFolderName(site));
        }

        [TestMethod]
        public void GetIP()
        {
            var addr = FtpUtilities.GetLocalIP4();
            Assert.AreEqual(AddressFamily.InterNetwork, addr.AddressFamily);
        }


        [TestMethod]
        public void CheckFTPFolderExists()
        {
            var creds = new NetworkCredential("olympics", "olympics");
            var url = "ftp://localhost/";
            Assert.IsTrue(FileSystem.Exists(FileSystem.Combine(url, "10.0.0.1")));
            Assert.IsTrue(FileSystem.Exists(FileSystem.Combine(url, "10.0.0.2")));
            Assert.IsTrue(FileSystem.Exists(FileSystem.Combine(url, "10.0.0.3")));
            Assert.IsFalse(FileSystem.Exists(FileSystem.Combine(url, "10.0.0.4")));
            Assert.IsFalse(FileSystem.Exists(FileSystem.Combine(url, "10.0.0.5")));
        }

        [TestMethod]
        public void CreateDeleteFtpFolder()
        {
            var url = "ftp://localhost/10.1.0.1";

            Assert.IsTrue(FileSystem.CreateFolder(url));
            Assert.IsTrue(FileSystem.Exists(FileSystem.Combine("ftp://localhost", "10.1.0.1")));

            Assert.IsTrue(FileSystem.DeleteFolder(url));

            Assert.IsFalse(FileSystem.Exists(FileSystem.Combine("ftp://localhost", "10.1.0.1")));

        }

        [TestMethod]
        public void Exists1()
        {
            Assert.IsTrue(FileSystem.Exists("ftp://localhost/unit-tests"));
        }

        [TestMethod]
        public void Exists2()
        {
            Assert.IsTrue(FileSystem.Exists("ftp://localhost/unit-tests/"));
        }

        [TestMethod]
        public void Exists3()
        {
            Assert.IsFalse(FileSystem.Exists("ftp://localhost/unit-tests-never-created"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ExistsNull()
        {
            FileSystem.Exists(null);
        }

        [TestMethod]
        public void RootExists()
        {
            Assert.IsTrue(FileSystem.Exists("/"));
        }

        [TestMethod]
        public void ListFolder()
        {
            var creds = new NetworkCredential("olympics", "olympics");
            var url = "ftp://localhost/10.7.0.1";

            var list = FileSystem.List(url);

            Assert.IsTrue(list.Contains("magic"));
            Assert.IsTrue(list.Contains("numbers"));
        }

        [TestMethod]
        public void DownloadFile()
        {
            var url = "ftp://localhost/10.2.0.1";
            var fileName = "single-line.txt";

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localDir = Path.Combine(appData, "Tester");
            var saveAs = Path.Combine(localDir, fileName);

            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }
            if (!File.Exists(saveAs))
            {
                File.Delete(saveAs);
            }

            var filePath = string.Format("{0}/{1}", url, fileName);

            FileSystem.Download(filePath, saveAs);

            Assert.IsTrue(File.Exists(saveAs));
            Assert.AreEqual(14, new FileInfo(saveAs).Length);
        }

        [TestMethod]
        public void UploadFile()
        {

            var url = "ftp://localhost/10.2.0.1";
            var fileName = "upload.txt";

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localDir = Path.Combine(appData, "Tester");
            var localFile = Path.Combine(localDir, fileName);

            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            File.WriteAllLines(localFile, new[] { "magic number" });


            Assert.IsTrue(FileSystem.Upload(localFile, FileSystem.Combine(url, fileName)));
        }

        [TestMethod]
        public void LoadJson()
        {
            var url = "ftp://localhost/10.3.0.1/mock.json";
            var result = FileSystem.LoadJson<JsonMock>(url);
            Assert.AreEqual("Test", result.Name);
            Assert.AreEqual(2, result.Values.Length);
            Assert.IsTrue(result.Values.Contains("magic"));
            Assert.IsTrue(result.Values.Contains("numbers"));
        }

        [DataContract]
        class JsonMock
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string[] Values { get; set; }
        }

        [TestMethod]
        public void SaveJson()
        {
            var url = "ftp://localhost/10.3.0.1/mock.json";
            var mock = new JsonMock { Name = "Test", Values = new[] { "magic", "numbers" } };

            Assert.IsTrue(FileSystem.SaveJson(url, mock));
        }

        [TestMethod]
        public void UploadNotExists()
        {
            string uploadFileName = "solution-upload";
            string uploadUrl = "ftp://localhost/unit-tests/solutionTests/upload";

            var exeName = uploadFileName + "-not-exist";
            var config = Configuration.Load(FileSystem, uploadUrl,"");

            var folder = string.Format("{0}/{1}/{2}", config.Site, FtpUtilities.GetLocalIP4(), exeName);
            var fileName = string.Format("{0}.txt", uploadFileName);
            var fullFileName = string.Format("{0}/{1}", folder, fileName);

            if (FileSystem.Exists(FileSystem.Combine(folder, fileName)))
            {
                FileSystem.DeleteFile(fullFileName);
                FileSystem.DeleteFolder(folder);
            }

            var unit = new Solution
            {
                LocalSourcePath = uploadFileName + ".txt",
                Task = new Task
                {
                    ExecutableName = exeName,
                },
                Configuration = config,
            };

            unit.Upload();

            Assert.IsTrue(FileSystem.Exists(FileSystem.Combine(folder, fileName)));
        }


    }

}
