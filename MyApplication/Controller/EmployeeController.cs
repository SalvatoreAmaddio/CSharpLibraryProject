using Backend.Database;
using Backend.Source;
using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class EmployeeController : AbstractFormController<Employee>
    {
        public RecordSource Genders { get; private set; } = new(DatabaseManager.Do.Find<Gender>());
        public RecordSource Departments { get; private set; } = new(DatabaseManager.Do.Find<Department>());
        public RecordSource Titles { get; private set; } = new(DatabaseManager.Do.Find<JobTitle>());
        public PayslipListController Payslips { get; } = new();
        public override int DatabaseIndex => 0;
    }
}
