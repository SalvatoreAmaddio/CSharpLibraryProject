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

namespace FrontEnd.Controller
{
    public interface IAbstractController : IAbstractSQLModelController, INotifier
    {
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

    public abstract class AbstractController<M> : AbstractSQLModelController, IAbstractFormController<M> where M : AbstractModel, new()
    {
        string _search = string.Empty;
        bool _isloading = false;
        bool _isDirty = false;
        private bool _allowNewRecord = true;
        protected ISQLModel? _currentModel;
        private string _records = string.Empty;

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
                RaisePropertyChanged(nameof(IsDirty));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;

        public override string Records { get => _records; protected set => UpdateProperty(ref value, ref _records); }
        public override ISQLModel? CurrentModel 
        { 
            get => _currentModel;
            set
            {
                UpdateProperty(ref value, ref _currentModel);
                RaisePropertyChanged(nameof(CurrentRecord));
            }
        }

        public override bool AllowNewRecord { get => _allowNewRecord; set => UpdateProperty(ref value, ref _allowNewRecord); }
        public bool IsLoading { get => _isloading; set => UpdateProperty(ref value, ref _isloading); }
        public string Search { get => _search; set => UpdateProperty(ref value, ref _search); }
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
        public M? CurrentRecord
        {
            get => (M?)CurrentModel;
            set => CurrentModel = value;
        }
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
    }

    public interface IAbstractFormListController<M> : IAbstractFormController<M> where M : ISQLModel, new()
    {
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public string Search { get; set; }
    }

    public interface IListController
    {
        public void Filter();

        /// <summary>
        /// Gets and Sets the default Search Query to be used. This property works in conjunction with a <see cref="FilterQueryBuilder"/> object.
        /// <para/>
        /// Your statement must have a WHERE clause.
        /// </summary>
        public string DefaultSearchQry { get; set; }

    }

    public abstract class AbstractListController<M> : AbstractController<M>, IListController, IAbstractFormListController<M> where M : AbstractModel, new()
    {
        protected FilterQueryBuilder QueryBuiler;
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public abstract string DefaultSearchQry { get; set; }
        protected abstract void Open(M? model);
        protected void OpenNew() => Open(new());

        protected readonly List<QueryParameter> SearchParameters = [];
        public AbstractListController() : base()
        {
            OpenCMD = new CMD<M>(Open);
            OpenNewCMD = new CMD(OpenNew);
            QueryBuiler = new(DefaultSearchQry);
        }

        public override void GoNew() => OpenNew();
        abstract public void Filter();

        /// <summary>
        /// Wrap up method for the <see cref="RecordSource.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/>
        /// </summary>
        /// <param name="qry">The query to be used, can be null</param>
        /// <param name="parameters">A list of parameters to be used, can be null</param>
        /// <returns>A RecordSource</returns>
        public Task<RecordSource> CreateFromAsyncList(string? qry = null, List<QueryParameter>? parameters = null) => RecordSource.CreateFromAsyncList(Db.RetrieveAsync(qry, parameters));
    }
}
