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
            CRUD crud = (!CurrentRecord.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            Db.Model = CurrentRecord;
            Db.Crud(crud, sql, parameters);
            CurrentRecord.IsDirty = false;
            Db.Records?.NotifyChildren(crud, Db.Model);
            GoAt(CurrentRecord);
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
        public bool OpenWindowOnNew { get; set; } = true;

        protected readonly List<QueryParameter> SearchParameters = [];
        public AbstractFormListController() : base()
        {
            OpenCMD = new CMD<M>(Open);
            OpenNewCMD = new CMD(OpenNew);
            QueryBuiler = new(SearchQry);
            Source.RunFilter += OnSourceRunFilter;
        }
        public abstract Task SearchRecordAsync();
        public abstract void OnOptionFilter();

        /// <summary>
        /// Override this method to open a new window to view the selected record. <para/>
        /// For Example:
        /// <code>
        ///  var win = new EmployeeForm(model);
        ///  win.Show();
        /// </code>
        /// </summary>
        /// <param name="model">An <see cref="AbstractModel"/> object which is the record to visualise in the new Window</param>
        protected abstract void Open(M? model);

        /// <summary>
        /// Calls the <see cref="Open(M?)"/> by passing a new instance of <see cref="AbstractModel"/>.
        /// </summary>
        protected void OpenNew() => Open(new());
        private void OnSourceRunFilter(object? sender, EventArgs e) => OnOptionFilter();
        public override void GoNew() 
        {
            if (OpenWindowOnNew)
                OpenNew();
            else 
            {
                base.GoNew();
                if (CurrentRecord == null) throw new Exception("Cannot add a null");
                Source.Add(CurrentRecord);
            }
        }

        private void CleanSource()
        {
            if (OpenWindowOnNew) return;
            var roRemove = Source.Cast<AbstractModel>().Where(s => s.IsNewRecord() && !s.IsDirty).ToList();

            foreach (var item in roRemove)
                Source.Remove(item);
        }

        public override void GoPrevious()
        {
            CleanSource();            
            base.GoPrevious();
        }

        public override void GoLast()
        {
            CleanSource();
            base.GoLast();
        }

        public override void GoFirst()
        {
            CleanSource();
            base.GoFirst();
        }

        public override void GoAt(ISQLModel? record)
        {
            if (record == null) CurrentModel = null;
            else if (record.IsNewRecord() && OpenWindowOnNew) GoNew();
            else if (record.IsNewRecord() && !OpenWindowOnNew) 
            {
                Navigator.MoveNew();
                Navigator.Index--;
            }
            else
            {
                CleanSource();
                Navigator.MoveAt(record);
                CurrentModel = Navigator.Current;
                Records = Source.RecordPositionDisplayer();
            }
        }


        /// <summary>
        /// Wrap up method for the <see cref="RecordSource.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/>
        /// </summary>
        /// <param name="qry">The query to be used, can be null</param>
        /// <param name="parameters">A list of parameters to be used, can be null</param>
        /// <returns>A RecordSource</returns>
        public Task<RecordSource> CreateFromAsyncList(string? qry = null, List<QueryParameter>? parameters = null) => RecordSource.CreateFromAsyncList(Db.RetrieveAsync(qry, parameters));

    }
}
