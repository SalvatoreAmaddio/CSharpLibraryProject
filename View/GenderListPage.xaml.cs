using MyApplication.Controller;
using System.Windows.Controls;

namespace MyApplication.View
{
    public partial class GenderListPage : Page
    {
        public GenderListPage()
        {
            InitializeComponent();
            DataContext = new GenderListController();
        }
    }
}
