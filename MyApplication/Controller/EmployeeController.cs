using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class EmployeeController : AbstractController<Employee>
    {
        public GenderListController Genders { get; } = new();
        public DepartmentListController Departments { get; } = new();
        public JobTitleListController Titles { get; } = new();
        public PayslipListController Payslips { get; } = new();
        public override int DatabaseIndex => 0;
    }
}
