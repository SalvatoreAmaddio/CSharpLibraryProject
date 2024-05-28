using FrontEnd.Controller;
using FrontEnd.FilterSource;
using MyApplication.Model;
using MyApplication.View;
using FrontEnd.Events;
using FrontEnd.Source;
using Backend.Database;

namespace MyApplication.Controller
{
    public class EmployeeControllerList : AbstractFormListController<Employee>
    {
        public RecordSource<Gender> Genders { get; private set; } = new(DatabaseManager.Find<Gender>()!);
        public RecordSource<Department> Departments { get; private set; } = new(DatabaseManager.Find<Department>()!);
        public RecordSource<JobTitle> Titles { get; private set; } = new(DatabaseManager.Find<JobTitle>()!);
        public SourceOption TitleOptions { get; private set; }
        public SourceOption GenderOptions { get; private set; }
        public SourceOption DepartmentOptions { get; private set; }
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Employee)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        public override int DatabaseIndex => 0;
        public EmployeeControllerList()
        {
            TitleOptions = new(Titles, "Title");
            GenderOptions = new(Genders, "GenderName");
            DepartmentOptions = new(Departments, "DepartmentName");
            AfterUpdate += OnAfterUpdate;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            var results = await Task.Run(SearchRecordAsync);
            AsRecordSource().ReplaceRange(results);

            if (sender is not FilterEventArgs filterEvtArgs) 
                GoFirst();
        }

        public override async Task<IEnumerable<Employee>> SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            return await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        }

        public override void OnOptionFilter(FilterEventArgs e)
        {
            QueryBuiler.Clear();
            QueryBuiler.AddCondition(GenderOptions.Conditions(QueryBuiler));
            QueryBuiler.AddCondition(TitleOptions.Conditions(QueryBuiler));
            QueryBuiler.AddCondition(DepartmentOptions.Conditions(QueryBuiler));
            OnAfterUpdate(e, new(null, null, nameof(Search)));
        }

        protected override void Open(Employee? model)
        {
            EmployeeForm win = new(model);
            win.Show();
        }
    }
}