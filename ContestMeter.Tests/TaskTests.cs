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
    public class TaskTests
    {


        [TestMethod]
        public void Task_Name()
        {
          Task task = new Task(); 
          task.Name = "somename";
          Assert.AreEqual("somename", task.Name);
        }


        [TestMethod]
        public void Task_Name_Empty()
        {
            Task task = new Task();
            task.Name = "";
            Assert.AreEqual("", task.Name);
        }

        [TestMethod]
        public void Task_Null_Name()
        {
            Task task = new Task();
            task.Name = null;
            Assert.AreEqual(null, task.Name);
        }

        static String very_long_String = new string('l', 1024);
        
        [TestMethod]    
        public void Task_long_name()
        {
            Task task = new Task();
            task.Name = very_long_String;
            Assert.AreEqual(very_long_String, task.Name);
        }

/////////////////
        [TestMethod]
        public void Task_ExecutableName()
        {
            Task task = new Task();
            task.ExecutableName = "someExecutableName";
            Assert.AreEqual("someExecutableName", task.ExecutableName);
        }

/////////////////////

        [TestMethod]
        public void Task_CheckerName()
        {
            Task task = new Task();
            task.CheckerName = "someCheckerName";
            Assert.AreEqual("someCheckerName", task.CheckerName);
        }
/////////////////////

        [TestMethod]
        public void Task_TestsFolder()
        {
            Task task = new Task();
            task.TestsFolder = "someTestsFolder";
            Assert.AreEqual("someTestsFolder", task.TestsFolder);
        }
/////////////////////

        [TestMethod]
        public void Task_TimeLimit()
        {
            Task task = new Task();
            task.TimeLimit = 0;
            Assert.AreEqual(0, task.TimeLimit);
        }



        [TestMethod]
        public void Task_TimeLimit_minus()
        {
            Task task = new Task();
            task.TimeLimit = -1;
            Assert.AreEqual(-1, task.TimeLimit);
        }

/////////////////////

        [TestMethod]
        public void Task_Weight()
        {
            Task task = new Task();
            task.Weight = 10;
            Assert.AreEqual(10, task.Weight);
        }

    }
}
