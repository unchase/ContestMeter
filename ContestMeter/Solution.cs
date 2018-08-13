using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Threading;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Security;
using ThreadState = System.Threading.ThreadState;

namespace ContestMeter.Common
{
    [DataContract]
    public class Solution : NotifyPropertyChanged//: Task
    {
        public IFileSystem FileSystem { get; set; }
        public const string TaskPropertyName = "Task";
        private Task _task;

        public Solution()
            : this(new FtpUtilities())
        {
        }

        public Solution(IFileSystem fileSystem)
        {
            ContestMeter.Check.NotNull(fileSystem, "fileSystem");
            FileSystem = fileSystem;
        }

        [DataMember]
        public Task Task
        {
            get { return _task; }
            set
            {
                if (_task != value)
                {
                    _task = value;
                    OnPropertyChanged(TaskPropertyName);
                    OnPropertyChanged("ExecutableName");
                    OnPropertyChanged("Weight");
                    OnPropertyChanged("Name");
                    OnPropertyChanged("CheckerName");
                    OnPropertyChanged("TimeLimit");
                    OnPropertyChanged("MaxSourceSize");
                    OnPropertyChanged("MaxMemorySize");
                    OnPropertyChanged("TestsFolder");
                }
            }
        }

        public string ExecutableName
        {
            get { return Task.ExecutableName; }
        }

        public int Weight
        {
            get { return Task.Weight; }
        }

        public string Name
        {
            get { return Task.Name; }
        }

        public string CheckerName
        {
            get { return Task.CheckerName; }
        }

        public int TimeLimit
        {
            get { return Task.TimeLimit; }
        }

        public int MaxSourceSize
        {
            get { return Task.MaxSourceSize; }
        }

        public int MaxMemorySize
        {
            get { return Task.MaxMemorySize; }
        }

        public string TestsFolder
        {
            get { return Task.TestsFolder; }
        }

        public string InputFileName
        {
            get { return ExecutableName + ".in"; }
        }

        public string OutputFileName
        {
            get { return ExecutableName + ".out"; }
        }

        public string AnswerFileName
        {
            get { return ExecutableName + ".a"; }
        }

