using System.Runtime.Serialization;

namespace ContestMeter.Common
{
    [DataContract]
    public class Task : NotifyPropertyChanged
    {
        private string _name;
        [DataMember]
        public string Name
        {
            get{ return _name; }
            set { if (_name != value )
            {
                _name  = value;
                OnPropertyChanged("Name");
            }
            }
        }


        private string _executableName;
        [DataMember]
        public string ExecutableName
        {
            get { return _executableName; }
            set
            {
                if (_executableName != value)
                {
                    _executableName = value;
                    OnPropertyChanged("ExecutableName");
                }
            }
        }

        const string CheckerNamePropertyName = "CheckerName";
        private string _checkerName;
        [DataMember]
        public string CheckerName
        {
            get { return _checkerName; }
            set
            {
                if (_checkerName != value)
                {
                    _checkerName = value;
                    OnPropertyChanged(CheckerNamePropertyName);
                }
            }
        }

        const string TestsFolderPropertyName = "TestsFolder";
        private string _testsFolder;
        [DataMember]
        public string TestsFolder
        {
            get { return _testsFolder; }
            set
            {
                if (_testsFolder != value)
                {
                    _testsFolder = value;
                    OnPropertyChanged(TestsFolderPropertyName);
                }
            }
        }

        private int _timeLimit;
        [DataMember]
        public int TimeLimit
        {
            get { return _timeLimit; }
            set
            {
                if (_timeLimit != value)
                {
                    _timeLimit = value;
                    OnPropertyChanged("TimeLimit");
                }
            }
        }

        const string WeightPropertyName = "Weight";
        private int _weight;
        [DataMember]
        public int Weight
        {
            get { return _weight; }
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged(WeightPropertyName);
                }
            }
        }

        private int _maxSourceSize;
        [DataMember]
        public int MaxSourceSize
        {
            get { return _maxSourceSize; }
            set
            {
                if (_maxSourceSize != value)
                {
                    _maxSourceSize = value;
                    OnPropertyChanged("MaxSourceSize");
                }
            }
        }

        private int _maxMemorySize;
        [DataMember]
        public int MaxMemorySize
        {
            get { return _maxMemorySize; }
            set
            {
                if (_maxMemorySize != value)
                {
                    _maxMemorySize = value;
                    OnPropertyChanged("MaxMemorySize");
                }
            }
        }
        //
    }
}
