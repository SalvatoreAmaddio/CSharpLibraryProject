using MyApplication.Controller;
using System.Windows.Controls;

namespace MyApplication.View
{
    /// <summary>
    /// Interaction logic for EmployeeListPage.xaml
    /// </summary>
    public partial class EmployeeListPage : Page
    {
        public EmployeeListPage()
        {
            InitializeComponent();
            DataContext = new EmployeeControllerList();
        }
    }
}