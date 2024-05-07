using FrontEnd.Controller;
using FrontEnd.FilterSource;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class DepartmentListController : AbstractListController<Department>
    {
        public override string DefaultSearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 2;

        public override void Filter(OnSelectedEventArgs e)
        {
        }

        protected override void Open(Department? model)
        {
        }

        protected override void OpenNew(Department? model)
        {
        }
    }
}
