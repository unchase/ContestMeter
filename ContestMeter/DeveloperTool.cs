﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;

namespace ContestMeter.Common
{
    [DataContract]
    public class DeveloperTool
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CompileCommand { get; set; }

        [DataMember]
        public string CommandArgs { get; set; }

        [DataMember]
        public bool IsExeFile { get; set; }

        public void Compile(string workingDirectory, string path)
        {

#if DEBUG
            //File.WriteAllText(Path.Combine(Solution.LocalFolder, "log.bat"), CompileCommand + " " + string.Format(CommandArgs, path));
#endif
            ProcessStartInfo prcInfo = new ProcessStartInfo()
            {
                FileName = CompileCommand,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
                Arguments = string.Format(CommandArgs, path ),
                //RedirectStandardOutput = true,
                //RedirectStandardError = true,
                //UseShellExecute = false,
            };
            var prc = Process.Start(prcInfo);
            prc.WaitForExit();
        }
    }
}
