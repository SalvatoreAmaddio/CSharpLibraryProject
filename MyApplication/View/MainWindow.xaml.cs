using Backend.Model;
using Backend.Office;
using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.ExtensionMethods;
using FrontEnd.Forms;
using FrontEnd.Reports;
using FrontEnd.Utils;
using MyApplication.Model;
using MyApplication.View.ReportPages;
using System.Windows;
using System.Windows.Controls;

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
        private void OnChangePasswordClicked(object sender, RoutedEventArgs e) => new ChangeUserPasswordDialog().ShowDialog();

        private void OnWalkthroughClicked(object sender, RoutedEventArgs e) => new Guide().ShowDialog();

        private void OnChangeEmailPasswordClicked(object sender, RoutedEventArgs e) => new EmailAppDialog().ShowDialog();

        private async void OnExcelClicked(object sender, RoutedEventArgs e)
        {
            MainTab.CurrentTabController().IsLoading = true;

            var x = MainTab?.CurrentTabController()?.Source.Cast<ISQLModel>().ToList();
            try
            {
                await Task.Run(() => WriteExcel(x));
            }
            catch (Exception ex)
            {
                Failure.Throw(ex.Message, "Carefull!");
                return;
            }
            finally 
            {
                MainTab.CurrentTabController().IsLoading = false;
            }

            MessageBox.Show("Done!");
        }

        private Task WriteExcel(List<ISQLModel> source) 
        {
            Excel excel = new();
            excel.Create();
            excel.Worksheet?.SetName("My Page");
            excel.Worksheet?.PrintData(source, 1);

            try
            {
                excel.Save("C:\\Users\\salva\\Desktop\\prova.xlsx");
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
            finally 
            {
                excel.Close();
            }
            return Task.CompletedTask;
        }
    }
}
