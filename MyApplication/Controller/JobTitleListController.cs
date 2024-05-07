using FrontEnd.Controller;
using FrontEnd.FilterSource;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class JobTitleListController : AbstractListController<JobTitle>
    {
        public override string DefaultSearchQry { get; set; } = string.Empty;

        public override int DatabaseIndex => 3;

        public override void Filter(OnSelectedEventArgs e)
        {
        }

        protected override void Open(JobTitle? model)
        {
        }

        protected override void OpenNew(JobTitle? model)
        {
        }
    }
}
