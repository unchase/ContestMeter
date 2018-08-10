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
    public class ConfigurationTests
    {
        static IFileSystem FileSystem = new LocalFileSystem();

        [TestMethod]
        public void SaveConfiguration()
        {
            var url = "TestFS/SaveConfiguration";
            Directory.CreateDirectory(url);

            var config = Configuration.Load(FileSystem, url, "");
            
            config.Save();

            var configUrl = "TestFS/SaveConfiguration/config.json";


            Assert.IsTrue(File.Exists(configUrl));
            Assert.AreNotEqual(0, new FileInfo(configUrl).Length);
        }

        [TestMethod]
        public void LoadConfiguration()
        {
            var url = "TestFS/LoadConfiguration";
            Directory.CreateDirectory(url);
            File.Copy("LoadConfiguration.json", "TestFS/LoadConfiguration/config.json");

            var config = Configuration.Load(FileSystem, url, "");

            Assert.AreEqual("magic numbers", config.Tasks.First().Name);
            Assert.AreEqual(url, config.Site);

        }

        //[TestMethod]
        //public void SavePartInfo()
        //{
        //    var url = "ftp://localhost/10.5.0.1";
        //    var ip = FtpUtilities.GetLocalIP4().ToString();
        //    var homeDir = string.Format("{0}/{1}", url, ip);

        //    var vm = new ChangeNameViewModel();
        //    var config = Configuration.Load(url);
        //    vm.Configuration = config;

        //    vm.Participant = Participant.Load(url);

        //    vm.Participant.LastName = "Иванов";
        //    vm.Participant.FirstName = "Ваня";
        //    vm.Participant.HomeTown = "LA";
        //    vm.Participant.School = "#5";
        //    vm.Participant.Grade = 9;

        //    vm.Save();

        //    Assert.IsTrue(FileSystem.Exists(homeDir));
        //    Assert.IsTrue(FileSystem.Exists(FileSystem.Combine(homeDir, "info.json")));
        //}

        //[TestMethod]
        //public void LoadPartInfo()
        //{
        //    var url = "ftp://localhost/10.5.0.1";
        //    var ip = FtpUtilities.GetLocalIP4().ToString();
        //    var homeDir = string.Format("{0}/{1}", url, ip);

        //    var vm = new ChangeNameViewModel();
        //    var config = Configuration.Load(url);

        //    var part = Participant.Load(url);

        //    Assert.AreEqual("Ваня", part.FirstName);
        //    Assert.AreEqual("Иванов", part.LastName);

        //}

        [TestMethod]
        public void AddNewTool()
        {
            var unit = Configuration.Load(new LocalFileSystem(), "", "");
            var tool = unit.AddNewTool();
            Assert.IsNotNull(tool);
            Assert.IsTrue(unit.DevTools.Contains(tool));
        }

        [TestMethod]
        public void RemoveTool()
        {
            var unit = Configuration.Load(new LocalFileSystem(), "", "");
            var tool = unit.AddNewTool();
            Assert.IsTrue(unit.DevTools.Contains(tool));

            unit.RemoveTool(tool);
            Assert.IsFalse(unit.DevTools.Contains(tool));

        }

        [TestMethod]
        public void RemoveTask()
        {
            var unit = Configuration.Load(new LocalFileSystem(), "", "");
            var task = unit.AddNewTask();
            Assert.IsTrue(unit.Tasks.Contains(task));

            unit.RemoveTask(task);
            Assert.IsFalse(unit.Tasks.Contains(task));

        }

        [TestMethod]
        [Ignore]
        public void GetInfoPath()
        {
            var site = "";
            var expected = string.Format("{1}\\info.json", site, FtpUtilities.GetLocalIP4());


            var unit = Configuration.Load(new LocalFileSystem(), site, "");

            Assert.AreEqual(expected, unit.InfoPath);
        }
    }
}
