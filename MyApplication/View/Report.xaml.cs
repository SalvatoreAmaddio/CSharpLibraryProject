using FrontEnd.Reports;
using MyApplication.View.ReportPages;
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
            viewer.ItemsSource = reports;
            viewer.SelectedPage = reports.First();
        }
    }
}