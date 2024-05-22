using FrontEnd.Reports;
using MyApplication.Model;

namespace MyApplication.View.ReportPages
{
    public partial class MyPage : ReportPage
    {
        public MyPage()
        {
            InitializeComponent();
            Employee employee = new() 
            { 
                FirstName = "Salvatore",
                LastName = "Amaddio"
            };
            DataContext = employee;
        }
    }
}
