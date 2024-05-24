using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class PayslipListController : AbstractFormListController<Payslip>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Payslip)} WHERE EmployeeID = @employeeID;";

        public override int DatabaseIndex => 4;

        public PayslipListController() 
        {
            NewRecordEvent += OnNewRecordEvent;
            OpenWindowOnNew = false;
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
