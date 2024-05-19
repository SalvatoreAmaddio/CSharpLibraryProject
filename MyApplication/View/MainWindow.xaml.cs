using FrontEnd.Dialogs;
using FrontEnd.Utils;
using System.Windows;

namespace MyApplication.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Helper.ManageTabClosing(MainTab);
            Curtain.SoftwareInfo = new Backend.Utils.SoftwareInfo();
        }

        private void OpenCurtain(object sender, RoutedEventArgs e)
        {
            Curtain.Visibility = Visibility.Visible;
        }
    }
}
