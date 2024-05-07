using Backend.Database;
using MyApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
