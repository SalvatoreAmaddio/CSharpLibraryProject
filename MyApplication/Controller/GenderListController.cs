using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class GenderListController : AbstractListController<Gender>
    {
        public override string SearchQry { get; set; } = string.Empty;
        public override int DatabaseIndex => 1;

        public override void OnOptionFilter()
        {
            throw new NotImplementedException();
        }

        public override Task SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        protected override void Open(Gender? model)
        {
            throw new NotImplementedException();
        }
    }
}
