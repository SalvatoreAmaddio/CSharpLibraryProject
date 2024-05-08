using FrontEnd.Controller;
using FrontEnd.FilterSource;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class GenderListController : AbstractListController<Gender>
    {
        public override string DefaultSearchQry { get; set; } = string.Empty;
        public override int DatabaseIndex => 1;

        public override void Filter(OnSelectedEventArgs e)
        {
        }

        protected override void Open(Gender? model)
        {
        }

    }
}
