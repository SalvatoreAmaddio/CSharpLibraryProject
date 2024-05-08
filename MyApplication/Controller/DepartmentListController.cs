using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class DepartmentListController : AbstractListController<Department>
    {
        public override string DefaultSearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 2;

        public override void Filter()
        {
        }

        protected override void Open(Department? model)
        {
        }

    }
}
