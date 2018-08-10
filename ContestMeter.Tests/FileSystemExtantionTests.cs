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
    public class FileSystemExtantionTests
    { 
        
        [ClassInitialize]
        public static void Files(TestContext context)
        { 
             FileStream source = File.Create("source_upload.txt");
            source.Close();
            //FileStream dest = File.Create("dest_upload.txt");
            //dest.Close();       
           
        }

        static String very_long_String = new string('l', 1024);
        static IFileSystem FileSystem = new LocalFileSystem();
        
       
        [TestMethod]
        public void Upload() {

            FileSystemExtensions.Upload(FileSystem, "source_upload.txt", "dest_upload.txt");
        }

      
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Upload_Null()
        {

            FileSystemExtensions.Upload(null, "source_upload.txt", "dest_upload.txt");
        }



        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Upload_Not_Found()
        {
             FileSystemExtensions.Upload(FileSystem, "imaginary_source.txt", "dest_upload.txt");
        }



       
        
        [TestMethod]
        [ExpectedException(typeof(PathTooLongException))]
        public void Upload_Path_Too_Long()
        {
            
            FileSystemExtensions.Upload(FileSystem, very_long_String, "dest_upload.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Upload_Empty_Path()
        {

            FileSystemExtensions.Upload(FileSystem, "", "dest_upload.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Upload_Bad_Name()
        {

            FileSystemExtensions.Upload(FileSystem, "*sdfaj.d.txt", "dest_upload.txt");
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void Upload_IOException()
        {
            FileStream source = File.Create("source_upload.txt");

            FileSystemExtensions.Upload(FileSystem, "source_upload.txt", "dest_upload.txt");
            source.Close();
        }



        [TestMethod]
        [ExpectedException(typeof(PathTooLongException))]
        public void Upload_Path_Too_Long_sec_arg()
        {

            FileSystemExtensions.Upload(FileSystem, "source_upload.txt", very_long_String);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Upload_Empty_Path_sec_arg()
        {

            FileSystemExtensions.Upload(FileSystem, "source_upload.txt", "");
        }

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void Upload_IOException_sec_arg()
        {
            FileStream dest = File.Create("dest_upload.txt");

            FileSystemExtensions.Upload(FileSystem, "source_upload.txt", "dest_upload.txt");
            dest.Close();
        }



        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Upload_Bad_Name_sec_arg()
        {

            FileSystemExtensions.Upload(FileSystem, "source_upload.txt", "*:.txt");
        }

        [TestMethod]
        //Здесь должны быть тесты для Copy с нормальными потоками
        public void Copy() {
            Stream dest = File.Create("Copy_dest.txt");
            Stream source = File.Create("Copy_source.txt");
            FileSystemExtensions.Copy(source, dest);
            source.Close();
            dest.Close();
         }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Copy_Not_Readable() {
            Stream source = Stream.Null;
            Stream dest = File.Create("Copy_dest.txt");
            dest.Close();              
            FileSystemExtensions.Copy(source, dest);
         }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Copy_Not_Writable()
        {

            Stream source = File.Create("Copy_source.txt");
            source.Close();
            Stream dest = File.Create("Copy_dest.txt");
            dest.Close();
            FileSystemExtensions.Copy(source, dest);
        }
    }
}
