using MyApplication.Controller;
using System.Windows;
using MyApplication.Model;
using System.ComponentModel;

namespace MyApplication.View
{

    public partial class EmployeeForm : Window
    {
        public EmployeeForm() 
        {
            InitializeComponent();
            DataContext = new EmployeeController();
        }

        public EmployeeForm(Employee? person) : this() => ((EmployeeController)DataContext).GoAt(person);

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ((EmployeeController)DataContext).OnWindowClosing(e);
        }
    }
}
