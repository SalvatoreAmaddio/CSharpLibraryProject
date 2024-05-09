using FrontEnd.Reports;
using MyApplication.View.ReportPages;
using System.Windows;
using System.Windows.Controls;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : Page
    {
        public List<ReportPage> reports { get; } = [];
        public Report()
        {
            InitializeComponent();
            reports.Add(new MyPage());
            reports.Add(new MyPage());
            viewer.ItemsSource = reports;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //PrintGrids_Click();
            //PrintDialog printDialog = new PrintDialog();
            //if (printDialog.ShowDialog() == true)
            //{
            //    // This assumes you have a Grid named 'myGrid' you want to print
            //    printDialog.PrintVisual(Page, "Printing Grid");
            //}
        }

    }
}