using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class DepartmentListController : AbstractFormListController<Department>
    {
        public override string SearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 2;

        public override Task SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        public override void OnOptionFilter()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Department? model)
        {
            throw new NotImplementedException();
        }

    }
}
