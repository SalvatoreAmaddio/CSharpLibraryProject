using FrontEnd.Controller;
using FrontEnd.FilterSource;
using MyApplication.Model;
using MyApplication.View;
using FrontEnd.Events;

namespace MyApplication.Controller
{
    public class EmployeeControllerList : AbstractFormListController<Employee>
    {
        static EmployeeControllerList() 
        { 
        
        }
        public GenderListController Genders { get; private set; } = new();
        public DepartmentListController Departments { get; private set; } = new();
        public JobTitleListController Titles { get; private set; } = new();
        public SourceOption TitleOptions { get; private set; }
        public SourceOption GenderOptions { get; private set; }
        public SourceOption DepartmentOptions { get; private set; }
        public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Employee)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        public override int DatabaseIndex => 0;
        public EmployeeControllerList()
        {
            TitleOptions = new(Titles.Source, "Title");
            GenderOptions = new(Genders.Source, "GenderName");
            DepartmentOptions = new(Departments.Source, "DepartmentName");
            AfterUpdate += OnAfterUpdate;
            OpenWindowOnNew = false;
        }

        private async void OnAfterUpdate(object? sender, AfterUpdateArgs e)
        {
            if (!e.Is(nameof(Search))) return;
            await SearchRecordAsync();
        }

        public override async Task SearchRecordAsync()
        {
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            QueryBuiler.AddParameter("name", Search.ToLower() + "%");
            var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
            Source.ReplaceRecords(results);
            GoFirst();
        }

        public override async void OnOptionFilter()
        {
            QueryBuiler.Clear();
            QueryBuiler.AddCondition(GenderOptions.Conditions(QueryBuiler));
            QueryBuiler.AddCondition(TitleOptions.Conditions(QueryBuiler));
            QueryBuiler.AddCondition(DepartmentOptions.Conditions(QueryBuiler));
            await SearchRecordAsync();
        }

        protected override void Open(Employee? model)
        {
            EmployeeForm win = new(model);
            win.Show();
        }
    }
}