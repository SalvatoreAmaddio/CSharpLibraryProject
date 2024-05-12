using Backend.Model;
using FrontEnd.Controller;
using MyApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Controller
{
    public class PayslipListController : AbstractListController<Payslip>
    {
        public override string DefaultSearchQry { get; set; } = $"SELECT * FROM {nameof(Payslip)} WHERE EmployeeID = @employeeID;";

        public override int DatabaseIndex => 4;

        public PayslipListController() 
        {
            NewRecordEvent += OnNewRecordEvent;
        }

        private void OnNewRecordEvent(object? sender, EventArgs e)
        {
            Employee? employee = (Employee?)_parentRecord;
            if (employee!=null && CurrentRecord!=null) 
            {
                CurrentRecord.Employee = new(employee.EmployeeID);
                CurrentRecord.IsDirty = false;
            }
        }

        public override async void OnSubFormFilter()
        {
            QueryBuiler.Clear();
            QueryBuiler.AddParameter("employeeID", _parentRecord?.GetTablePK()?.GetValue());
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            Source.ReplaceRange(results);
            GoFirst();
        }

        public override void OnOptionFilter()
        {
        }

        protected override void Open(Payslip? model)
        {
        }
    }
}
