using MyApplication.Controller;
using System.Windows.Controls;

namespace MyApplication.View
{
    public partial class JobTitleListPage : Page
    {
        public JobTitleListPage()
        {
            InitializeComponent();
            DataContext = new JobTitleListController();
        }
    }
}
