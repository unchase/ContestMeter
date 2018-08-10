using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Permissions;
using System.Diagnostics;

namespace RequireFullTrust
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
            return "Необходимые разрешения получены"; ;
        }
    }
}
