using System.Windows;
using System.Security.Permissions;
using System.Diagnostics;

namespace RequireFullTrust
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();


            try
            {
                Message = Dummy();
            }
            catch
            {
                Message = "Не удалось получить необходимые разрешения";
            }
            DataContext = this;
        }

        public string Message { get; set; }

        [PermissionSet(SecurityAction.Demand,Name="FullTrust")]
        private string Dummy()
        {
            Process.Start("notepad");
            return "Необходимые разрешения получены";
        }
    }
}
