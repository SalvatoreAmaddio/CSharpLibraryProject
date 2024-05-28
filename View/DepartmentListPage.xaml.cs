using MyApplication.Controller;
using System.Windows.Controls;

namespace MyApplication.View
{
    public partial class DepartmentListPage : Page
    {
        public DepartmentListPage()
        {
            InitializeComponent();
            DataContext = new DepartmentListController();
        }
    }
}
