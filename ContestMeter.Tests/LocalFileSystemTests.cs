using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContestMeter.Common;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace ContestMeter.Tests
{


    [TestClass]
    public class LocalFileSystemTests
    {
        [ClassInitialize]
        public static void Files(TestContext context)
        {
            FileStream source = File.Create("LFS_Tests.txt");
            source.Close();
            Directory.CreateDirectory("folder");

        }

        static IFileSystem FileSystem = new LocalFileSystem();
        static String very_long_String = new string('/', 1024);

        [TestMethod]
        public void GetFileName1()
        {
            Assert.AreEqual("LFS_Tests.txt", FileSystem.GetFileName("LFS_Tests.txt"));
        }
        [TestMethod]
        public void GetFileName2()
        {
            Assert.AreEqual("LFS_Tests.txt", FileSystem.GetFileName("1/LFS_Tests.txt"));
        }

        [TestMethod]
        public void GetFileName3()
        {
            Assert.AreEqual("LFS_Tests.txt", FileSystem.GetFileName("1//LFS_Tests.txt"));
        }

        [TestMethod]
        public void GetFileName_Too_Long()
        {
            Assert.AreEqual("LFS_Tests.txt", FileSystem.GetFileName(very_long_String + "LFS_Tests.txt"));
        }


/////////////


        [TestMethod]
        public void GetFolderName1()
        {
            Assert.AreEqual("folder", FileSystem.GetFolderName("folder/LFS_Tests.txt"));
        }
        [TestMethod]
        public void GetFolderName2()
        {
            Assert.AreEqual("folder", FileSystem.GetFolderName("folder/ /LFS_Tests.txt"));
        }


        [TestMethod]
        public void GetFolderName4()
        {
            Assert.AreEqual("folder", FileSystem.GetFolderName("folder/ "));
        }

        [TestMethod]
        public void GetFolderName_Too_Long()
        {
            Assert.AreEqual("folder", FileSystem.GetFolderName("folder"+very_long_String + "LFS_Tests.txt"));
        }

        [TestMethod]
        public void Exists()
        {
            Assert.IsTrue(FileSystem.Exists("LFS_Tests.txt"));
        }

        [TestMethod]
        public void Exists_Not_Exists()
        {
            Assert.IsFalse(FileSystem.Exists("/LFS_Tests.txt"));
        }

        [TestMethod]
        public void Exists_Bad_Name()
        {
            Assert.IsFalse(FileSystem.Exists("*.txt"));
        }

        [TestMethod]
        public void Exists_Empty_Name()
        {
            Assert.IsFalse(FileSystem.Exists(""));
        }

////////////////////////////
        [TestMethod]
        public void CreateFolder()
        {
            Assert.IsTrue(FileSystem.CreateFolder("newfolder"));
        }
        
        [TestMethod]
        public void CreateFolder_Empty_Name()
        {
            Assert.IsFalse(FileSystem.CreateFolder(""));
        }
        
        [TestMethod]
        public void CreateFolder_Bad_Name()
        {
            Assert.IsFalse(FileSystem.CreateFolder("*"));
        }

        [TestMethod]
        public void CreateFolder_Long_Name()
        {
            Assert.IsFalse(FileSystem.CreateFolder(very_long_String));
        }
////////////////////////

        [TestMethod]
        public void DeleteFile()
        {
            FileStream source = File.Create("delete.txt");
            source.Close();
            Assert.IsTrue(FileSystem.DeleteFile("delete.txt"));
        }

        [TestMethod]
        public void DeleteFile_Bad_Mame()
        {
            Assert.IsFalse(FileSystem.DeleteFile("*.txt"));
        }

        [TestMethod]
        public void DeleteFile_Busy()
        {
            FileStream Dfile = File.Create("Dfile.txt");
           
            Assert.IsFalse(FileSystem.DeleteFile("Dfile.txt"));
            Dfile.Close();
        }
/////////////////
        [TestMethod]
        public void DeleteFolder()
        {

            Assert.IsTrue(FileSystem.DeleteFolder("folder"));

        }

        [TestMethod]
        public void DeleteFolder_Empty_Name()
        {

            Assert.IsFalse(FileSystem.DeleteFolder(""));

        }
        [TestMethod]
        public void DeleteFolder_Upper()
        {

            Assert.IsFalse(FileSystem.DeleteFolder("../"));

        }

    }
}