        const string PathPropertyName = "Path";
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged(PathPropertyName);
                }
            }
        }

        public const string DevToolPropertyName = "DevTool";
        private DeveloperTool _devTool;
        [DataMember]
        public DeveloperTool DevTool
        {
            get { return _devTool; }
            set
            {
                if (_devTool != value)
                {
                    _devTool = value;
                    OnPropertyChanged(DevToolPropertyName);
                }
            }
        }

        const string ScorePropertyName = "Score";
        private int _score;
        public int Score
        {
            get { return _score; }
            set
            {
                if (_score != value)
                {
                    _score = value;
                    OnPropertyChanged(ScorePropertyName);
                    OnPropertyChanged("WeightedScore");
                }
            }
        }

        public int WeightedScore
        {
            get { return Score * Weight; }
        }

        const string TestsCountPropertyName = "TestsCount";
        private int _testsCount;
        public int TestsCount
        {
            get { return _testsCount; }
            set
            {
                if (_testsCount != value)
                {
                    _testsCount = value;
                    OnPropertyChanged(TestsCountPropertyName);
                }
            }
        }

        const string FailedChecksPropertyName = "FailedChecks";
        private int _failedChecks;
        public int FailedChecks
        {
            get { return _failedChecks; }
            set
            {
                if (_failedChecks != value)
                {
                    _failedChecks = value;
                    OnPropertyChanged(FailedChecksPropertyName);
                }
            }
        }

        const string FailedRunsPropertyName = "FailedRuns";
        private int _failedRuns;
        public int FailedRuns
        {
            get { return _failedRuns; }
            set
            {
                if (_failedRuns != value)
                {
                    _failedRuns = value;
                    OnPropertyChanged(FailedRunsPropertyName);
                }
            }
        }

        /// <summary>
        /// Имя файла с исходным кодом решения
        /// </summary>
        public string SourceFileName
        {
            get { return System.IO.Path.GetFileName(LocalSourcePath); }
        }

        public IConfiguration Configuration { get; set; }
        public Participant Participant { get; set; }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Check(string workingDirectory, string solutionsFolder)
        {
            var fi = new FileInfo(LocalSourcePath);
            if (fi.Length <= MaxSourceSize || MaxSourceSize == 0)
            {
                Score = 0;
                FailedRuns = 0;
                FailedChecks = 0;
                DoCheck(workingDirectory, solutionsFolder);
                OnPropertyChanged(ScorePropertyName);
                OnPropertyChanged(FailedChecksPropertyName);
                OnPropertyChanged(FailedRunsPropertyName);
            }
            else
            {
                throw new Exception("Ошибка: превышен предел по размеру исходного файла решения!");
            }
        }

        private void DoCheck(string workingDirectory, string solutionsFolder)
        {
            var testsFolder = FileSystem.Combine(Configuration.TestsFolder, ExecutableName);
            Compile(workingDirectory, solutionsFolder);
            var tests = FileSystem.List(testsFolder); 
            if (tests != null)
            {
                tests = tests.Where(x => !x.Contains(".a"));
                TestsCount = tests.Count();
                foreach (var infile in tests)
                {
                    var infileUrl = FileSystem.Combine(testsFolder, infile);

                    if (!Run(infileUrl, infileUrl + ".a", GetCheckerUrl()))
                    {
                        break;
                    }
                }
            }

            Clean(workingDirectory);
        }

        private string GetCheckerUrl()
        {
            return FileSystem.Combine(FileSystem.Combine(Configuration.Site, "Checkers"), CheckerName);
        }

        private bool Run(string input, string answer, string checker)
        {
            if (!File.Exists(Path))
            {
                FailedRuns++;
                return true;
            }
            var workingDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));

            Directory.CreateDirectory(workingDirectory);

            var tempInput = System.IO.Path.Combine(workingDirectory, InputFileName);
            var tempOutput = System.IO.Path.Combine(workingDirectory, OutputFileName);
            var tempAnswer = System.IO.Path.Combine(workingDirectory, AnswerFileName);
            var tempExe = System.IO.Path.Combine(workingDirectory, ExecutableName + ".exe");
            var tempChecker = System.IO.Path.Combine(workingDirectory, CheckerName);

            FileSystem.Download(input, tempInput);

            File.Copy(Path, tempExe);

            SafeDelete(tempOutput);

            try
            {
                if (RunInSandbox(workingDirectory, TimeLimit))
                {
                    FileSystem.Download(checker, tempChecker);
                    FileSystem.Download(answer, tempAnswer);

                    if (CheckOutputFile(workingDirectory, tempInput, tempOutput, tempAnswer, tempChecker))
                    {
                        Score++;
                    }
                    else
                    {
                        FailedChecks++;
                        switch (Configuration.ContestType)
                        {
                            case "KIROV":

                                break;
                            case "ACM":
                                Score = 0;
                                Clean(workingDirectory);
                                return false;
                            case "OLYMPIAD":

                                break;
                            default:

                                break;
                        }
                    }
                }
                else
                {
                    FailedRuns++;
                }
            }
            catch (SecurityException sex)
            {
                Clean(workingDirectory);
                throw new Exception("Исключение в безопасности. Причина: разрешение " + sex.PermissionType.ToString() + " с состоянием " + sex.PermissionState);
            }
            catch (FileNotFoundException fnfex)
            {
                Clean(workingDirectory);
                throw new Exception("Файл " + fnfex.FileName + " не найден.");
            }
            catch (OutOfMemoryException oomex)
            {
                GC.Collect();
                Clean(workingDirectory);
                throw new Exception(oomex.Message);
            }
            catch (Exception ex)
            {
                Clean(workingDirectory);
                throw new Exception(ex.Message);
            }

            Clean(workingDirectory);
            return true;
        }

        private bool CheckOutputFile(string workingDirectory, string tempInput, string tempOutput, string tempAnswer, string tempChecker)
        {
            var result = System.IO.Path.Combine(workingDirectory, "result.xml");
            var procInfo = new ProcessStartInfo
            {
                FileName = tempChecker,
                Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\" -appes",
                tempInput, tempOutput, tempAnswer, result),
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
            };
            var process = new Process();
            try
            {
                process.StartInfo = procInfo;
                process.Start();
                process.WaitForExit();

                var serializer = new XmlSerializer(typeof(CheckerResult));
                using (var stream = File.OpenRead(result))
                {
                    var checkResult = (CheckerResult)serializer.Deserialize(stream);

                    return checkResult.IsAccepted;
                }
            }
            finally
            {
                if (File.Exists(result))
                {
                    File.Delete(result);
                }
            }
        }

        private bool RunInSandbox(string workingDirectory, int timeLimit)
        {
            var fileNameArg = System.IO.Path.Combine(workingDirectory, ExecutableName + ".exe");

            var tempInput = System.IO.Path.Combine(workingDirectory, InputFileName);
            var tempOutput = System.IO.Path.Combine(workingDirectory, OutputFileName);

            try
            {
                var testAssembly =
                    System.Reflection.AssemblyName.GetAssemblyName(fileNameArg);
                // если после этой строки не возникло исключение BadImageFormatException, то проверяется .net сборка

                // запрещаем все AppDomain'у
                var permissionSet = new PermissionSet(PermissionState.None);

                // добавляем необходимые разрешения
                permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                permissionSet.AddPermission(new UIPermission(PermissionState.Unrestricted));
                permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, workingDirectory));
                permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, tempInput));
                permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Write, tempOutput));
                permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Append, tempOutput));
                permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, fileNameArg));

                // включаем мониторинг ЦП и памяти доменов приложений для данного процесса
                AppDomain.MonitoringIsEnabled = true;

                var sandbox = new Sandbox(workingDirectory, permissionSet);

                // заводим переменную Exception для хранения в основном потоке тех исключений, которые возникают во вторичном потоке
                Exception threadException = null;
                var newThread = new Thread(() =>
                {
                    try
                    {
                        // запускаем сборку с "чужим" кодом на исполнение
                        sandbox.ExecuteAssembly(fileNameArg);
                    }
                    catch (Exception ex)
                    {
                        // если во вторичном потоке возникло исключение, то запоминаем его
                        threadException = ex;
                    }

                });
                newThread.Start();

                var taskMemoryInspector = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    while (newThread.IsAlive)
                    {
                        if (MaxMemorySize != 0 && sandbox.GetTotalAllocatedMemorySize() > MaxMemorySize)
                        {
                            newThread.Interrupt();
                            threadException = new OutOfMemoryException("Превышено допустимое максимальное кол-во выделяемой паямти, равное " + MaxMemorySize.ToString() + " байт");
                            break;
                        }
                    }
                });

                // задаем тайм-аут для созданного потока. Если время превышено, выдаем исключение
                if (!newThread.Join(timeLimit * 1000))
                {
                    if (File.Exists(tempOutput))
                        SafeDelete(tempOutput);
                    taskMemoryInspector.Wait();
                    if (newThread.ThreadState == ThreadState.Stopped && threadException != null)
                    {
                        sandbox.Unload();
                        GC.Collect();
                        throw new Exception(threadException.Message);
                    }
                    throw new TimeoutException("Время выполнения программы было превышено.");
                }
                // если во вторичном потоке возникло исключение, то threadException не будет пуст
                if (threadException != null)
                {
                    if (File.Exists(tempOutput))
                        SafeDelete(tempOutput);
                    newThread.Interrupt();
                    taskMemoryInspector.Wait();
                    sandbox.Unload();
                    GC.Collect();
                    throw new Exception(threadException.Message);
                }

                GC.Collect();
                newThread.Interrupt();
                taskMemoryInspector.Wait();
                sandbox.Unload();

                return true;
            }
            catch (System.BadImageFormatException)
            {
                // если было поймано BadImageFormatException, то это не .net сборка
                var startInfo = new ProcessStartInfo
                {
                    FileName = fileNameArg,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = workingDirectory,
                    ErrorDialog = false,
                    LoadUserProfile = false,
                };

                var process = new Process {StartInfo = startInfo};


                if (process.Start())
                {
                    Exception threadException = null;
                    var taskMemoryInspectorForProcess = System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        while (!process.HasExited)
                        {
                            //ToDo: Не работает: разобраться с тем, какое значение в process должно сравниваться с MaxMemorySize
                            if (!process.HasExited && MaxMemorySize != 0 &&
                                (process.WorkingSet64 > MaxMemorySize ||
                                 process.PagedMemorySize64 > MaxMemorySize))
                            {
                                process.Kill();
                                if (!process.HasExited)
                                {
                                    process.WaitForExit();
                                }
                                threadException = new OutOfMemoryException("Превышено допустимое максимальное кол-во выделяемой паямти, равное " + MaxMemorySize.ToString() + " байт");
                                break;
                            }
                        }
                    });


                    if (process.WaitForExit(timeLimit * 1000))
                    {
                        if (threadException != null)
                            throw new Exception(threadException.Message);
                        return true;
                    }
                    else
                    {
                        process.Kill();
                        if (!process.HasExited)
                        {
                            process.WaitForExit();
                        }
                        if (threadException != null)
                            throw new Exception(threadException.Message);
                        throw new TimeoutException("Время выполнения программы было превышено.");
                    }
                }

                return false;
            }
        }

        #region File system helpers
        private static void SafeDelete(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private static void Clean(string folder)
        {
            if (Directory.Exists(folder))
            {
                try
                {
                    Directory.Delete(folder, true);
                }
                catch { }
            }
        }
        #endregion

        public void Upload()
        {
            if (File.Exists(LocalSourcePath))
            {
                if (!FileSystem.Exists(Configuration.SolutionsFolder))
                {
                    FileSystem.CreateFolder(Configuration.SolutionsFolder);
                }
                FileSystem.Upload(LocalSourcePath, FileSystem.Combine(Configuration.SolutionsFolder, SourceFileName));

                LocalSourcePath = FileSystem.Combine(Configuration.SolutionsFolder, SourceFileName);
                Path = LocalSourcePath;
            }
        }

        public void Compile(string workingDirectory, string solutionsFolder)
        {
            if (!DevTool.IsExeFile)
            {
                var exeFile = System.IO.Path.Combine(workingDirectory, ExecutableName + ".exe");
                SafeDelete(exeFile);
                var sourcePath = System.IO.Path.Combine(workingDirectory, SourceFileName);
                var taskUrl = solutionsFolder;
                var sourceFile = FileSystem.Combine(taskUrl, SourceFileName);
                if (FileSystem.Exists(sourceFile))
                {
                    var url = string.Format("{0}/{1}", taskUrl, SourceFileName);
                    FileSystem.Download(url, sourcePath);
                    DevTool.Compile(workingDirectory, sourcePath);
                    Path = exeFile;
                }
            }
        }

        const string LocalSorcePathPropertyName = "LocalSourcePath";
        private string _localSourcePath;
        /// <summary>
        /// Путь к файлу с исходным кодом решения на локальном компьютере. 
        /// Этот путь участник указывает при сдаче задания и отправке на проверку.
        /// </summary>
        [DataMember]
        public string LocalSourcePath
        {
            get { return _localSourcePath; }
            set
            {
                if (_localSourcePath != value)
                {
                    _localSourcePath = value;
                    OnPropertyChanged(LocalSorcePathPropertyName);
                }
            }
        }

    }
}
