using System;
using System.Text;
using System.Security;
using System.Security.Policy;
using System.IO;

namespace ContestMeter.Common
{
    public class Sandbox
    {
        private AppDomain SandboxAppDomain { get; set; }
        private PermissionSet SandboxPermissionSet { get; set; }
        private string WorkindDirectory { get; set; }

        public static void Write2LogFile(string logPath, string[] messages)
        {
            var fs = new LocalFileSystem();
            if (!fs.Exists(Path.GetDirectoryName(logPath)))
                fs.CreateFolder(Path.GetDirectoryName(logPath));

            using (var sw = new StreamWriter(logPath, true, Encoding.Default))
            {
                try
                {
                    foreach (var line in messages)
                    {
                        if (!string.IsNullOrEmpty(line))
                            sw.WriteLine(line);
                    }
                    sw.WriteLine();
                    sw.Close();
                }
                finally
                {
                    sw.Close();
                }
            }
        }

        public Sandbox(string workingDirectory, PermissionSet permissionSet)
        {
            WorkindDirectory = workingDirectory;
            SandboxPermissionSet = permissionSet;
            var sandboxAppDomainSetup = new AppDomainSetup
            {
                ApplicationBase = WorkindDirectory,
                PrivateBinPath = WorkindDirectory
            };
            SandboxAppDomain = AppDomain.CreateDomain("Sandbox", new Evidence(), sandboxAppDomainSetup, SandboxPermissionSet);
        }

        public void ExecuteAssembly(string assemblyFile)
        {
            var currentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = WorkindDirectory;
            SandboxAppDomain.ExecuteAssembly(assemblyFile);
            Environment.CurrentDirectory = currentDirectory;
        }

        public string GetWorkingDirectory()
        {
            return WorkindDirectory;
        }

        public long GetTotalAllocatedMemorySize()
        {
            return SandboxAppDomain.MonitoringTotalAllocatedMemorySize;
        }

        public void Unload()
        {
            AppDomain.Unload(SandboxAppDomain);
        }
    }
}
