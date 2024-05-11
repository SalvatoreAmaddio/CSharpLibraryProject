using MyApplication.Controller;
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
using MyApplication.Model;

namespace MyApplication.View
{

    public partial class EmployeeForm : Window
    {
        public EmployeeForm() => InitializeComponent();

        public EmployeeForm(Employee? person) : this() => ((PersonController)DataContext).GoAt(person);
    }
}
