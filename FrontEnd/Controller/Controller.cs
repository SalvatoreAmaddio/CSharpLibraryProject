using Backend.Controller;
using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Recordsource;
using FrontEnd.Events;
using FrontEnd.Model;
using FrontEnd.Notifier;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FrontEnd.Forms;
using FrontEnd.Forms.FormComponents;

namespace FrontEnd.Controller
{
    public interface ISubFormController 
    {
        /// <summary>
        /// Holds a reference to the SubForm's ParentRecord property. This property is set by the <see cref="SetParentRecord(AbstractModel?)"/>  called within the SubForm object.
        /// </summary>
        public AbstractModel? ParentRecord { get; }

        /// <summary>
        /// This method is called by the SubForm to notify its controller that the ParentController has moved to another Record.
        /// </summary>
        /// <param name="ParentRecord"></param>
        public void SetParentRecord(AbstractModel? ParentRecord);

        /// <summary>
        /// Occurs when the SubForm is going to add a new Record.
        /// <para/>
        /// For Example:
        /// <code>
        /// public YourSubFormController() => NewRecordEvent += OnNewRecordEvent;
        /// ...
        /// private void OnNewRecordEvent(object? sender, EventArgs e) 
        /// {
        ///      Employee? employee = (Employee?)ParentRecord;
        ///      if (employee!=null) 
        ///      {
        ///           CurrentRecord.Employee = new(employee.EmployeeID);
        ///           CurrentRecord.IsDirty = false;
        ///      }
        /// }
        /// </code>
        /// </summary>
        public event NewRecordEventHandler? NewRecordEvent;

        /// <summary>
        /// Override this method to implement a custom logic to filter a SubForm object.
        /// <para/>
        /// For Example:
        /// <code>
        /// ...
        /// string sql = $"SELECT * FROM YourTable WHERE YourForeignKey = @foreignKey;";
        /// List&lt;QueryParameter> queryParameters = [];
        /// queryParameters.Add(new ("employeeID", ParentRecord?.GetTablePK()?.GetValue()));
        /// var results = await CreateFromAsyncList(sql, queryParameters);
        /// Source.ReplaceRange(results);
        /// GoFirst();
        /// ...
        /// </code>
        /// </summary>
        public void OnSubFormFilter();

    }
    public interface IAbstractController : IAbstractSQLModelController, INotifier
    {
        /// <summary>
        /// Gets and Sets a boolean indicating if the Form's ProgressBar is running/> 
        /// </summary>
        public bool IsLoading { get; set; }
    }

    public interface IAbstractFormController<M> : IAbstractController where M : ISQLModel, new()
    {
        /// <summary>
        /// A more concrete version of <see cref="IAbstractSQLModelController.CurrentModel"/>
        /// </summary>
        /// <value>The actual object that implements <see cref="IAbstractSQLModelController.CurrentModel"/></value>
        public M? CurrentRecord { get; set; }
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
    }

    public abstract class AbstractController<M> : AbstractSQLModelController, ISubFormController, IAbstractFormController<M> where M : AbstractModel, new()
    {
        string _search = string.Empty;
        bool _isloading = false;
        bool _isDirty = false;
        private bool _allowNewRecord = true;
        protected ISQLModel? _currentModel;
        private string _records = string.Empty;
        public AbstractModel? ParentRecord { get; private set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        public event NewRecordEventHandler? NewRecordEvent;
        public bool IsDirty 
        { 
            get => _isDirty; 
            set 
            { 
                _isDirty = value; 
                RaisePropertyChanged(nameof(IsDirty)); 
            } 
        }
        public override ISQLModel? CurrentModel 
        { 
            get => _currentModel;
            set
            {
                UpdateProperty(ref value, ref _currentModel);
                RaisePropertyChanged(nameof(CurrentRecord));
            }
        }

        public M? CurrentRecord
        {
            get => (M?)CurrentModel;
            set => CurrentModel = value;
        }

        public override string Records { get => _records; protected set => UpdateProperty(ref value, ref _records); }
        public override bool AllowNewRecord { get => _allowNewRecord; set => UpdateProperty(ref value, ref _allowNewRecord); }
        public bool IsLoading { get => _isloading; set => UpdateProperty(ref value, ref _isloading); }
        public string Search { get => _search; set => UpdateProperty(ref value, ref _search); }
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }

        public AbstractController() : base()
        {
            UpdateCMD = new CMD<M>(Update);
            DeleteCMD = new CMD<M>(Delete);
        }

        protected virtual void Update(M? model)
        {
            if (model == null) return;
            CurrentRecord = model;
            AlterRecord();
        }
        protected virtual void Delete(M? model)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this record?","Confirm",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;
            CurrentRecord = model;
            DeleteRecord();
        }

        public override void GoNew()
        {
            if (!AllowNewRecord) return;
            Navigator.MoveNew();
            CurrentRecord = new M();
            NewRecordEvent?.Invoke(this, EventArgs.Empty);
            Records = Source.RecordPositionDisplayer();
        }

