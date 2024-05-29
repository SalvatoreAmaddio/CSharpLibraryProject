using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Office;
using Backend.Utils;
using FrontEnd.Dialogs;
using FrontEnd.ExtensionMethods;
using FrontEnd.Forms;
using FrontEnd.Utils;
using MyApplication.Model;
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
                Failure.Allert(ex.Message, "Carefull!");
                return;
            }
            finally 
            {
                MainTab.CurrentTabController().IsLoading = false;
            }

            SuccessDialog.Display("Report Successfully Exported!");
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
                    headers = ["Employee ID", "First Name", "Last Name", "DOB", "Gender", "Department", "Job Title", "Email"];
                 break;
                case nameof(Gender):
                    headers = ["Gender ID", "Gender"];
                break;
                case nameof(JobTitle):
                    headers = ["Title ID", "Title"];
                break;
                case nameof(Department):
                    headers = ["Department ID", "Department Name"];
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

        private async void OnExportDBClicked(object sender, RoutedEventArgs e)
        {
            MainTab.CurrentTabController().IsLoading = true;

            try
            {
                await Task.Run(ExportDBData);
            }
            catch (WorkbookException ex)
            {
                Failure.Allert(ex.Message, "Carefull!");
                return;
            }
            finally
            {
                MainTab.CurrentTabController().IsLoading = false;
            }

            SuccessDialog.Display("Report Successfully Exported!");
        }

        public Task ExportDBData() 
        {
            Excel excel = new();
            excel.Create();

            foreach (IAbstractDatabase db in DatabaseManager.All)
            {
                if (db.MasterSource.Count == 0) continue;
                string sheetName = db.ModelType.Name;
                excel?.Worksheet?.SetName(sheetName);
                excel?.Worksheet?.PrintHeader(db.Model.GetEntityFields());
                excel?.Worksheet?.PrintData(db.MasterSource, true);
                excel?.WorkBook?.AddNewSheet();
            }

            excel?.Worksheet?.Delete();

            try
            {
                excel?.Save($"{Sys.Desktop}\\database.xlsx");
            }
            catch (WorkbookException ex)
            {
                return Task.FromException(ex);
            }
            finally
            {
                excel?.Close();
            }
            return Task.CompletedTask;
        }
    }
}
