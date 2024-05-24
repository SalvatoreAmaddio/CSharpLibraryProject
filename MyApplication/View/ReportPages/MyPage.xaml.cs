using Backend.Database;
using FrontEnd.Reports;
using MyApplication.Model;

namespace MyApplication.View.ReportPages
{
    public partial class MyPage : ReportPage
    {
        public MyPage()
        {
            InitializeComponent();
        }

        public MyPage(Employee employee) : this()
        {
            FirstName.Content = $"{employee.FirstName} {employee.LastName}";
            Email.Content = employee.Email;
            JobTitle.Content = $"Job Title: {DatabaseManager.Find<JobTitle>().Records.FirstOrDefault(s => s.Equals(employee.JobTitle))}";
            Department.Content = $"Department: {DatabaseManager.Find<Department>().Records.FirstOrDefault(s => s.Equals(employee.Department))}";
            PayDate.Content = employee?.Payslip?.DOP.Value.ToString("dd/MM/yyyy");
            EmployeeID.Content = employee?.EmployeeID;
            Salary.Content = $"£{employee?.Payslip?.Salary.ToString("N2")}";
            NetPay.Content = $"£{employee?.Payslip?.SubtractDeductions().ToString("N2")}";
        }
    }
}
