using FrontEnd.Controller;
using FrontEnd.Events;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class GenderListController : AbstractFormListController<Gender>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Gender)} WHERE LOWER(GenderName) LIKE @name";
        public override int DatabaseIndex => 1;

        public GenderListController()
        {
            OpenWindowOnNew = false;
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await SearchRecordAsync();
        }

        public override void OnOptionFilter()
        {
        }

        public override async Task SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            Source.ReplaceRecords(results);
            GoFirst();
        }

        protected override void Open(Gender? model)
        {
        }
    }
}
