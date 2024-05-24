using FrontEnd.Controller;
using FrontEnd.Events;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class DepartmentListController : AbstractFormListController<Department>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Department)} WHERE LOWER(DepartmentName) LIKE @name";

        public override int DatabaseIndex => 2;

        public DepartmentListController() 
        {
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            var results = await SearchRecordAsync();
            AsRecordSource().ReplaceRange(results);
            GoFirst();

        }

        public override async Task<IEnumerable<Department>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        public override void OnOptionFilter()
        {
        }

        protected override void Open(Department? model)
        {
        }
    }
}
