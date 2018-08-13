using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ContestMeter.Common
{
    [DataContract]
    public class Configuration : NotifyPropertyChanged, IConfiguration
    {
        public IFileSystem FileSystem { get; set; }
        public Configuration(){}

        [DataMember]
        ObservableCollection<Task> _tasks = new ObservableCollection<Task>();
        public ObservableCollection<Task> Tasks
        {
            get { return _tasks; }
        }

        [DataMember]
        ObservableCollection<DeveloperTool> _devTools = new ObservableCollection<DeveloperTool>();
        public ObservableCollection<DeveloperTool> DevTools
        {
            get
            {
                return _devTools;
            }
        }

        public string SolutionsFolder
        {
            //get
            //{
            //    return FileSystem.Combine(Site, FtpUtilities.GetLocalIP4().ToString());
            //}
            get;
            private set;
        }

        public string TestsFolder
        {
            get
            {
                return FileSystem.Combine(Site, "Tests");
            }
        }

        const string InfoPathPropertyName = "InfoPath";
        public string InfoPath
        {
            get
            {
                return FileSystem.Combine(SolutionsFolder, "info.json");
            }
        }

        [DataMember]
        private string _contestType;
        public string ContestType
        {
            get
            {
                return _contestType;
            }
            set
            {
                if (value != null)
                    _contestType = value;
            }
        }

        [DataMember]
        private string _contestName;
        public string ContestName
        {
            get
            {
                return _contestName;
            }
            set
            {
                if (value != null)
                    _contestName = value;
            }
        }
        //

        public Task AddNewTask()
        {
            var task = new Task();
            _tasks.Add(task);
            OnPropertyChanged("Tasks");
            return task;
        }

        public void RemoveTask(Task task)
        {
            if (_tasks.Contains(task))
            {
                _tasks.Remove(task);
                OnPropertyChanged("Tasks");
            }
        }

        private const string _fileName = "config.json";

        public static IConfiguration Load(IFileSystem fs, string url, string participantUrl)
        {
            Check.NotNull(fs, "fs");
            Configuration config = null;

            var path = fs.Combine(url, _fileName);

            if (fs.Exists(path))
            {
                config = fs.LoadJson<Configuration>(path);
            }

            if (config == null)
            {
                config = new Configuration();
            }

            config.Site = url;
            config.SolutionsFolder = participantUrl;
            config.FileSystem = fs;

            return config;
        }

        public void Save()
        {
            FileSystem.SaveJson(FileSystem.Combine(Site, _fileName), this);
        }

        public const string SitePropertyName = "Site";
        private string _site;
        public string Site
        {
            get { return _site; }
            set
            {
                if (_site != value)
                {
                    _site = value;
                    OnPropertyChanged(SitePropertyName);
                }
            }
        }

        public DeveloperTool AddNewTool()
        {
            var tool = new DeveloperTool();
            _devTools.Add(tool);
            return tool;
        }

        public void RemoveTool(DeveloperTool tool)
        {
            if (_devTools.Contains(tool))
            {
                _devTools.Remove(tool);
            }
        }
    }


}
