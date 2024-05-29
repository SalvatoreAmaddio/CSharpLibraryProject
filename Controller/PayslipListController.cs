using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Events;
using FrontEnd.Properties;
using FrontEnd.Reports;
using MyApplication.Model;
using MyApplication.View.ReportPages;
using System.Windows;
using System.Windows.Input;

namespace MyApplication.Controller
{
    public class PayslipListController : AbstractFormListController<Payslip>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Payslip)} WHERE EmployeeID = @employeeID;";
        public ICommand OpenReportCMD { get; }
        public override int DatabaseIndex => 4;

        public PayslipListController() 
        {
            NewRecordEvent += OnNewRecordEvent;
            OpenWindowOnNew = false;
            OpenReportCMD = new CMD<Payslip>(openReport);
        }

        private void openReport(Payslip? payslip) 
        {
            Employee? employee = (Employee?)ParentRecord;
            if (employee == null) throw new Exception("Parent Record cannot be null.");
            if (payslip == null) throw new Exception("Payslip cannot be null.");
            string? month = null;
            string? year = null;
            if (payslip.DOP.HasValue) 
            {
                month = payslip?.DOP.Value.ToString("MMM");
                year = payslip?.DOP.Value.ToString("yyyy");
            }
            employee.Payslip = payslip;
            EmailSender emailSender = new("smtp.gmail.com", Sys.EmailUserName, "The Company", "Payslip");
            emailSender.AddReceiver(employee.Email, employee.FirstName);
            emailSender.Body = $"Dear {employee.FirstName},\n Please find attached your payslip.\nRegards,\nThe Company.";

            ReportViewerWindow win = new()
            {
                FileName = $"{employee.FirstName}_{employee.LastName}_Payslip_{month}_{year}",
                EmailSender = emailSender,
            };
            win.AddPage(new MyPage(employee));
            win.SelectedPage = win[0];
            win.Show();
        }


        private void OnNewRecordEvent(object? sender, EventArgs e)
        {
            Employee? employee = (Employee?)ParentRecord;
            if (employee!=null && CurrentRecord!=null) 
            {
                CurrentRecord.Employee = new(employee.EmployeeID);
                CurrentRecord.IsDirty = false;
            }
        }

        public override async void OnSubFormFilter()
        {
            QueryBuiler.Clear();
            QueryBuiler.AddParameter("employeeID", ParentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            AsRecordSource().ReplaceRange(results);
            GoFirst();
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {

        }

        protected override void Open(Payslip? model)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Payslip>> SearchRecordAsync()
        {
            throw new NotImplementedException();
        }
    }
}
