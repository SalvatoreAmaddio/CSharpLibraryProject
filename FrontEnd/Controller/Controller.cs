using Backend.Controller;
using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Source;
using FrontEnd.Events;
using FrontEnd.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class extends <see cref="AbstractSQLModelController"/> and implementats <see cref="IAbstractFormController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormController<M> : AbstractSQLModelController, ISubFormController, IAbstractFormController<M> where M : AbstractModel, new()
    {
        string _search = string.Empty;
        bool _isloading = false;
        protected ISQLModel? _currentModel;
        private string _records = string.Empty;
        public AbstractModel? ParentRecord { get; private set; }
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

        public override bool AllowNewRecord 
        { 
            get => _allowNewRecord;
            set
            {
                UpdateProperty(ref value, ref _allowNewRecord);
                base.AllowNewRecord = value;
            }
        }

        public bool IsLoading { get => _isloading; set => UpdateProperty(ref value, ref _isloading); }
        public string Search { get => _search; set => UpdateProperty(ref value, ref _search); }
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
        public ICommand RequeryCMD { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        public event NewRecordEventHandler? NewRecordEvent;

        public AbstractFormController() 
        {
            UpdateCMD = new CMD<M>(Update);
            DeleteCMD = new CMD<M>(Delete);
            RequeryCMD = new CMDAsync(Requery);
        }

        protected override bool CanMove()
        {
            if (CurrentRecord != null)
            {
                if (CurrentRecord.IsNewRecord() && !CurrentRecord.IsDirty) return true;
                if (!CurrentRecord.AllowUpdate()) return false;
            }
            return true;
        }

        /// <summary>
        /// This method is called by <see cref="RequeryCMD"/> command to Requery the database table.
        /// It awaits the <see cref="RecordSource.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/> whose result is then used to replace the records kept in the <see cref="IAbstractSQLModelController.Db"/> property and in the <see cref="IAbstractSQLModelController.Source"/> property.
        /// </summary>
        protected virtual async Task Requery() 
        {
            IsLoading = true;
            RecordSource? results = null;
            await Task.Run(async () => 
            {
                results = await RecordSource.CreateFromAsyncList(Db.RetrieveAsync());

            });

            if (results == null) throw new Exception("Source is null");
            Db.Records?.ReplaceRange(results);
            Source.ReplaceRange(results);
            IsLoading = false;
        }

        public bool PerformUpdate() => Update(CurrentRecord);

        /// <summary>
        /// This method is called by <see cref="UpdateCMD"/> command to perform an Update or Insert CRUD operation.
        /// </summary>
        /// <param name="model">The record that must be inserted or updated</param>
        /// <returns>true if the operation was successful</returns>
        protected virtual bool Update(M? model)
        {
            if (model == null) return false;
            CurrentRecord = model;
            return AlterRecord();
        }

        /// <summary>
        /// This method is called by <see cref="DeleteCMD"/> command to perform a Delete CRUD operation.
        /// </summary>
        /// <param name="model">The record that must be deleted</param>
        /// <returns>true if the operation was successful</returns>
        protected virtual void Delete(M? model)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;
            CurrentRecord = model;
            DeleteRecord();
        }

        public override void GoNew()
        {
            if (!CanMove() || !Navigator.MoveNew()) return;
            CurrentRecord = new M();
            InvokeOnNewRecordEvent();
            Records = Source.RecordPositionDisplayer();
        }

        protected void InvokeOnNewRecordEvent() => NewRecordEvent?.Invoke(this, EventArgs.Empty);

        public void RaisePropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "")
        {
            BeforeUpdateArgs args = new(value, _backProp, propName);
            BeforeUpdate?.Invoke(this, args);
            if (args.Cancel) return;
            _backProp = value;
            RaisePropertyChanged(propName);
            AfterUpdate?.Invoke(this, args);
        }

        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentModel == null) throw new NoModelException();
            if (!((AbstractModel)CurrentModel).IsDirty) return false;
            CRUD crud = (!CurrentModel.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            if (!CurrentModel.AllowUpdate()) return false;
            Db.Model = CurrentModel;
            Db.Crud(crud, sql, parameters);
            ((AbstractModel)CurrentModel).IsDirty = false;
            Db.Records?.NotifyChildren(crud, Db.Model);
            if (crud == CRUD.INSERT) GoLast();
            return true;
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

        public void OnWindowClosing(CancelEventArgs e)
        {
            bool dirty = Source.Any(s => ((M)s).IsDirty);
            e.Cancel = dirty;

            if (dirty)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save your changes before closing?", "Wait", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) 
                {
                    CurrentRecord?.Undo();
                    e.Cancel = false;
                }
                else 
                {
                    bool updateResult = PerformUpdate();
                    e.Cancel = !updateResult;
                }
            }
        }
    }

    /// <summary>
    /// This class extends <see cref="AbstractFormListController{M}"/> and implements <see cref="IAbstractFormListController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormListController<M> : AbstractFormController<M>, IAbstractFormListController<M> where M : AbstractModel, new()
    {
        bool _openWindowOnNew = true;
        protected FilterQueryBuilder QueryBuiler;
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public abstract string SearchQry { get; set; }
        public bool OpenWindowOnNew
        {
            get => _openWindowOnNew;
            set 
            {
                _openWindowOnNew = value;
                VoidParentUpdate = !value;
            }
        }

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
        ///  EmployeeForm win = new (model);
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
        public void CleanSource()
        {
            if (OpenWindowOnNew) return;
            List<ISQLModel> toRemove = Source.Where(s => s.IsNewRecord()).ToList();

            foreach (var item in toRemove)
                Source.Remove(item);
        }
        public override void GoNew()
        {
            if (OpenWindowOnNew) 
            {
                base.GoNew();
                OpenNew();
                return;
            }
            if (!CanMove()) return;
            if (Source.Any(s => s.IsNewRecord())) return; //If there is already a new record exit the method.
            Source.Add(new M()); //add a new record to the collection.
            Navigator.MoveLast(); //Therefore, you can now move to the last record which is indeed a new record.
            CurrentModel = Navigator.Current; //set the CurrentModel property.
            InvokeOnNewRecordEvent(); //Invoke the the OnNewRecordEvent() if it has been subscribed.
            Records = "New Record"; //update RecordTracker's record displayer.
        }
        public override void GoPrevious()
        {
            if (!CanMove()) return;
            CleanSource();            
            base.GoPrevious();
        }
        public override void GoLast()
        {
            if (!CanMove()) return;
            CleanSource();
            base.GoLast();
        }
        public override void GoFirst()
        {
            if (!CanMove()) return;
            CleanSource();
            base.GoFirst();
        }

        public override void GoAt(ISQLModel? record)
        {
            if (!CanMove()) return;
            if (record == null) CurrentModel = null;
            else if (record.IsNewRecord() && OpenWindowOnNew) GoNew();
            else if (record.IsNewRecord() && !OpenWindowOnNew) Navigator.MoveNew();
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