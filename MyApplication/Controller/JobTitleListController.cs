using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class JobTitleListController : AbstractFormListController<JobTitle>
    {
        public override string SearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 3;

        public override Task SearchRecordAsync()
        {
            throw new NotImplementedException();
        }

        public override void OnOptionFilter()
        {
            throw new NotImplementedException();
        }

        protected override void Open(JobTitle? model)
        {
            throw new NotImplementedException();
        }
    }
}
