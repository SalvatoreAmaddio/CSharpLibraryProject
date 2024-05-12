using FrontEnd.Controller;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class GenderListController : AbstractListController<Gender>
    {
        public override string DefaultSearchQry { get; set; } = string.Empty;
        public override int DatabaseIndex => 1;

        public override void OnOptionFilter()
        {
        }

        protected override void Open(Gender? model)
        {
        }

    }
}
