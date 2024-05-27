using Backend.Exceptions;
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

            Type? type = MainTab.GenericController();
            string? sheetName = type?.Name;

            List<ISQLModel>? data = MainTab?.CurrentTabController()?.Source.Cast<ISQLModel>()?.ToList();
            try
            {
                await Task.Run(() => WriteExcel(data, sheetName));
            }
            catch (WorkbookException ex)
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

        private Task WriteExcel(List<ISQLModel>? source, string? sheetName) 
        {
            if (source == null || string.IsNullOrEmpty(sheetName)) throw new Exception("Something went wrong here!");

            Excel excel = new();
            excel.Create();
            excel.Worksheet?.SetName(sheetName);

            string[] headers = [];

            switch (sheetName) 
            {
                case nameof(Employee):
                    headers = ["EmployeeID", "First Name", "Last Name", "DOB", "Gender", "Department", "Job Title", "Email"];
                 break;
                case nameof(Gender):
                    headers = ["GenderID", "Gender"];
                break;
                case nameof(JobTitle):
                    headers = ["TitleID", "Title"];
                break;
                case nameof(Department):
                    headers = ["DepartmentID", "Department Name"];
                break;
            }

            excel.Worksheet?.PrintHeader(headers);
            excel.Worksheet?.PrintData(source);

            try
            {
                excel.Save($"{Sys.Desktop}\\{sheetName}.xlsx");
            }
            catch (WorkbookException ex)
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
