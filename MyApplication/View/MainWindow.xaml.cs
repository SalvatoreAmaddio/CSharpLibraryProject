using Backend.Utils;
using FrontEnd.ExtensionMethods;
using FrontEnd.Forms;
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
            Curtain.SoftwareInfo = new SoftwareInfo("Salvatore Amaddio","www.salvatoreamaddio.co.uk","Mister J","2024");
        }

        private void OpenCurtain(object sender, RoutedEventArgs e) => Curtain.Open();
        private void OnLogoutClicked(object sender, RoutedEventArgs e) => Helper.Logout(new LoginForm());
        private void OnChangePasswordClicked(object sender, RoutedEventArgs e) => Helper.ChangeUserPassword("soloio59");

        private void OnWalkthroughClicked(object sender, RoutedEventArgs e)
        {
            new Guide().ShowDialog();
        }
    }
}
