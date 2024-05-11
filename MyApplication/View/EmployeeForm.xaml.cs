using MyApplication.Controller;
using System.Windows;
using MyApplication.Model;

namespace MyApplication.View
{

    public partial class EmployeeForm : Window
    {
        public EmployeeForm() => InitializeComponent();

        public EmployeeForm(Employee? person) : this() => ((EmployeeController)DataContext).GoAt(person);
    }
}
