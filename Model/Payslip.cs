using Backend.Model;
using FrontEnd.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Model
{
    [Table(nameof(Payslip))]
    public class Payslip : AbstractModel
    {
        private long _payslipid;
        private Employee? _employee;
        private DateTime? _dop;
        private double _salary;

        [PK]
        public long PayslipID { get => _payslipid; set => UpdateProperty(ref value, ref _payslipid); }
        [FK]
        public Employee? Employee { get => _employee; set => UpdateProperty(ref value, ref _employee); }
        [Field]
        public DateTime? DOP { get => _dop; set => UpdateProperty(ref value, ref _dop); }
        [Field]
        public double Salary { get => _salary; set => UpdateProperty(ref value, ref _salary); }

        public Payslip() { }

        public Payslip(DbDataReader reader) 
        {
            _payslipid = reader.GetInt64(0);
            _employee = new(reader.GetInt64(1));
            _dop = reader.GetDateTime(2);
            _salary = reader.GetDouble(3);
        }

        public override bool AllowUpdate()
        {
            if (Employee == null) return false;
            if (Employee.IsNewRecord()) return false;
            return true;
        }

        public double SubtractDeductions()
        {
            return Salary - 250 - 55 - 0 - 50 - 5;
        }
        public override ISQLModel Read(DbDataReader reader) => new Payslip(reader);

    }
}
