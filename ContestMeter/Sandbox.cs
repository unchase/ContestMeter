using System;
using System.Collections.Generic;
using System.Linq;
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
            LocalFileSystem fs = new LocalFileSystem();
            if (!fs.Exists(Path.GetDirectoryName(logPath)))
                fs.CreateFolder(Path.GetDirectoryName(logPath));

            using (StreamWriter sw = new StreamWriter(logPath, true, Encoding.Default))
            {
                try
                {
                    foreach (var line in messages)
                    {
                        if (line != "" && line != null)
                            sw.WriteLine(line);
                    }
                    sw.WriteLine();
                    sw.Close();
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
        }

        public Sandbox(string workingDirectory, PermissionSet permissionSet)
        {
            WorkindDirectory = workingDirectory;
            SandboxPermissionSet = permissionSet;
            AppDomainSetup sandboxAppDomainSetup = new AppDomainSetup();
            sandboxAppDomainSetup.ApplicationBase = WorkindDirectory;
            sandboxAppDomainSetup.PrivateBinPath = WorkindDirectory;
            SandboxAppDomain = AppDomain.CreateDomain("Sandbox", new Evidence(), sandboxAppDomainSetup, SandboxPermissionSet);
        }

        public void ExecuteAssembly(string assemblyFile)
        {
            var currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = WorkindDirectory;
            SandboxAppDomain.ExecuteAssembly(assemblyFile);
            System.Environment.CurrentDirectory = currentDirectory;
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
