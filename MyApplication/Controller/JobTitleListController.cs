using FrontEnd.Controller;
using FrontEnd.Events;
using MyApplication.Model;

namespace MyApplication.Controller
{
    public class JobTitleListController : AbstractFormListController<JobTitle>
    {
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(JobTitle)} WHERE LOWER(Title) LIKE @name";
        public override int DatabaseIndex => 3;

        static JobTitleListController()
        {

        }
        public JobTitleListController()
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

        public override async Task<IEnumerable<JobTitle>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        public override void OnOptionFilter()
        {
            
        }

        protected override void Open(JobTitle? model)
        {
            throw new NotImplementedException();
        }
    }
}