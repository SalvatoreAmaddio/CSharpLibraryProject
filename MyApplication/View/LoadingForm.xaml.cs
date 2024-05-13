using Backend.Database;
using System.Windows;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for LoadingForm.xaml
    /// </summary>
    public partial class LoadingForm : Window
    {
        public LoadingForm()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(((Exception)e.ExceptionObject).Message);
        }

        private async void LoadingMask_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(DatabaseManager.Do.FetchData);
            Hide();
            MainWindow mainWindow = new();
            mainWindow.Show();
            Close();
        }
    }
}
