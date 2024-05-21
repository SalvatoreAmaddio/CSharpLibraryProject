using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Source;

namespace Backend.Controller
{
    public abstract class AbstractSQLModelController : IAbstractSQLModelController, IDisposable
    {
        protected bool _disposed = false;
        protected bool _allowNewRecord = true;
        public bool VoidParentUpdate { get; protected set; } = false;
        public IAbstractDatabase Db { get; protected set; } = null!;
        public abstract int DatabaseIndex { get; }
        public RecordSource Source { get; protected set; }
        protected INavigator Navigator => Source.Navigate();
        public AbstractSQLModelController()
        {
            try 
            {
                Db = DatabaseManager.Do[DatabaseIndex];
            }
            catch (IndexOutOfRangeException e) 
            {
                Console.WriteLine(e.Message);
            }

            Source = new RecordSource(Db, this);
            GoFirst();
        }

        public virtual bool AllowNewRecord 
        { 
            get => _allowNewRecord; 
            set 
            {
                _allowNewRecord = value;
                Navigator.AllowNewRecord = value;
            } 
        }

        public virtual ISQLModel? CurrentModel { get; set; }
        public virtual string Records { get; protected set; } = string.Empty;
        
        protected virtual bool CanMove() 
        {
            if (CurrentModel != null)
            {
                if (CurrentModel.IsNewRecord()) return true;
                if (!CurrentModel.AllowUpdate()) return false;
            }
            return true;
        }

        public virtual void GoNext()
        {
            if (!CanMove()) return;
            bool moved = Navigator.MoveNext();
            if (!moved) return;
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoPrevious()
        {
            if (!CanMove()) return;
            Navigator.MovePrevious();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoLast()
        {
            if (!CanMove()) return;
            Navigator.MoveLast();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoFirst()
        {
            if (!CanMove()) return;
            Navigator.MoveFirst();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoNew()
        {
            if (!CanMove()) return;
            if (!AllowNewRecord) return;
            if (Navigator.IsNewRecord) return;
            Navigator.MoveNew();
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoAt(int index)
        {
            if (!CanMove()) return;
            Navigator.MoveAt(index);
            CurrentModel = Navigator.Current;
            Records = Source.RecordPositionDisplayer();
        }

        public virtual void GoAt(ISQLModel? record)
        {
            if (!CanMove()) return;
            if (record == null) CurrentModel = null;
            else if (record.IsNewRecord()) GoNew();
            else
            {
                Navigator.MoveAt(record);
                CurrentModel = Navigator.Current;
                Records = Source.RecordPositionDisplayer();
            }
        }

        public void DeleteRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentModel == null) throw new NoModelException();
            Db.Model = CurrentModel;
            Db.Crud(CRUD.DELETE, sql, parameters);
            if (Db.Model.IsNewRecord()) //this occurs in ListView objects when you add a new record but then decided to delete it.
            {
                Source.Remove(Db.Model); //remove the record from the Source, thereof from the ListView
                if (Navigator.BOF && !Navigator.NoRecords) GoFirst(); //The record is deleted, handle the direction that the Navigator object should point at.
                else GoPrevious(); //if we still have records move back.
            }
            else
                Db?.Records?.NotifyChildren(CRUD.DELETE, Db.Model); //notify children sources that the master source has changed.
        }

        public virtual bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentModel == null) throw new NoModelException();
            if (!CurrentModel.AllowUpdate()) return false; //cannot update.
            Db.Model = CurrentModel;
            CRUD crud = (!Db.Model.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            Db.Crud(crud, sql, parameters);
            Db.Records?.NotifyChildren(crud, Db.Model);
            GoAt(CurrentModel);
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}