using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace ContestMeter.Common
{
    [DataContract]
    public class Participant : NotifyPropertyChanged
    {
        public IFileSystem FileSystem { get; set; }

        [DataMember]
        ObservableCollection<Solution> _solutions;

        public ObservableCollection<Solution> Solutions
        {
            get
            {
                if (_solutions == null)
                {
                    _solutions = new ObservableCollection<Solution>();
                }
                return _solutions;
            }
        }

        public Participant() : this(new FtpUtilities(), FtpUtilities.GetLocalIP4().ToString())
        {
            
        }

        private string _folder;

        public Participant(IFileSystem fs, string folder)
        {
            Check.NotNull(folder, "folder");
            _folder = folder;
            FileSystem = fs;
        }

        #region Properties

        const string ErrorPropertyName = "Error";
        private string _error = "";
        public string Error
        {
            get { return _error; }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    OnPropertyChanged(ErrorPropertyName);
                }
            }
        }
        //

        const string LastNamePropertyName = "LastName";
        private string _lastName;
        [DataMember]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(LastNamePropertyName);
                }
            }
        }

        const string FirstNamePropertyName = "FirstName";
        private string _firstName;
        [DataMember]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(FirstNamePropertyName);
                }
            }
        }

        const string MiddleNamePropertyName = "MiddleName";
        private string _middleName;
        [DataMember]
        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                if (_middleName != value)
                {
                    _middleName = value;
                    OnPropertyChanged(MiddleNamePropertyName);
                }
            }
        }

        const string GradePropertyName = "Grade";
        private int _grade;
        [DataMember]
        public int Grade
        {
            get { return _grade; }
            set
            {
                if (_grade != value)
                {
                    _grade = value;
                    OnPropertyChanged(GradePropertyName);
                }
            }
        }

        const string SchoolPropertyName = "School";
        private string _school;
        [DataMember]
        public string School
        {
            get { return _school; }
            set
            {
                if (_school != value)
                {
                    _school = value;
                    OnPropertyChanged(SchoolPropertyName);
                }
            }
        }

        const string HomeTownPropertyName = "HomeTown";
        private string _homeTown;
        [DataMember]
        public string HomeTown
        {
            get { return _homeTown; }
            set
            {
                if (_homeTown != value)
                {
                    _homeTown = value;
                    OnPropertyChanged(HomeTownPropertyName);
                }
            }
        }
        #endregion

        #region Serialization Helpers
        public void Save(string folder)
        {
            Check.NotNull(folder, "folder");

            if (!FileSystem.Exists(folder))
            {
                FileSystem.CreateFolder(folder);
            }

            FileSystem.SaveJson(FileSystem.Combine(folder, "info.json"), this);
        }

        public static Participant Load(IFileSystem fs, string folder)
        {
            Check.NotNull(folder, "folder");

            if (!fs.Exists(folder))
            {
                fs.CreateFolder(folder);
            }

            var path = fs.Combine(folder, "info.json");
            if (!fs.Exists(path))
            {
                return new Participant(fs,folder);
            }

            var result = fs.LoadJson<Participant>(path);

            result.FileSystem = fs;

            foreach (var solution in result.Solutions)
            {
                solution.FileSystem = fs;
                solution.Path = solution.LocalSourcePath;
            }

            return result;
        }
        #endregion

        #region IDataErrorInfo Members
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(LastName)
                    || string.IsNullOrEmpty(FirstName)
                    || string.IsNullOrEmpty(School)
                    || Grade < 8 || Grade > 11)
                {
                    Error = "Данные не действительны";
                    return false;
                }

                Error = "";
                return true;
            }
        }

        #endregion

        public object Discover(IConfiguration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
