using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Source;
using FrontEnd.Model;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class extends <see cref="AbstractFormListController{M}"/> and implements <see cref="IAbstractFormListController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormListController<M> : AbstractFormController<M>, IDisposable, IAbstractFormListController<M> where M : AbstractModel, new()
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
            List<ISQLModel> toRemove = Source.Where(s => s.IsNewRecord()).ToList(); //get only the records which are new in the collection.

            foreach (var item in toRemove) 
                Source.Remove(item); //get rid of them.
        }

        public override void GoNew()
        {
            if (OpenWindowOnNew) 
            {
                base.GoNew(); //tell the Navigator to add a new record.
                OpenNew(); //open a new window displaying the new record.
                return;
            }
            if (!CanMove()) return; //Cannot move to a new record because the current record break integrity rules.
            if (Source.Any(s => s.IsNewRecord())) return; //If there is already a new record exit the method.
            Source.Add(new M()); //add a new record to the collection.
            Navigator.MoveLast(); //Therefore, you can now move to the last record which is indeed a new record.
            CurrentModel = Navigator.Current; //set the CurrentModel property.
            InvokeOnNewRecordEvent(); //if you are using SubForms, Invoke the the OnNewRecordEvent().
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

        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentRecord == null) throw new NoModelException();
            if (!CurrentRecord.IsDirty) return false; //if the record has not been changed there is nothing to update.
            CRUD crud = (!CurrentRecord.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            if (!CurrentRecord.AllowUpdate()) return false; //the record did not meet the criteria to be updated.
            Db.Model = CurrentRecord;
            Db.Crud(crud, sql, parameters);
            CurrentRecord.IsDirty = false;
            Db.Records?.NotifyChildren(crud, Db.Model); //tell children sources to reflect the changes occured in the master source's collection.
            if (crud == CRUD.INSERT)
            {
                GoLast();
            }
            return true;
        }

        public override void Dispose()
        {
            
        }

    }
}