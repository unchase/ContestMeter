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
    public class SolutionTests
    {
        static IFileSystem FileSystem = new LocalFileSystem();
        [ClassInitialize]
        public static void Files(TestContext context)
        {
            FileStream ST = File.Create("SolutionTests.txt");
            ST.Close();


        }


        [TestMethod]
        public void Compile()
        {

            String url = "TestFS/CheckerTests/Compile";
            var csc = new DeveloperTool
            {
                Name = "CSC",
                CompileCommand = "C:\\Windows\\Microsoft.NET\\Framework\\v3.5\\csc.exe",
                CommandArgs = "\"{0}\"",
            };
            var solution = new Solution()
            {
                Task = new Task
                {
                    Name = "checkertests-compile",
                    ExecutableName = "checkertests-compile",

                    CheckerName = "check.exe"
                },
                LocalSourcePath = "checkertests-compile.cs",
                DevTool = csc,
            };
            Directory.CreateDirectory(url);



            // FileSystem.CreateFolder("folder");
            solution.Compile(url, url);

            //Assert.IsTrue(File.Exists(Path.Combine(url, "CheckerTests-Compile.exe")));
        }




        [TestMethod]
        public void RunAccepted()
        {
            var task = "CheckerTests-RunAccepted";

            var solution = CreateTestSolution(task);
            solution.Check(task, "");

            Assert.AreEqual(1, solution.TestsCount);
            Assert.AreEqual(1, solution.Score);
        }

        private static Solution CreateTestSolution(string task)
        {
            //Directory.CreateDirectory(task);

            //File.Copy(task + ".cs", Path.Combine(task, task + ".cs"));
            //File.Copy(task + "-checker.exe", Path.Combine(task, task + "-checker.exe"));

            var csc = new DeveloperTool
            {
                Name = "CSC",
                CompileCommand = "C:\\Windows\\Microsoft.NET\\Framework\\v3.5\\csc.exe",
                CommandArgs = "\"{0}\"",
            };

            var solution = new Solution(FileSystem)
            {
                Task = new Task
                {
                    Name = task,
                    ExecutableName = task,

                    CheckerName = task + "-checker.exe",
                    TimeLimit = 3,
                    Weight = 1,
                },
                LocalSourcePath = task + ".cs",
                DevTool = csc,
                Configuration = Configuration.Load(FileSystem, "",""),
                Path = task + ".exe",
            };

            return solution;
        }

        [TestMethod]
        public void RunFailed()
        {
            var task = "CheckerTests-RunFailed";


            var solution = CreateTestSolution(task);
            solution.Check(task, "");

            Assert.AreEqual(1, solution.TestsCount);
            Assert.AreEqual(0, solution.Score);

        }

        [TestMethod]
        public void RunNoncompilable()
        {
            var url = "";
            var csc = new DeveloperTool
            {
                Name = "CSC",
                CompileCommand = "C:\\Windows\\Microsoft.NET\\Framework\\v3.5\\csc.exe",
                CommandArgs = "\"{0}\"",
            };

            var solution = new Solution
            {
                Task = new Task
                {
                    Name = "noncomp",
                    ExecutableName = "noncomp",

                    CheckerName = "noncomp-checker.exe",
                    TimeLimit = 2,
                    Weight = 2,
                },
                LocalSourcePath = "noncomp.cs",
                DevTool = csc,
            };

            var configuration = Configuration.Load(FileSystem, "TestFS","");
            solution.Configuration = configuration;
            solution.Check(url, "");


            Assert.AreEqual(10, solution.TestsCount);
            Assert.AreEqual(0, solution.Score);
        }

        [TestMethod]
        public void RunHang()
        {
            var csc = new DeveloperTool
            {
                Name = "CSC",
                CompileCommand = "C:\\Windows\\Microsoft.NET\\Framework\\v3.5\\csc.exe",
                CommandArgs = "\"{0}\"",
            };
            var solution = new Solution()
            {
                Task = new Task
                {
                    Name = "hang",
                    ExecutableName = "hang",

                    CheckerName = "check.exe"
                },
                LocalSourcePath = "hang.cs",
                DevTool = csc,
            };

            solution.Check("", "");


            Assert.IsTrue(File.Exists(Path.Combine("", "test.exe")));
            Assert.AreEqual(1, solution.FailedRuns);
            Assert.AreEqual(0, solution.FailedChecks);
            Assert.AreEqual(1, solution.TestsCount);
        }








        [TestMethod]
        public void Solution_FailedChecks()
        {

            var solution = new Solution();
            solution.FailedChecks = 10;
            Assert.AreEqual(10, solution.FailedChecks);


        }
        [TestMethod]
        public void Solution_WeightedScore()
        {

            var solution = new Solution()
            {
                Task = new Task
                {
                    Weight = 10,
                }
            };
            solution.Score = 10;
            Assert.AreEqual(100, solution.WeightedScore);
        }

        [TestMethod]
        public void Solution_Name()
        {

            var solution = new Solution()
            {
                Task = new Task
                {
                    Name = "name",
                }
            };
            Assert.AreEqual("name", solution.Name);
        }


        [TestMethod]
        public void Solution_TestsFolder()
        {

            var solution = new Solution()
            {
                Task = new Task
                {
                    TestsFolder = "folder",
                }
            };
            Assert.AreEqual("folder", solution.TestsFolder);
        }
        [TestMethod]
        public void Run_Long()
        {
            var csc = new DeveloperTool
            {
                Name = "CSC",
                CompileCommand = "C:\\Windows\\Microsoft.NET\\Framework\\v3.5\\csc.exe",
                CommandArgs = "\"{0}\"",
            };

            var solution = new Solution()
            {
                Task = new Task
                {
                    Name = "wait",
                    ExecutableName = "wait",

                    CheckerName = "check.exe"
                },
                LocalSourcePath = "hang.cs",
                DevTool = csc,
            };

            //var configuration = Configuration.Load(FileSystem, "c:/");
            //solution.Configuration = configuration;


            solution.Check("c:/", "c:/");


        }
        //in Participant

    }
}