        public void RaisePropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "")
        {
            BeforeUpdateArgs args = new(value, _backProp, propName);
            BeforeUpdate?.Invoke(this, args);
            if (args.Cancel) return;
            _backProp = value;
            IsDirty = true;
            RaisePropertyChanged(propName);
            AfterUpdate?.Invoke(this, args);
        }

        public override void AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentRecord == null) throw new NoModelException();
            if (!CurrentRecord.IsDirty) return;
            Db.Model = CurrentRecord;
            CRUD crud = (!Db.Model.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            Db.Crud(crud, sql, parameters);
            CurrentRecord.IsDirty = false;
            Db.Records?.NotifyChildren(crud, Db.Model);
            GoAt(CurrentModel);
        }

        public void SetParentRecord(AbstractModel? parentRecord)
        {
            ParentRecord = parentRecord;
            OnSubFormFilter();
        }

        public virtual void OnSubFormFilter()
        {
            throw new NotImplementedException();
        }
    }

    public interface IAbstractFormListController<M> : IAbstractFormController<M> where M : ISQLModel, new()
    {
        /// <summary>
        /// Gets and Sets the command to execute to open a Record.
        /// </summary>
        public ICommand OpenCMD { get; set; }

        /// <summary>
        /// Gets and Sets the command to execute to open a New Record.
        /// </summary>
        public ICommand OpenNewCMD { get; set; }

        /// <summary>
        /// Gets and Sets the string parameter used in a search textbox to filter the RecordSource.
        /// </summary>
        public string Search { get; set; }
    }

    /// <summary>
    /// A non generic version of <see cref="IAbstractFormListController{M}"/> which is used by Form UI Components to manage filtering operations.
    /// <para/>
    /// see also <seealso cref="RecordTracker"/>, <seealso cref="FilterOption"/>
    /// </summary>
    public interface IListController
    {
        /// <summary>
        /// Override this method to implement your filter logic. 
        /// For Example:
        /// <code>
        /// //overide SearchQry Property.
        /// public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Employee)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        /// ...
        /// public override async Task SearchRecordAsync() 
        /// {
        ///     QueryBuiler.AddParameter("name", Search.ToLower() + "%");
        ///     QueryBuiler.AddParameter("name", Search.ToLower() + "%");
        ///     var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        ///     Source.ReplaceRange(results);
        ///     GoFirst();
        /// }
        /// </code>
        /// </summary>
        /// <returns>A Taks</returns>
        public Task SearchRecordAsync();

        /// <summary>
        /// This method is called by the <see cref="Forms.FilterOption"/> object when an option is selected or unselected.
        /// It instructs the Controller to filter its RecordSource.
        /// <para/>
        /// For Example:
        /// <code>
        /// public override async void OnOptionFilter()
        /// {
        ///     QueryBuiler.Clear();
        ///     QueryBuiler.AddCondition(GenderOptions.Conditions(QueryBuiler));
        ///     ... // Other conditions if needed
        ///     await SearchRecordAsync();
        /// }
        /// </code>
        /// </summary>
        public void OnOptionFilter();

        /// <summary>
        /// Gets and Sets the Search Query to be used. This property works in conjunction with a <see cref="FilterQueryBuilder"/> object.
        /// <para/>
        /// Your statement must have a WHERE clause.
        /// <para/>
        /// For Example:
        /// <code>
        /// public override string SearchQry { get; set; } = $"SELECT * FROM Payslip WHERE EmployeeID = @ID;";
        /// //OR
        /// public override string SearchQry { get; set; } = $"SELECT * FROM Employee WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        /// </code>
        /// </summary>
        public string SearchQry { get; set; }

    }

    public abstract class AbstractListController<M> : AbstractController<M>, IListController, IAbstractFormListController<M> where M : AbstractModel, new()
    {
        protected FilterQueryBuilder QueryBuiler;
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public abstract string SearchQry { get; set; }
        protected abstract void Open(M? model);
        protected void OpenNew() => Open(new());

        protected readonly List<QueryParameter> SearchParameters = [];
        public AbstractListController() : base()
        {
            OpenCMD = new CMD<M>(Open);
            OpenNewCMD = new CMD(OpenNew);
            QueryBuiler = new(SearchQry);
            Source.RunFilter += OnSourceRunFilter;
        }

        protected void OnSourceRunFilter(object? sender, EventArgs e) => OnOptionFilter();

        public override void GoNew() => OpenNew();
        abstract public void OnOptionFilter();

        /// <summary>
        /// Wrap up method for the <see cref="RecordSource.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/>
        /// </summary>
        /// <param name="qry">The query to be used, can be null</param>
        /// <param name="parameters">A list of parameters to be used, can be null</param>
        /// <returns>A RecordSource</returns>
        public Task<RecordSource> CreateFromAsyncList(string? qry = null, List<QueryParameter>? parameters = null) => RecordSource.CreateFromAsyncList(Db.RetrieveAsync(qry, parameters));

        public abstract Task SearchRecordAsync();
    }
}
