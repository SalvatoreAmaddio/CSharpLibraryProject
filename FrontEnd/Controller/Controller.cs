using Backend.Controller;
using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Recordsource;
using FrontEnd.Events;
using FrontEnd.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// Actual implementation of <see cref="IAbstractFormController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormController<M> : AbstractSQLModelController, ISubFormController, IAbstractFormController<M> where M : AbstractModel, new()
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

        public AbstractFormController() : base()
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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm", MessageBoxButton.YesNo);
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

    /// <summary>
    /// This class extends <see cref="AbstractFormListController{M}"/> and implements <see cref="IAbstractFormListController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormListController<M> : AbstractFormController<M>, IAbstractFormListController<M> where M : AbstractModel, new()
    {
        protected FilterQueryBuilder QueryBuiler;
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public abstract string SearchQry { get; set; }
        protected abstract void Open(M? model);
        protected void OpenNew() => Open(new());

        protected readonly List<QueryParameter> SearchParameters = [];
        public AbstractFormListController() : base()
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
