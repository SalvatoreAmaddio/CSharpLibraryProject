using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
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
            ReportViewerWindow win = new()
            {
                FileName = $"{employee.FirstName}_{employee.LastName}_Payslip_{month}_{year}"
            };
            win.SendEmail += Win_SendEmail;
            win.AddPage(new MyPage(employee));
            win.SelectedPage = win[0];
            win.Show();
        }

        private async void Win_SendEmail(object? sender, EventArgs e)
        {
            Employee? employee = (Employee?)ParentRecord;
            if (employee == null) throw new Exception("Parent Record cannot be null.");
            ReportViewer? reportViewer = (ReportViewer?)sender;
            if (reportViewer == null) throw new Exception("ReportViewer is null");

            DialogResult result = ConfirmDialog.Ask("Do you want to send this Payslip by email?");
            
            if (result == DialogResult.No) return;
            reportViewer.IsLoading = true;
            Task t1 = reportViewer.PrintFixDocs();
            bool openFile = reportViewer.OpenFile;
            reportViewer.OpenFile = false;
           
            EmailSender emailSender = new ("smtp.gmail.com", FrontEndSettings.Default.EmailUserName, "The Company", "Payslip");
            emailSender.AddReceiver(employee.Email, employee.FirstName);
            emailSender.Body = $"Dear {employee.FirstName},\n Please find attached your payslip.\nRegards,\nThe Company.";
            await t1;
            emailSender.AddAttachment(reportViewer.PDFPrinterManager.FilePath);
            await Task.Run(emailSender.SendAsync);

            reportViewer.IsLoading = false;
            reportViewer.OpenFile = openFile;

            SuccessDialog.Display("Email Sent");
            //delele file
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

        public override void OnOptionFilter()
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
