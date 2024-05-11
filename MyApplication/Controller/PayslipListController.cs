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
        public override string DefaultSearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 4;

        public override void Filter()
        {
        }

        protected override void Open(Payslip? model)
        {
        }
    }
}
