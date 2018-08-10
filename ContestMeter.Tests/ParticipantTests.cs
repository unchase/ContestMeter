using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContestMeter.Common;
using System.IO;
using System.Collections.ObjectModel;


namespace ContestMeter.Tests
{
    [TestClass]
    public class ParticipantTests
    {
        public static LocalFileSystem FileSystem = new LocalFileSystem();
        const string site = "ParticipantTests";
        public static Participant prt = new Participant(FileSystem, site);

        [TestMethod]
        public void SaveNew()
        {
            if (Directory.Exists(site))
            {
                Directory.Delete(site, true);
            }
            var unit = new Participant(FileSystem, site);
            unit.Save(site);

            Assert.IsTrue(File.Exists(Path.Combine(site, "info.json")));
        }

        [TestMethod]
        public void LoadAbsent()
        {
            if (Directory.Exists(site))
            {
                Directory.Delete(site, true);
            }

            var unit = Participant.Load(FileSystem, site);
            Assert.IsNotNull(unit);
        }

        [TestMethod]
        public void IsValid() {
            var prt = new Participant(FileSystem,site);
            prt.LastName = "lastname";
            prt.School = "someschool";
            prt.FirstName = "firstname";
            prt.Grade = 10;
            Assert.IsTrue(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Bad_FirstName()
        {
            var prt = new Participant(FileSystem, site);
            prt.FirstName = ""; 
            prt.LastName = "lastname";
            prt.School = "someschool";

            prt.Grade = 10;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Null_FirstName()
        {
            prt.FirstName = null;
            prt.LastName = "lastname";
            prt.School = "someschool";

            prt.Grade = 10;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Bad_LastName()
        {
            prt.FirstName = "firstname";
            prt.LastName = "";
            prt.School = "someschool";

            prt.Grade = 10;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Null_LastName()
        {
            prt.FirstName = "firstname";
            prt.LastName = null;
            prt.School = "someschool";

            prt.Grade = 10;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Bad_School()
        {
            prt.FirstName = "firstname";
            prt.LastName = "lastname";
            prt.School = "";

            prt.Grade = 10;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Null_School()
        {
            prt.FirstName = "firstname";
            prt.LastName = "lastname";
            prt.School = null;

            prt.Grade = 10;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Bad_Grade()
        {
            prt.FirstName = "firstname";
            prt.LastName = "lastname";
            prt.School = "someschool";

            prt.Grade = -10;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void IsValid_Bad_Grade2()
        {
            prt.FirstName = null;
            prt.LastName = "lastname";
            prt.School = "someschool";

            prt.Grade = 100;
            Assert.IsFalse(prt.IsValid);
        }

        [TestMethod]
        public void Participant_Create() {
            var prt_default = new Participant();
            /*Какая должна быть проверка?*/
        
        }
        [TestMethod]
        public void Solution_Collection()
        {
            var collection =new ObservableCollection<Solution>();
            var solution = new Solution();
            collection = prt.Solutions;
         }
        [TestMethod]
        public void HomeTown() {
            prt.HomeTown = "somecity";
            Assert.AreEqual("somecity", prt.HomeTown);
       }

        [TestMethod]
        public void MiddleName()
        {
            prt.MiddleName = "middlename";
            Assert.AreEqual("middlename", prt.MiddleName);
        }

        [TestMethod]
        public void Load()
        {
            prt.MiddleName = "middlename";
            Assert.AreEqual("middlename", prt.MiddleName);
        }
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Discover()
        {
            //var configuration = Configuration.Load(FileSystem, "c://");
            var participant = new Participant();
            //participant.Discover(configuration);
        }

        [TestMethod]
        public void Load2()
        {
            var participant = new Participant();
            FileSystem.CreateFolder("loadfolder");
            FileStream info = File.Create("loadfolder/info.json");
            info.Close();
            var someone = Participant.Load(FileSystem, "loadfolder");

        }

    }
}
