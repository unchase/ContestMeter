using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContestMeter.Common;
using System.IO;

namespace ContestMeter.Tests
{
    [TestClass]
    public class FTPUtilitesTests
    {

        static IFileSystem FileSystem = new LocalFileSystem();
        [TestMethod]
        public void GetFileName() {
            var util =new FtpUtilities();

            Assert.AreEqual("file.txt", util.GetFileName("file.txt"));
        
        
        }

        [TestMethod]
        public void GetFileName_Empty()
        {
            var util = new FtpUtilities();

            Assert.AreEqual("", util.GetFileName(""));


        }
        [TestMethod]
        public void GetFileName_Null()
        {
            var util = new FtpUtilities();

            Assert.AreEqual(null, util.GetFileName(null));


        }
        [TestMethod]
        public void GetFileName2()
        {
            var util = new FtpUtilities();

            Assert.AreEqual("filename", util.GetFileName("folder/filename"));


        }


        [TestMethod]
        public void GetFolferName() {
            var util = new FtpUtilities();
            Assert.AreEqual("ftp:/foldername", util.GetFolderName("foldername/filename"));

        }

        [TestMethod]
        public void GetFolferName_with_ftp()
        {
            var util = new FtpUtilities();
            Assert.AreEqual("ftp://foldername", util.GetFolderName("ftp://foldername/filename"));

        } 
        /*почему в первом случае ftp:/, а во втором - ftp://?*/
        
        [TestMethod]
        public void GetFolferName_backslash()
        {
            var util = new FtpUtilities();
            Assert.AreEqual("ftp://foldername", util.GetFolderName("ftp://foldername\\filename"));

        }

        [TestMethod]
        public void Combine() {
            var util = new FtpUtilities();
            String path = "ftp://folder";
            String file = "file";
            Assert.AreEqual("ftp://folder/file", util.Combine(path, file));
        
        }
        [TestMethod]
        public void Combine2()
        {
            var util = new FtpUtilities();
            String path = "ftp://folder/";
            String file = "file";
            Assert.AreEqual("ftp://folder/file", util.Combine(path, file));

        }

        [TestMethod]
        public void Exists_Current_Dir() {
            var util = new FtpUtilities();
            util.Exists("/");
        }
        [TestMethod]
        public void Exists_With_Slash()
        {
            FileSystem.CreateFolder("folder");
            FileStream ST = File.Create("SolutionTests.txt");
            ST.Close();
            var util = new FtpUtilities();
            util.Exists("ftp://www.nic.funet.fi/pub/unix/OpenBSD/");
            //Authority
        }
        [TestMethod]
        public void List() {
            var util = new FtpUtilities();
            util.List("ftp://nic.funet.fi");//открытый ftp
            //с чем сравнивать?
  

        }
        [TestMethod]
        public void Download_ftp() {
            var util = new FtpUtilities();
            FileStream FS = File.Create("README.txt");

            util.Download("ftp://www.nic.funet.fi/pub/unix/OpenBSD/README",FS);
            FS.Close();
        }//http://www.nic.funet.fi/pub/unix/OpenBSD/README

        [TestMethod]
        public void Combine_ftp() {
            var util = new FtpUtilities();

            Assert.AreEqual("ftp://www.nic.funet.fi/pub/unix/openbsd/readme", util.Combine("ftp://www.nic.funet.fi/pub/unix/OpenBSD", "README"));

        }



    }
}
